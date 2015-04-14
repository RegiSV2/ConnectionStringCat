using ConStringCat.Core.Model;
using ConStringCat.Core.UnitTests.Utils;
using ConStringCat.Core.ValueUpdating;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConStringCat.Core.UnitTests.ValueUpdating
{
	[TestFixture]
	public class JsonFileConfigurationValueUpdaterTests
	{
		private const string TestDocumentPath = "jsonfile.json";
		private const string ValidJsonPath = "$.catalog";
		private const string ValidDocumentPath = "someFile.xml";

		[SetUp]
		public void SetUp()
		{
			EmbeddedResourceInterop.WriteEmbeddedResourceToFile(
				"ConStringCat.Core.UnitTests.TestJsonFile.json", TestDocumentPath);
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(TestDocumentPath))
				File.Delete(TestDocumentPath);
		}

		[Test]
		public void Create_InvalidConstructorParameters_ShouldFail()
		{
			Assert.That(() => new JsonFileConfigurationValueUpdater(null, ValidJsonPath),
				Throws.Exception);
			Assert.That(() => new JsonFileConfigurationValueUpdater(ValidDocumentPath, null),
				Throws.Exception);
		}

		[Test]
		public void SetNewValue_PathToNode_ShouldUpdateDocument()
		{
			var jsonPath = "$.catalog[?(@.id=='bk102')].price";
			SetNewValue_TestWithPath(jsonPath);
		}

		[Test]
		public void SetNewValue_PathToMultipleNodes_ShouldUpdateDocument()
		{
			var jsonPath = "$.catalog[*].price";
			SetNewValue_TestWithPath(jsonPath);
		}

		[Test]
		public void SetNewValue_DocumentDoesNotExist_ShouldThrowConnectionStringUpdatingException()
		{
			File.Delete(TestDocumentPath);
			var updater = CreateUpdater(TestDocumentPath, ValidJsonPath);

			AssertConfigurationValueUpdatingExceptionThrown(updater);
		}

		[Test]
		public void SetNewValue_InvalidJsonPath_ShouldThrowConnectionStringUpdatingException()
		{
			var updater = CreateUpdater(TestDocumentPath, "some invalid jsonPath");

			AssertConfigurationValueUpdatingExceptionThrown(updater);
		}

		[Test]
		public void SetNewValue_PathToZeroNodes_ShouldThrowConnectionStringUpdatingException()
		{
			var updater = CreateUpdater(TestDocumentPath, "/nonExistingRoute");

			AssertConfigurationValueUpdatingExceptionThrown(updater);
		}

		private void SetNewValue_TestWithPath(string jsonPath)
		{
			var updater = CreateUpdater(TestDocumentPath, jsonPath);

			var newValue = "some new value";
			updater.SetNewValue(newValue);

			JObject jobject;
			using (var reader = new StreamReader(TestDocumentPath))
				jobject = JObject.Parse(reader.ReadToEnd());
			var nodes = jobject.SelectTokens(jsonPath, false).ToList();

			Assert.That(nodes != null && nodes.Count > 0);
			foreach (var node in nodes)
			{
				Assert.That(node.Value<string>() == newValue);
			}
		}

		private void AssertConfigurationValueUpdatingExceptionThrown(JsonFileConfigurationValueUpdater updater)
		{
			Assert.That(() => updater.SetNewValue("some value"), Throws.InstanceOf<ConfigurationValueUpdatingException>());
		}

		private JsonFileConfigurationValueUpdater CreateUpdater(string filePath, string xPath)
		{
			return new JsonFileConfigurationValueUpdater(filePath, xPath);
		}
	}
}
