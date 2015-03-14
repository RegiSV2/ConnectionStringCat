using System;
using System.Collections.Generic;
using System.Linq;
using ConStringCat.Core.Model;
using ConStringCat.Core.UnitTests.Utils;
using ConStringCat.Core.VSInterop;
using EnvDTE;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop
{
	[TestFixture]
	public class VariantsSetServiceTests
	{
		private const string DefaultSolutionPath = "DefaultSolution.sln";
		private const string ModifiedSolutionPath = "ModifiedSolution.sln";
		private Mock<ConfigurationAliasesEntity> _defaultSettings, _modifiedSettings;
		private Mock<DTE> _dte;
		private Mock<VariantsSettingsLoader> _loader;
		private VariantsSetService _service;
		private Mock<Solution> _solution;

		private string LastRegisteredVariantAlias
		{
			get { return _defaultSettings.Object.Aliases.LastOrDefault(); }
		}

		[Test]
		public void GetVariantAliases_AliasesAdded_ShouldReturnAliasesFromSet()
		{
			//Act
			var aliases = _service.GetAliases();
			//Assert
			Assert.That(aliases, Is.EquivalentTo(_defaultSettings.Object.Aliases));
		}

		[Test]
		public void GetVariantAliases_AliasesAddedButSolutionNotLoaded_ShouldReturnEmptyList()
		{
			//Arrange
			_solution.SetupGet(x => x.IsOpen).Returns(false);
			//Act
			var aliases = _service.GetAliases();
			//Assert
			Assert.That(aliases, Is.EquivalentTo(Enumerable.Empty<string>()));
		}

		[Test]
		public void GetVariantAliases_SolutionChangedAfterLastCall_ShouldLoadVariantsSetForNewSolution()
		{
			//Arrange
			_service.GetAliases();
			_solution.Setup(x => x.FileName).Returns(ModifiedSolutionPath);

			//Act
			var aliases = _service.GetAliases();

			//Assert
			Assert.That(aliases != null && aliases.Any());
			_loader.Verify(x => x.LoadAspectsForSolution(ModifiedSolutionPath), Times.Once);
		}

		[Test]
		public void GetVariantAliases_SolutionClosedAfterlastCall_ShouldFreeVariants()
		{
			//Arrange
			_service.GetAliases();
			_solution.Setup(x => x.IsOpen).Returns(false);

			//Act
			var aliases = _service.GetAliases();

			//Assert
			Assert.That(aliases != null && !aliases.Any());
		}

		[Test]
		public void GetSetCurrentVariant_NullArgument_ShouldNotChangeCurrentVariant()
		{
			//Act
			_service.GetSetCurrentVariant(null);
			//Assert
			_defaultSettings.Verify(x => x.SetCurrentVariant(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void GetSetCurrentVariant_NullArgument_ShouldReturnCurrentVariant()
		{
			//Arrange
			_defaultSettings.SetupGet(x => x.CurrentVariantAlias).Returns(LastRegisteredVariantAlias);
			//Assert
			Assert.That(_service.GetSetCurrentVariant(null), Is.EqualTo(LastRegisteredVariantAlias));
		}

		[Test]
		public void GetSetCurrentVariant_NotNullArgument_ShouldSetCurrentVariant()
		{
			//Arrange
			_service.GetSetCurrentVariant(LastRegisteredVariantAlias);
			//Assert
			_defaultSettings.Verify(x => x.SetCurrentVariant(LastRegisteredVariantAlias), Times.Once);
		}

		[Test]
		public void GetSetCurrentVariant_SolutionNotOpened_ShouldReturnNullVariant()
		{
			//Arrange
			_service.GetSetCurrentVariant(LastRegisteredVariantAlias);
			_solution.Setup(x => x.IsOpen).Returns(false);
			//Assert
			Assert.That(_service.GetSetCurrentVariant(null), Is.EqualTo(null));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void IsServiceAvailable_ShouldBeAvailableOnlyWhenSolutionIsOpen(bool isSolutionOpen)
		{
			//Arrange
			_solution.Setup(x => x.IsOpen).Returns(isSolutionOpen);

			//Assert
			Assert.That(_service.IsServiceAvailable == _solution.Object.IsOpen);
		}

		#region Initialization

		[SetUp]
		public void InitializeContext()
		{
			CreateMocks();
			CreateSettings();

			_service = new VariantsSetServiceImpl(_dte.Object, _loader.Object);
		}

		private void CreateMocks()
		{
			_dte = new Mock<DTE>();
			_solution = new Mock<Solution>();
			_solution.Setup(x => x.IsOpen).Returns(true);
			_solution.Setup(x => x.FileName).Returns(DefaultSolutionPath);

			_dte.Setup(x => x.Solution)
				.Returns(_solution.Object);

			_loader = new Mock<VariantsSettingsLoader>();
			_loader.Setup(x => x.GetEmptyAspect())
				.Returns(NullConfigurationAliasesEntity.Instance);
			RegisterVariantsSetForPath(_loader, DefaultSolutionPath,
				() => new List<ConfigurationAliasesEntity> {_defaultSettings.Object});
			RegisterVariantsSetForPath(_loader, ModifiedSolutionPath,
				() => new List<ConfigurationAliasesEntity> {_modifiedSettings.Object});
		}

		private void RegisterVariantsSetForPath(Mock<VariantsSettingsLoader> loaderMock,
			string solutionPath, Func<IList<ConfigurationAliasesEntity>> set)
		{
			loaderMock.Setup(x => x.LoadAspectsForSolution(solutionPath))
				.Returns(set)
				.Verifiable();
		}

		private void CreateSettings()
		{
			const int variantsCount = 3;
			_defaultSettings = new Mock<ConfigurationAliasesEntity>();
			_defaultSettings.SetupGet(x => x.Name).Returns("EntityName");
			_modifiedSettings = new Mock<ConfigurationAliasesEntity>();
			_modifiedSettings.SetupGet(x => x.Name).Returns("ModifiedName");
			foreach (var idx in Enumerable.Range(0, variantsCount))
			{
				VariantsCreator.AddVariant(_defaultSettings, idx);
				VariantsCreator.AddVariant(_modifiedSettings, idx + variantsCount);
			}
		}

		#endregion
	}
}