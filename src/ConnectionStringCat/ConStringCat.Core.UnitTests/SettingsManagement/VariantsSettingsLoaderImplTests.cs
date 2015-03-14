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
		private const string SolutionName = "TestSolutionFile.sln";

		private const string ConnectionStringSettingsFileName =
			"TestSolutionFile" + VariantsSettingsLoaderImpl.SettingsFileSuffix;

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
			foreach (var fileName in filesToDelete)
				if (File.Exists(fileName))
					File.Delete(fileName);
		}

		[Test]
		public void GetEmptyAspect_ShouldReturnNullConfigurationAliasesEntity()
		{
			Assert.That(_loader.GetEmptyAspect() == NullConfigurationAliasesEntity.Instance);
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		public void LoadVariantsSetForSolution_NullOrEmptyFileName_ShouldReturnEmptyList(string solutionPath)
		{
			Assert.That(!_loader.LoadAspectsForSolution(solutionPath).Any());
		}

		[Test]
		public void LoadVariantsSetForSolution_SolutionFileDoesNotExist_ShouldReturnEmptyList()
		{
			//Arrange
			GC.Collect();
			GC.WaitForPendingFinalizers();
			File.Delete(SolutionName);
			CreateValidSettingsFile();

			//Assert
			Assert.That(!_loader.LoadAspectsForSolution(SolutionName).Any());
		}

		[Test]
		public void LoadVariantsSetForSolution_ValidSettingsFileExists_ShouldLoadSettings()
		{
			//Arrange
			CreateValidSettingsFile();

			//Act
			var aspects = _loader.LoadAspectsForSolution(SolutionName)
				.Cast<ConfigurationAspect>()
				.ToList();

			//Assert
			var firstAspect = new ConfigurationAspect("DB Connection string");
			firstAspect.AddAlias("First string");
			firstAspect.AddAlias("Second string");
			firstAspect.AddAlias("Third string");
			var conStrSet = new ConnectionStringVariantsSetImpl("Connection string");
			conStrSet.AddVariant("First string", "connection string 1");
			conStrSet.AddVariant("Second string", "connection string 2");
			conStrSet.AddVariant("Third string", "connection string 3");
			conStrSet.AddUpdater(new XmlFileConnectionStringUpdater(ToAbsolutePath("./SomeFolder/../../someFile.xml"),
				"/catalog/book[@id='bk102']/price"));
			conStrSet.AddUpdater(new JsonFileConnectionStringUpdater("R:/someJson.json", "$.store.book[0].title"));
			firstAspect.AddVariantsSet(conStrSet);
			var driverSet = new ConnectionStringVariantsSetImpl("Driver");
			driverSet.AddVariant("First string", "driver 1");
			driverSet.AddVariant("Second string", "driver 2");
			driverSet.AddVariant("Third string", "driver 3");
			driverSet.AddUpdater(new XmlFileConnectionStringUpdater(ToAbsolutePath("./SomeFolder/../../someFile.xml"),
				"/catalog/book[@id='bk102']/author"));
			driverSet.AddUpdater(new JsonFileConnectionStringUpdater("R:/someJson.json", "$.store.book[0].author"));
			firstAspect.AddVariantsSet(driverSet);

			var secondAspect = new ConfigurationAspect("Web service address");
			secondAspect.AddAlias("First string");
			secondAspect.AddAlias("Second string");
			secondAspect.AddAlias("Third string");
			var defaultAddrSet = new ConnectionStringVariantsSetImpl("Default set");
			defaultAddrSet.AddVariant("First string", "address 1");
			defaultAddrSet.AddVariant("Second string", "address 2");
			defaultAddrSet.AddVariant("Third string", "address 3");
			defaultAddrSet.AddUpdater(new XmlFileConnectionStringUpdater(ToAbsolutePath("./SomeFolder/../../someFile.xml"),
				"/catalog/book[@id='bk102']/genre"));
			defaultAddrSet.AddUpdater(new JsonFileConnectionStringUpdater("R:/someJson.json", "$.store.book[0].genre"));
			secondAspect.AddVariantsSet(defaultAddrSet);

			CollectionAssert.IsNotEmpty(aspects);
			aspects.ForEach(AssertAspectInDefaultState);
			AssertAspectsEqual(firstAspect, aspects[0]);
			AssertAspectsEqual(secondAspect, aspects[1]);
		}

		private string ToAbsolutePath(string path)
		{
			return Path.Combine(Directory.GetCurrentDirectory(), path);
		}

		private void AssertAspectInDefaultState(ConfigurationAspect aspect)
		{
			Assert.That(aspect.CurrentVariantAlias == aspect.Aliases.First());
			foreach (var set in aspect.Sets)
				Assert.That(set.CurrentVariantAlias == aspect.CurrentVariantAlias);
		}

		private void AssertAspectsEqual(ConfigurationAspect expected, ConfigurationAspect actual)
		{
			AssertConfigurationEntitiesEqual(expected, actual);
			Assert.That(expected.Sets.Count == actual.Sets.Count);
			foreach (var set in actual.Sets.Cast<ConnectionStringVariantsSetImpl>())
			{
				var set1 = set;
				var actualSet = expected.Sets.Cast<ConnectionStringVariantsSetImpl>().FirstOrDefault(x => x.Name == set1.Name);
				AssertSetsEqual(set1, actualSet);
			}
		}

		private void AssertSetsEqual(ConnectionStringVariantsSetImpl expected, ConnectionStringVariantsSetImpl actual)
		{
			AssertConfigurationEntitiesEqual(expected, actual);
			Assert.That(expected.Updaters.Count == actual.Updaters.Count);
			foreach (var updater in actual.Updaters)
				AssertUpdatersContainEqual(expected.Updaters, updater);
		}

		private void AssertUpdatersContainEqual(IReadOnlyList<ConnectionStringUpdater> updaters,
			ConnectionStringUpdater updater)
		{
			if (updater is JsonFileConnectionStringUpdater)
			{
				AssertContainsUpdater(updaters, (JsonFileConnectionStringUpdater) updater,
					(a, b) => a.DocumentPath == b.DocumentPath && a.JsonPath == b.JsonPath);
			}
			else if (updater is XmlFileConnectionStringUpdater)
			{
				AssertContainsUpdater(updaters, (XmlFileConnectionStringUpdater) updater,
					(a, b) => a.DocumentPath == b.DocumentPath && a.XPath == b.XPath);
			}
		}

		private void AssertContainsUpdater<TUpdater>(IEnumerable<ConnectionStringUpdater> updaters,
			TUpdater updater, Func<TUpdater, TUpdater, bool> comparer)
		{
			Assert.That(updaters.OfType<TUpdater>().Count(x => comparer(x, updater)) > 0);
		}

		private void AssertConfigurationEntitiesEqual(ConfigurationAliasesEntity expected, ConfigurationAliasesEntity actual)
		{
			Assert.That(expected.Name == actual.Name);
			CollectionAssert.AreEquivalent(expected.Aliases, actual.Aliases);
		}

		[Test]
		public void LoadAspectsForSolution_SettingsFileViolatesSchema_ShouldThrowException()
		{
			//Arrange
			CreateInvalidSettingsFile();

			//Assert
			AssertLoadingThrowsException();
		}

		[Test]
		public void LoadAspectsForSolution_SettingsFileHasNotJsonFormat_ShouldThrowException()
		{
			//Arrange
			CreateSettingsFileWithInvalidJson();

			//Assert
			AssertLoadingThrowsException();
		}

		[Test]
		public void LoadAspectsForSolution_SettingsFileDoesNotExist_ShouldReturnEmptyList()
		{
			//Act
			var aspects = _loader.LoadAspectsForSolution(SolutionName);

			//Assert
			CollectionAssert.IsEmpty(aspects);
		}

		[Test]
		public void LoadAspectsForSolution_SettingsFileDoesNotExist_ShouldCreateDefaultFile()
		{
			Assert.That(!File.Exists(ConnectionStringSettingsFileName));
			_loader.LoadAspectsForSolution(SolutionName);

			Assert.That(File.Exists(ConnectionStringSettingsFileName));
			var newSettings = _loader.LoadAspectsForSolution(SolutionName);

			CollectionAssert.IsEmpty(newSettings);
		}

		private void AssertLoadingThrowsException()
		{
			Assert.That(() => _loader.LoadAspectsForSolution(SolutionName),
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
	}
}