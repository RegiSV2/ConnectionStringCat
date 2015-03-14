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
		private Mock<ConfigurationAliasesEntity> _defaultAspect, _defaultAspect2, _modifiedAspect;
		private Mock<DTE> _dte;
		private Mock<VariantsSettingsLoader> _loader;
		private VariantsSetService _service;
		private Mock<Solution> _solution;

		private string LastRegisteredVariantAlias
		{
			get { return _defaultAspect.Object.Aliases.LastOrDefault(); }
		}

		[Test]
		public void GetAspects_SolutionNotLoaded_ShouldReturnEmptyList()
		{
			//Arrange
			_solution.SetupGet(x => x.IsOpen).Returns(false);
			//Act
			var aliases = _service.GetAspects();
			//Assert
			Assert.That(!aliases.Any());
		}

		[Test]
		public void GetAspects_SolutionChangedAfterLastCall_ShouldLoadAspectsForNewSolution()
		{
			//Arrange
			_service.GetAspects();
			_solution.Setup(x => x.FileName).Returns(ModifiedSolutionPath);

			//Act
			var aspects = _service.GetAspects();

			//Assert
			Assert.That(aspects != null && aspects.Any());
			_loader.Verify(x => x.LoadAspectsForSolution(ModifiedSolutionPath), Times.Once);
		}

		[Test]
		public void GetAspects_SolutionClosedAfterlastCall_ShouldReturnEmptyAspects()
		{
			//Arrange
			_service.GetAspects();
			_solution.Setup(x => x.IsOpen).Returns(false);

			//Act
			var aspects = _service.GetAspects();

			//Assert
			Assert.That(aspects != null && !aspects.Any());
		}

		[Test]
		public void GetSetCurrentAspect_NullArgument_ShouldNotChangeCurrentAspect()
		{
			//Act
			var aliasesBeforeCall = _service.GetAliases();
			_service.GetSetCurrentAspect(null);
			var aliasesAfterCall = _service.GetAliases();

			//Assert
			CollectionAssert.AreEquivalent(aliasesBeforeCall, aliasesAfterCall);
		}

		[Test]
		public void GetSetCurrentAspect_ShouldGetAndSetCurrentAspect()
		{
			//Act
			_service.GetSetCurrentAspect(_defaultAspect2.Object.Name);
			var currentAspectName = _service.GetSetCurrentAspect(null);

			//Assert
			Assert.AreEqual(currentAspectName, _defaultAspect2.Object.Name);
		}

		[Test]
		public void GetSetCurrentAspect_SolutionNotOpened_ShouldReturnNullAspect()
		{
			//Arrange
			_service.GetSetCurrentAspect(LastRegisteredVariantAlias);
			_solution.Setup(x => x.IsOpen).Returns(false);
			//Assert
			Assert.That(_service.GetSetCurrentAspect(null), Is.EqualTo(null));
		}

		[Test]
		public void GetVariantAliases_AliasesAdded_ShouldReturnAliasesFromSet()
		{
			//Act
			var aliases = _service.GetAliases();
			//Assert
			Assert.That(aliases, Is.EquivalentTo(_defaultAspect.Object.Aliases));
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
			_defaultAspect.Verify(x => x.SetCurrentVariant(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void GetSetCurrentVariant_NullArgument_ShouldReturnCurrentVariant()
		{
			//Arrange
			_defaultAspect.SetupGet(x => x.CurrentVariantAlias).Returns(LastRegisteredVariantAlias);
			//Assert
			Assert.That(_service.GetSetCurrentVariant(null), Is.EqualTo(LastRegisteredVariantAlias));
		}

		[Test]
		public void GetSetCurrentVariant_NotNullArgument_ShouldSetCurrentVariant()
		{
			//Arrange
			_service.GetSetCurrentVariant(LastRegisteredVariantAlias);
			//Assert
			_defaultAspect.Verify(x => x.SetCurrentVariant(LastRegisteredVariantAlias), Times.Once);
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
				() => new List<ConfigurationAliasesEntity> {_defaultAspect.Object, _defaultAspect2.Object});
			RegisterVariantsSetForPath(_loader, ModifiedSolutionPath,
				() => new List<ConfigurationAliasesEntity> {_modifiedAspect.Object});
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
			_defaultAspect = new Mock<ConfigurationAliasesEntity>();
			_defaultAspect.SetupGet(x => x.Name).Returns("EntityName");
			_defaultAspect2 = new Mock<ConfigurationAliasesEntity>();
			_defaultAspect2.SetupGet(x => x.Name).Returns("EntityName 2");
			_modifiedAspect = new Mock<ConfigurationAliasesEntity>();
			_modifiedAspect.SetupGet(x => x.Name).Returns("ModifiedName");
			foreach (var idx in Enumerable.Range(0, variantsCount))
			{
				VariantsCreator.AddVariant(_defaultAspect, idx);
				VariantsCreator.AddVariant(_modifiedAspect, idx + variantsCount);
				VariantsCreator.AddVariant(_defaultAspect2, idx + variantsCount * 2);
			}
		}

		#endregion
	}
}