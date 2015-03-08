using System;
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
		private VariantsSetService _service;

		private ConnectionStringVariantsSetImpl _defaultVariantsSet, _modifiedVariantsSet;

		private Mock<DTE> _dte;

		private Mock<Solution> _solution;

		private Mock<VariantsSettingsLoader> _loader;

		private const string DefaultSolutionPath = "DefaultSolution.sln";

		private const string ModifiedSolutionPath = "ModifiedSolution.sln";

		private string LastRegisteredVariantAlias
		{
			get { return _defaultVariantsSet.Variants.Last().Key; }
		}

		#region Initialization

		[SetUp]
		public void InitializeContext()
		{
			CreateMocks();
			CreateVariants();

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
			_loader.Setup(x => x.GetEmptyVariantsSet())
				.Returns(NullConnectionStringVariantsSet.Instance);
			RegisterVariantsSetForPath(_loader, DefaultSolutionPath, () => _defaultVariantsSet);
			RegisterVariantsSetForPath(_loader, ModifiedSolutionPath, () => _modifiedVariantsSet);
		}

		private void RegisterVariantsSetForPath(Mock<VariantsSettingsLoader> loaderMock, 
			string solutionPath, Func<ConnectionStringVariantsSetImpl> set)
		{
			loaderMock.Setup(x => x.LoadVariantsSetForSolution(solutionPath))
				.Returns(set)
				.Verifiable();
		}

		private void CreateVariants()
		{
			const int variantsCount = 3;
			_defaultVariantsSet = new ConnectionStringVariantsSetImpl("SetName");
			_modifiedVariantsSet = new ConnectionStringVariantsSetImpl("ModifiedSetName");
			foreach (var idx in Enumerable.Range(0, variantsCount))
			{
				VariantsCreator.AddVariant(_defaultVariantsSet, idx);
				VariantsCreator.AddVariant(_modifiedVariantsSet, idx + variantsCount);
			}
		}

		#endregion

		[Test]
		public void GetVariantAliases_AliasesAdded_ShouldReturnAliasesFromSet()
		{
			//Act
			var aliases = _service.GetAliases();
			//Assert
			Assert.That(aliases, Is.EquivalentTo(_defaultVariantsSet.Aliases));
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
			_loader.Verify(x => x.LoadVariantsSetForSolution(ModifiedSolutionPath), Times.Once);
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
			Assert.That(_defaultVariantsSet.CurrentVariantAlias, Is.EqualTo(_defaultVariantsSet.Variants.First().Key));
		}

		[Test]
		public void GetSetCurrentVariant_NullArgument_ShouldRetunrCurrentVariant()
		{
			//Arrange
			_defaultVariantsSet.SetCurrentVariant(LastRegisteredVariantAlias);
			//Assert
			Assert.That(_service.GetSetCurrentVariant(null), Is.EqualTo(LastRegisteredVariantAlias));
		}

		[Test]
		public void GetSetCurrentVariant_NotNullArgument_ShouldSetCurrentVariant()
		{
			//Arrange
			_service.GetSetCurrentVariant(LastRegisteredVariantAlias);
			//Assert
			Assert.That(_defaultVariantsSet.CurrentVariantAlias, Is.EqualTo(LastRegisteredVariantAlias));
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
	}
}
