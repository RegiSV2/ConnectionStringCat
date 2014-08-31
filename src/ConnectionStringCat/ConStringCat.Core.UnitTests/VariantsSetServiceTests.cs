using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConStringCat.Core.Model;
using ConStringCat.Core.UnitTests.Utils;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests
{
	[TestFixture]
	public class VariantsSetServiceTests
	{
		private VariantsSetService _service;

		private ConnectionStringVariantsSet _variantsSet;

		[SetUp]
		public void InitializeContext()
		{
			_service = new VariantsSetServiceImpl();
			_variantsSet = new ConnectionStringVariantsSet("SetName");
			foreach (var idx in Enumerable.Range(0, 3))
				_variantsSet.AddVariant(VariantsCreator.Variant(idx));
			_service.SetVariantsSet(_variantsSet);
		}

		[Test]
		public void GetVariantAliases_AliasesAdded_ShouldReturnAliasesFromSet()
		{
			//Act
			var aliases = _service.GetAliases();
			//Assert
			Assert.That(aliases, Is.EquivalentTo(_variantsSet.Aliases));
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
			var currentVariantAlias = _variantsSet.Variants.Last().Alias;
			_variantsSet.SetCurrentVariant(currentVariantAlias);
			//Assert
			Assert.That(_service.GetSetCurrentVariant(null), Is.EqualTo(currentVariantAlias));
		}

		[Test]
		public void GetSetCurrentVariant_NotNullArgument_ShouldSetCurrentVariant()
		{
			//Arrange
			var selectedVariant = _variantsSet.Variants.Last();
			_service.GetSetCurrentVariant(selectedVariant.Alias);
			//Assert
			Assert.That(_variantsSet.CurrentVariant, Is.EqualTo(selectedVariant));
		}
	}
}
