using System;
using ConStringCat.Core.Model;
using System.Diagnostics.Contracts;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace ConStringCat.Core.ValueUpdating
{
	/// <summary>
	///     Updates values in json file
	/// </summary>
	public sealed class JsonFileConfigurationValueUpdater : ConfigurationValueUpdater
	{
		private const string FileNotFoundMsg = "File \"{0}\" not found";
		private const string InvalidSelectionMsg = "Invalid json node selection specified";
		private const string NodesNotFoundMsg = "Couldn't find json nodes to update";

		public JsonFileConfigurationValueUpdater(string documentPath, string jsonPath)
		{
			Contract.Requires(!string.IsNullOrEmpty(documentPath));
			Contract.Requires(!string.IsNullOrEmpty(jsonPath));

			DocumentPath = documentPath;
			JsonPath = jsonPath;
		}

		public string DocumentPath { get; private set; }
		public string JsonPath { get; private set; }

		public void SetNewValue(string newValue)
		{
			try
			{
				TrySetValue(newValue);
			}
			catch (JsonException ex)
			{
				ThrowConfigurationValueUpdatingException(InvalidSelectionMsg, ex);
			}
			catch (FileNotFoundException ex)
			{
				ThrowConfigurationValueUpdatingException(string.Format(FileNotFoundMsg, DocumentPath), ex);
			}
		}

		private void TrySetValue(string newValue)
		{
			WorkWithJson(jObject => UpdateJsonNodes(jObject, JsonPath, newValue));

		}

		private void WorkWithJson(Action<JObject> workItem)
		{
			JObject jObject;
			using (var reader = new StreamReader(DocumentPath))
				jObject = JObject.Parse(reader.ReadToEnd());

			workItem(jObject);

			using (var writer = new StreamWriter(DocumentPath))
				writer.Write(jObject.ToString());
		}

		private void UpdateJsonNodes(JObject jObject, string jsonPath, string newValue)
		{
			var nodes = jObject.SelectTokens(jsonPath, false).ToList();
			if (nodes.Count == 0)
				ThrowConfigurationValueUpdatingException(NodesNotFoundMsg);

			Debug.Assert(nodes != null, "nodes != null");
			foreach (var node in nodes)
				((JProperty)node.Parent).Value = newValue;
		}

		private void ThrowConfigurationValueUpdatingException(string message)
		{
			throw new ConfigurationValueUpdatingException(message);
		}

		private void ThrowConfigurationValueUpdatingException(string message, Exception innerEx)
		{
			throw new ConfigurationValueUpdatingException(message, innerEx);
		}
	}
}