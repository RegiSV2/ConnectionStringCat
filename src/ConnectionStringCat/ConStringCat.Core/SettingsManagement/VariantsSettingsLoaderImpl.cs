﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConStringCat.Core.Model;
using ConStringCat.Core.Utils;
using ConStringCat.Core.ValueUpdating;
using ConStringCat.Core.VSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ConStringCat.Core.SettingsManagement
{
	public class VariantsSettingsLoaderImpl : VariantsSettingsLoader
	{
		#region Constants

		public const string SettingsFileSuffix = ".ConStrCat.json";
		private const string JsonUpdaterType = "jsonPath";
		private const string XmlUpdaterType = "xPath";
		private const string AspectsProperty = "aspects";
		private const string SetsProperty = "valueSets";
		private const string NameProperty = "name";
		private const string VariantsProperty = "variants";
		private const string UpdatersProperty = "updaters";
		private const string VariantAliasProperty = "alias";
		private const string VariantValueProperty = "value";
		private const string UpdaterTypeProperty = "type";
		private const string UpdaterXPathProperty = "xPath";
		private const string UpdaterJsonPathProperty = "jsonPath";
		private const string UpdaterFilePathProperty = "filePath";
		private const string SettingsSchemaResourceName = "ConStringCat.Core.SettingsManagement.VariantSettingsSchema.json";
		private const string DefaultSettingsResourceName = "ConStringCat.Core.SettingsManagement.DefaultSettings.json";

		private const string UnsupportedUpdaterTypeMsg =
			"An unsupported updater type (\"{0}\") was encountered while reading configuration file";

		private const string SettingsFileIsNotJsonMsg =
			"Could not load ConnectionStringCat settings because settings file is not JSON";

		private const string SettingsFileFormatIsInvalidMsg =
			"Could not load ConnectionStringCat settings because settings file is invalid. The following errors found:\n\r{0}";

		#endregion

		#region Private fields

		private JsonSchema _schema;
		private readonly ConfigurationValueUpdaterFactory _updaterFactory;

		#endregion

		public VariantsSettingsLoaderImpl(ConfigurationValueUpdaterFactory updaterFactory)
		{
			_updaterFactory = updaterFactory;
		}

		public ConfigurationAliasesEntity GetEmptyAspect()
		{
			return NullConfigurationAliasesEntity.Instance;
		}

		public IList<ConfigurationAliasesEntity> LoadAspectsForSolution(string solutionFileName)
		{
			if (string.IsNullOrEmpty(solutionFileName) || !File.Exists(solutionFileName))
				return new List<ConfigurationAliasesEntity>();

			var settingsFileName = GetSettingsFileName(solutionFileName);
			var settings = LoadSettings(settingsFileName);
			return InitConfigurationAspects(Path.GetDirectoryName(solutionFileName), settings[AspectsProperty]);
		}

		public bool SettingsExist(string solutionFileName)
		{
			var settingsFileName = GetSettingsFileName(solutionFileName);
			return File.Exists(settingsFileName);
		}

		private string GetSettingsFileName(string solutionFileName)
		{
			var fileName = Path.GetFileNameWithoutExtension(solutionFileName) + SettingsFileSuffix;
			var directory = Path.GetDirectoryName(solutionFileName);
			if (directory != null)
				fileName = Path.Combine(directory, fileName);
			return fileName;
		}

		private JObject LoadSettings(string settingsFileName)
		{
			string settingsRaw = ReadRawSettings(settingsFileName);
			var settings = ParseSettingsFromString(settingsRaw);
			ValidateSettings(settings);
			return settings;
		}

		private static string ReadRawSettings(string settingsFileName)
		{
			string settingsRaw;
			if (File.Exists(settingsFileName))
			{
				using (var reader = new StreamReader(settingsFileName))
					settingsRaw = reader.ReadToEnd();
			}
			else
			{
				settingsRaw = EmbeddedResourcesHelper.ReadEmbeddedResourceFile(DefaultSettingsResourceName);
			}
			return settingsRaw;
		}

		private static JObject ParseSettingsFromString(string settingsString)
		{
			try
			{
				return JObject.Parse(settingsString);
			}
			catch (JsonReaderException ex)
			{
				throw new VariantsSettingsLoadingException(SettingsFileIsNotJsonMsg, ex);
			}
		}

		private void ValidateSettings(JObject settings)
		{
			var schema = InitSchema();

			IList<string> errorMessages;
			if (!settings.IsValid(schema, out errorMessages))
			{
				throw new VariantsSettingsLoadingException(
					string.Format(SettingsFileFormatIsInvalidMsg, errorMessages.Aggregate((a, b) => a + "\r\n" + b)));
			}
		}

		private JsonSchema InitSchema()
		{
			if (_schema == null)
			{
				var schemaRaw = EmbeddedResourcesHelper.ReadEmbeddedResourceFile(SettingsSchemaResourceName);
				_schema = JsonSchema.Parse(schemaRaw);
			}
			return _schema;
		}

		private IList<ConfigurationAliasesEntity> InitConfigurationAspects(string solutionDirectory, JToken aspectsArray)
		{
			var aspects = aspectsArray.Select(x => InitConfigurationAspect(solutionDirectory, x)).ToList();
			foreach (var aspect in aspects)
				aspect.RefreshSetVariants();
			return aspects.Cast<ConfigurationAliasesEntity>().ToList();
		}

		private ConfigurationAspect InitConfigurationAspect(string solutionDirectory, JToken aspectToken)
		{
			var aspect = new ConfigurationAspect(ReadValue(aspectToken, NameProperty));

			foreach (var variant in aspectToken[VariantsProperty])
				aspect.AddAlias(variant.Value<string>());
			foreach (var set in aspectToken[SetsProperty].Select(x => InitVariantsSet(solutionDirectory, x)))
				aspect.AddVariantsSet(set);

			return aspect;
		}

		private ConfigurationVariantsSet InitVariantsSet(string solutionDirectory, JToken setSettings)
		{
			var variantsSet = new ConfigurationValueVariantsSet(ReadValue(setSettings, NameProperty));

			foreach (var variant in setSettings[VariantsProperty])
				variantsSet.AddVariant(ReadValue(variant, VariantAliasProperty), ReadValue(variant, VariantValueProperty));
			foreach (var updaterSettings in setSettings[UpdatersProperty])
				variantsSet.AddUpdater(CreateUpdater(solutionDirectory, updaterSettings));

			return variantsSet;
		}

		private string ReadValue(JToken token, string key)
		{
			return token[key].Value<string>();
		}

		private ConfigurationValueUpdater CreateUpdater(string solutionDirectory, JToken updaterSettings)
		{
			var updaterType = ReadValue(updaterSettings, UpdaterTypeProperty);
			var filePath = ConvertToAbsolutePath(solutionDirectory, ReadValue(updaterSettings, UpdaterFilePathProperty));
			if (updaterType == JsonUpdaterType)
				return _updaterFactory.CreateJsonUpdater(filePath, ReadValue(updaterSettings, UpdaterJsonPathProperty));
			if (updaterType == XmlUpdaterType)
				return _updaterFactory.CreateXmlUpdater(filePath, ReadValue(updaterSettings, UpdaterXPathProperty));
			throw new VariantsSettingsLoadingException(string.Format(UnsupportedUpdaterTypeMsg, updaterType));
		}

		private string ConvertToAbsolutePath(string solutionDirectory, string pathFromSettings)
		{
			return Path.Combine(solutionDirectory, pathFromSettings);
		}
	}
}