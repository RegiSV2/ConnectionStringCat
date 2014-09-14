﻿using System.Linq;
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

		private ConnectionStringVariantsSet _variantsSet;

		private Mock<DTE> _dte;

		private Mock<Solution> _solution;

		private ConnectionStringVariant LastRegisteredVariant()
		{
			return _variantsSet.Variants.Last();
		}

		#region Initialization

		[SetUp]
		public void InitializeContext()
		{
			CreateMocks();

			_service = new VariantsSetServiceImpl(_dte.Object);
			_variantsSet = new ConnectionStringVariantsSet("SetName");
			foreach (var idx in Enumerable.Range(0, 3))
				_variantsSet.AddVariant(VariantsCreator.Variant(idx));
			_service.SetVariantsSet(_variantsSet);
		}

		private void CreateMocks()
		{
			_dte = new Mock<DTE>();
			_solution = new Mock<Solution>();
			_solution.Setup(x => x.IsOpen).Returns(true);
			_dte.Setup(x => x.Solution)
				.Returns(_solution.Object);
		}

		#endregion

		[Test]
		public void GetVariantAliases_AliasesAdded_ShouldReturnAliasesFromSet()
		{
			//Act
			var aliases = _service.GetAliases();
			//Assert
			Assert.That(aliases, Is.EquivalentTo(_variantsSet.Aliases));
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
		public void GetSetCurrentVariant_NullArgument_ShouldNotChangeCurrentVariant()
		{
			//Act
			_service.GetSetCurrentVariant(null);
			//Assert
			Assert.That(_variantsSet.CurrentVariant, Is.EqualTo(_variantsSet.Variants.First()));
		}

		[Test]
		public void GetSetCurrentVariant_NullArgument_ShouldRetunrCurrentVariant()
		{
			//Arrange
			var currentVariantAlias = LastRegisteredVariant().Alias;
			_variantsSet.SetCurrentVariant(currentVariantAlias);
			//Assert
			Assert.That(_service.GetSetCurrentVariant(null), Is.EqualTo(currentVariantAlias));
		}

		[Test]
		public void GetSetCurrentVariant_NotNullArgument_ShouldSetCurrentVariant()
		{
			//Arrange
			var selectedVariant = LastRegisteredVariant();
			_service.GetSetCurrentVariant(selectedVariant.Alias);
			//Assert
			Assert.That(_variantsSet.CurrentVariant, Is.EqualTo(selectedVariant));
		}

		[Test]
		public void GetSetCurrentVariant_SolutionNotOpened_ShouldReturnNullVariant()
		{
			//Arrange
			_service.GetSetCurrentVariant(LastRegisteredVariant().Alias);
			_solution.Setup(x => x.IsOpen).Returns(false);
			//Assert
			Assert.That(_service.GetSetCurrentVariant(null), Is.EqualTo(null));
		}
	}
}