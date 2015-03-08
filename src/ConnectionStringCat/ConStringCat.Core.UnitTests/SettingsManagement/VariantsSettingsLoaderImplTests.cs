using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConStringCat.Core.Model;
using ConStringCat.Core.SettingsManagement;
using ConStringCat.Core.UnitTests.Utils;
using ConStringCat.Core.ValueUpdating;
using ConStringCat.Core.VSInterop;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.SettingsManagement
{
	[TestFixture]
	public class VariantsSettingsLoaderImplTests
	{
		const string SolutionName = "TestSolutionFile.sln";

		const string ConnectionStringSettingsFileName = "TestSolutionFile" + VariantsSettingsLoaderImpl.SettingsFileSuffix;

		private const string EmbeddedValidSettingsFile = "ConStringCat.Core.UnitTests.SampleVariantSettings.json";

		private const string EmbeddedInvalidSettingsFile = "ConStringCat.Core.UnitTests.SampleInvalidVariantSettings.json";

		private const string EmbeddedInvalidJsonFile = "ConStringCat.Core.UnitTests.SampleInvalidJson.json";

		private VariantsSettingsLoaderImpl _loader;

		[SetUp]
		public void SetUp()
		{
			_loader = new VariantsSettingsLoaderImpl();
			File.Create(SolutionName);
		}

		[TearDown]
		public void TearDown()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			var filesToDelete = new[] {SolutionName, ConnectionStringSettingsFileName};
			foreach(var fileName in filesToDelete)
				if (File.Exists(fileName))
					File.Delete(fileName);
		}

		[Test]
		public void GetEmptyVariantsSet_ShouldReturnNullConnectionStringVariantsSet()
		{
			Assert.That(_loader.GetEmptyVariantsSet() == NullConnectionStringVariantsSet.Instance);
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		public void LoadVariantsSetForSolution_NullOrEmptyFileName_ShouldReturnNullVariantsSet(string solutionPath)
		{
			Assert.That(_loader.LoadVariantsSetForSolution(solutionPath) == NullConnectionStringVariantsSet.Instance);
		}

		[Test]
		public void LoadVariantsSetForSolution_SolutionFileDoesNotExist_ShouldReturnEmptySet()
		{
			//Arrange
			GC.Collect();
			GC.WaitForPendingFinalizers();
			File.Delete(SolutionName);
			CreateValidSettingsFile();

			//Assert
			Assert.That(_loader.LoadVariantsSetForSolution(SolutionName) == NullConnectionStringVariantsSet.Instance);
		}

		[Test]
		public void LoadVariantsSetForSolution_ValidSettingsFileExists_ShouldLoadSettings()
		{
			//Arrange
			CreateValidSettingsFile();

			//Act
			var variantsSet = (ConnectionStringVariantsSetImpl)_loader.LoadVariantsSetForSolution(SolutionName);

			//Assert
			Assert.That(variantsSet, Is.Not.Null);
			Assert.That(variantsSet.Name, Is.EqualTo("First variants set"));

			CollectionAssert.AreEquivalent(variantsSet.Variants,
				new Dictionary<string, string>
				{
					{"First item", "firstValueItem"},
					{"Second item", "secondValueItem"},
					{"third item", "thirdValueItem"}
				});
			Assert.That(variantsSet.Updaters.Count == 2);

			var xmlUpdater = variantsSet.Updaters.OfType<XmlFileConnectionStringUpdater>().First();
			Assert.That(xmlUpdater.DocumentPath, Is.EqualTo(Path.Combine(Directory.GetCurrentDirectory(), "./SomeFolder/../../someFile.xml")));
			Assert.That(xmlUpdater.XPath, Is.EqualTo("/catalog/book[@id='bk102']/price"));

			var jsonUpdater = variantsSet.Updaters.OfType<JsonFileConnectionStringUpdater>().First();
			Assert.That(jsonUpdater.DocumentPath, Is.EqualTo("R:/someJson.json"));
			Assert.That(jsonUpdater.JsonPath, Is.EqualTo("$.store.book[0].title"));
		}

		[Test]
		public void LoadVariantsSetForSolution_SettingsFileViolatesSchema_ShouldThrowException()
		{
			//Arrange
			CreateInvalidSettingsFile();

			//Assert
			AssertLoadingThrowsException();
		}

		[Test]
		public void LoadVariantsSetForSolution_SettingsFileHasNotJsonFormat_ShouldThrowException()
		{
			//Arrange
			CreateSettingsFileWithInvalidJson();

			//Assert
			AssertLoadingThrowsException();
		}

		[Test]
		public void LoadVariantsSetForSolution_SettingsFileDoesNotExist_ShouldReturnEmptySet()
		{
			//Act
			var variantsSet = (ConnectionStringVariantsSetImpl)_loader.LoadVariantsSetForSolution(SolutionName);

			//Assert
			AssertSetEmpty(variantsSet);
		}

		[Test]
		public void LoadVariantsSetForSolution_SettingsFileDoesNotExist_ShouldCreateDefaultFile()
		{
			Assert.That(!File.Exists(ConnectionStringSettingsFileName));
			var defaultSettings = _loader.LoadVariantsSetForSolution(SolutionName);

			Assert.That(File.Exists(ConnectionStringSettingsFileName));
			var newSettings = (ConnectionStringVariantsSetImpl)_loader.LoadVariantsSetForSolution(SolutionName);

			AssertSetEmpty(newSettings);
		}

		private void AssertLoadingThrowsException()
		{
			Assert.That(() => _loader.LoadVariantsSetForSolution(SolutionName),
				Throws.InstanceOf<VariantsSettingsLoadingException>());
		}

		private static void CreateValidSettingsFile()
		{
			EmbeddedResourceInterop.WriteEmbeddedResourceToFile(
				EmbeddedValidSettingsFile, ConnectionStringSettingsFileName);
		}

		private static void CreateInvalidSettingsFile()
		{
			EmbeddedResourceInterop.WriteEmbeddedResourceToFile(
				EmbeddedInvalidSettingsFile, ConnectionStringSettingsFileName);
		}

		private void CreateSettingsFileWithInvalidJson()
		{
			EmbeddedResourceInterop.WriteEmbeddedResourceToFile(
				EmbeddedInvalidJsonFile, ConnectionStringSettingsFileName);
		}

		private static void AssertSetEmpty(ConnectionStringVariantsSetImpl variantsSet)
		{
			Assert.That(!variantsSet.Variants.Any());
			Assert.That(!variantsSet.Updaters.Any());
		}
	}
}
