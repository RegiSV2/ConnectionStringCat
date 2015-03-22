using ConStringCat.Core.Model;
using ConStringCat.Core.SettingsManagement;
using System;

namespace ConStringCat.Core.ValueUpdating
{
	public class ConfigurationValueUpdaterFactoryImpl : ConfigurationValueUpdaterFactory
	{
		public ConfigurationValueUpdater CreateXmlUpdater(string filePath, string xmlPath)
		{
			return new XmlFileConfigurationValueUpdater(filePath, xmlPath);
		}

		public ConfigurationValueUpdater CreateJsonUpdater(string filePath, string jsonPath)
		{
			return new JsonFileConfigurationValueUpdater(filePath, jsonPath);
		}
	}
}
