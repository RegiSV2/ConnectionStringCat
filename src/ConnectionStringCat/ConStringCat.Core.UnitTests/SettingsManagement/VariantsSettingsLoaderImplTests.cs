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
using Moq;

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
		private Mock<ConfigurationValueUpdaterFactory> _updaterFactory;

		[SetUp]
		public void SetUp()
		{
			_updaterFactory = new Mock<ConfigurationValueUpdaterFactory>();
			_updaterFactory.Setup(x=> x.CreateXmlUpdater(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(new Mock<ConfigurationValueUpdater>().Object).Verifiable();
			_updaterFactory.Setup(x=> x.CreateJsonUpdater(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(new Mock<ConfigurationValueUpdater>().Object).Verifiable();

			_loader = new VariantsSettingsLoaderImpl(_updaterFactory.Object);
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
			var conStrSet = new ConfigurationValueVariantsSet("Connection string");
			conStrSet.AddVariant("First string", "connection string 1");
			conStrSet.AddVariant("Second string", "connection string 2");
			conStrSet.AddVariant("Third string", "connection string 3");
			conStrSet.AddUpdater(new XmlFileConfigurationValueUpdater(ToAbsolutePath("./SomeFolder/../../someFile.xml"),
				"/catalog/book[@id='bk102']/price"));
			conStrSet.AddUpdater(new JsonFileConfigurationValueUpdater("R:/someJson.json", "$.store.book[0].title"));
			
			firstAspect.AddVariantsSet(conStrSet);
			var driverSet = new ConfigurationValueVariantsSet("Driver");
			driverSet.AddVariant("First string", "driver 1");
			driverSet.AddVariant("Second string", "driver 2");
			driverSet.AddVariant("Third string", "driver 3");
			driverSet.AddUpdater(new XmlFileConfigurationValueUpdater(ToAbsolutePath("./SomeFolder/../../someFile.xml"),
				"/catalog/book[@id='bk102']/author"));
			driverSet.AddUpdater(new JsonFileConfigurationValueUpdater("R:/someJson.json", "$.store.book[0].author"));
			firstAspect.AddVariantsSet(driverSet);

			var secondAspect = new ConfigurationAspect("Web service address");
			secondAspect.AddAlias("First string");
			secondAspect.AddAlias("Second string");
			secondAspect.AddAlias("Third string");
			var defaultAddrSet = new ConfigurationValueVariantsSet("Default set");
			defaultAddrSet.AddVariant("First string", "address 1");
			defaultAddrSet.AddVariant("Second string", "address 2");
			defaultAddrSet.AddVariant("Third string", "address 3");
			defaultAddrSet.AddUpdater(new XmlFileConfigurationValueUpdater(ToAbsolutePath("./SomeFolder/../../someFile.xml"),
				"/catalog/book[@id='bk102']/genre"));
			defaultAddrSet.AddUpdater(new JsonFileConfigurationValueUpdater("R:/someJson.json", "$.store.book[0].genre"));
			secondAspect.AddVariantsSet(defaultAddrSet);

			CollectionAssert.IsNotEmpty(aspects);
			aspects.ForEach(AssertAspectInDefaultState);
			AssertAspectsEqual(firstAspect, aspects[0]);
			AssertAspectsEqual(secondAspect, aspects[1]);
			AssertUpdatersCreated(conStrSet.Updaters.Union(driverSet.Updaters).Union(defaultAddrSet.Updaters).ToList());
		}

		private string ToAbsolutePath(string path)
		{
			return Path.Combine(Path.GetDirectoryName(SolutionName), path);
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
			foreach (var set in actual.Sets.Cast<ConfigurationValueVariantsSet>())
			{
				var set1 = set;
				var actualSet = expected.Sets.Cast<ConfigurationValueVariantsSet>().FirstOrDefault(x => x.Name == set1.Name);
				AssertSetsEqual(set1, actualSet);
			}
		}

		private void AssertSetsEqual(ConfigurationValueVariantsSet expected, ConfigurationValueVariantsSet actual)
		{
			AssertConfigurationEntitiesEqual(expected, actual);
			Assert.That(expected.Updaters.Count == actual.Updaters.Count);
		}

		private void AssertContainsUpdater<TUpdater>(IEnumerable<ConfigurationValueUpdater> updaters,
			TUpdater updater, Func<TUpdater, TUpdater, bool> comparer)
		{
			Assert.That(updaters.OfType<TUpdater>().Count(x => comparer(x, updater)) > 0);
		}

		private void AssertConfigurationEntitiesEqual(ConfigurationAliasesEntity expected, ConfigurationAliasesEntity actual)
		{
			Assert.That(expected.Name == actual.Name);
			CollectionAssert.AreEquivalent(expected.Aliases, actual.Aliases);
		}

		private void AssertUpdatersCreated(IList<ConfigurationValueUpdater> updaters)
		{
			foreach (var updater in updaters.OfType<JsonFileConfigurationValueUpdater>())
				_updaterFactory.Verify(x => x.CreateJsonUpdater(updater.DocumentPath, updater.JsonPath), Times.Once);
			foreach (var updater in updaters.OfType<XmlFileConfigurationValueUpdater>())
				_updaterFactory.Verify(x => x.CreateXmlUpdater(updater.DocumentPath, updater.XPath), Times.Once);
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