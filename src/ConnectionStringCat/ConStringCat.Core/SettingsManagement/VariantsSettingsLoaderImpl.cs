using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ConStringCat.Core.Model;
using ConStringCat.Core.VSInterop;
using System.IO;
using System.Linq;
using System.Reflection;
using ConStringCat.Core.Utils;
using ConStringCat.Core.ValueUpdating;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ConStringCat.Core.SettingsManagement
{
	public class VariantsSettingsLoaderImpl : VariantsSettingsLoader
	{
		public const string SettingsFileSuffix = ".ConStrCat.json";

		private const string JsonUpdaterType = "jsonPath";

		private const string XmlUpdaterType = "xPath";

		private const string SetsProperty = "sets";

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

		private JsonSchema _schema;

		public ConnectionStringVariantsSet GetEmptyVariantsSet()
		{
			return NullConnectionStringVariantsSet.Instance;
		}

		public ConnectionStringVariantsSet LoadVariantsSetForSolution(string solutionFileName)
		{
			if (string.IsNullOrEmpty(solutionFileName) || !File.Exists(solutionFileName))
				return GetEmptyVariantsSet();

			var settingsFileName = GetSettingsFileName(solutionFileName);
			var settings = LoadSettings(settingsFileName);
			var setSettings = settings[SetsProperty][0]; //Only one set supported yet
			return InitVariantsSet(setSettings);
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
			if (!File.Exists(settingsFileName))
				CreateDefaultSettingsFile(settingsFileName);
			var settings = ReadSettingsFromFile(settingsFileName);
			ValidateSettings(settings);
			return settings;
		}

		private void CreateDefaultSettingsFile(string settingsFileName)
		{
			using (var writer = new StreamWriter(settingsFileName))
				writer.Write(EmbeddedResourcesHelper.ReadEmbeddedResourceFile(DefaultSettingsResourceName));
		}

		private static JObject ReadSettingsFromFile(string settingsFileName)
		{
			try
			{
				using (var reader = new StreamReader(settingsFileName))
					return JObject.Parse(reader.ReadToEnd());
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

		private ConnectionStringVariantsSet InitVariantsSet(JToken setSettings)
		{
			var variantsSet = new ConnectionStringVariantsSetImpl(ReadValue(setSettings, NameProperty));

			foreach (var variant in setSettings[VariantsProperty])
				variantsSet.AddVariant(ReadValue(variant, VariantAliasProperty),ReadValue(variant, VariantValueProperty));
			foreach (var updaterSettings in setSettings[UpdatersProperty])
				variantsSet.AddUpdater(CreateUpdater(updaterSettings));

			return variantsSet;
		}

		private string ReadValue(JToken token, string key)
		{
			return token[key].Value<string>();
		}

		private ConnectionStringUpdater CreateUpdater(JToken updaterSettings)
		{
			var updaterType = ReadValue(updaterSettings, UpdaterTypeProperty);
			var filePath = ConvertToAbsolutePath(ReadValue(updaterSettings, UpdaterFilePathProperty));
			if (updaterType == JsonUpdaterType)
				return CreateJsonUpdater(filePath, ReadValue(updaterSettings, UpdaterJsonPathProperty));
			if (updaterType == XmlUpdaterType)
				return CreateXmlUpdater(filePath, ReadValue(updaterSettings, UpdaterXPathProperty));
			throw new VariantsSettingsLoadingException(string.Format(UnsupportedUpdaterTypeMsg, updaterType));
		}

		private string ConvertToAbsolutePath(string pathFromSettings)
		{
			return Path.Combine(Directory.GetCurrentDirectory(), pathFromSettings);
		}

		private ConnectionStringUpdater CreateXmlUpdater(string filePath, string xmlPath)
		{
			return new XmlFileConnectionStringUpdater(filePath, xmlPath);
		}

		private ConnectionStringUpdater CreateJsonUpdater(string filePath, string jsonPath)
		{
			return new JsonFileConnectionStringUpdater(filePath, jsonPath);
		}
	}
}
