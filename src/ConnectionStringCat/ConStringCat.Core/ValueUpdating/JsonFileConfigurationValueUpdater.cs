using System;
using ConStringCat.Core.Model;

namespace ConStringCat.Core.ValueUpdating
{
	/// <summary>
	///     Updates values in json file
	/// </summary>
	public sealed class JsonFileConfigurationValueUpdater : ConfigurationValueUpdater
	{
		public JsonFileConfigurationValueUpdater(string documentPath, string jsonPath)
		{
			DocumentPath = documentPath;
			JsonPath = jsonPath;
		}

		public string DocumentPath { get; private set; }
		public string JsonPath { get; private set; }

		public void SetNewValue(string newValue)
		{
			throw new NotImplementedException();
		}
	}
}