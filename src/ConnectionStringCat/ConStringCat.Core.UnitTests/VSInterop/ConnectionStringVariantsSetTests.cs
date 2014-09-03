using System;
using System.Linq;
using ConStringCat.Core.Model;
using ConStringCat.Core.UnitTests.Utils;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop
{
	[TestFixture]
	public class ConnectionStringVariantsSetTests
	{
		private const string VariantsSetName = "Some name";

		private ConnectionStringVariantsSet _variantsSet;

		private void SelectLastVariant()
		{
			_variantsSet.SetCurrentVariant(_variantsSet.Variants.Last().Alias);
		}

		[SetUp]
		public void InitializeContext()
		{
			_variantsSet = new ConnectionStringVariantsSet(VariantsSetName);
		}

		[Test]
		public void AddVariant_NotNullVariant_ShouldAddNewVariant()
		{
			//Arrange
			var addedVariant = VariantsCreator.Variant(0);
			//Act
			_variantsSet.AddVariant(addedVariant);
			//Assert
			Assert.That(_variantsSet.Variants.Count, Is.EqualTo(1));
			Assert.That(_variantsSet.Variants[0] == addedVariant);
		}

		[Test]
		public void AddVariant_NullVariant_ShouldThrowException()
		{
			Assert.That(() => _variantsSet.AddVariant(null), Throws.Exception);
		}

		[Test]
		public void Aliases_ShouldReturnVariantsAliases()
		{
			//Arrange
			foreach (var idx in Enumerable.Range(0, 3))
				_variantsSet.AddVariant(VariantsCreator.Variant(idx));
			//Act
			var aliases = _variantsSet.Aliases;
			//Assert
			foreach (var idx in Enumerable.Range(0, 3))
				Assert.That(aliases[idx] == VariantsCreator.VariantAlias(idx));
		}

		[Test]
		public void CurrentVariant_NoVariantsAdded_ShouldReturnNullVariant()
		{
			Assert.That(_variantsSet.CurrentVariant, Is.EqualTo(ConnectionStringVariant.Null));
		}

		[Test]
		public void CurrentVariant_SomeVariantsAdded_ShouldReturnFirstAddedVariant()
		{
			//Arrange
			foreach (var idx in Enumerable.Range(0, 3))
				_variantsSet.AddVariant(VariantsCreator.Variant(idx));
			//Assert
			Assert.That(_variantsSet.CurrentVariant, Is.EqualTo(_variantsSet.Variants.First()));
		}

		[Test]
		public void SetCurrentVariant_SetOwnedVariant_ShouldSetVariantProperly()
		{
			//Arrange
			_variantsSet.AddVariant(VariantsCreator.Variant(0));
			_variantsSet.AddVariant(VariantsCreator.Variant(1));
			//Act
			SelectLastVariant();
			//Assert
			Assert.That(_variantsSet.CurrentVariant == _variantsSet.Variants.Last());
		}

		[Test]
		public void SetCurrentVariant_SetNotOwnedVariant_ShouldThrowArgumentException()
		{
			Assert.That(() => _variantsSet.SetCurrentVariant(VariantsCreator.VariantAlias(0)),
				Throws.InstanceOf<InvalidOperationException>());
		}

		[Test]
		public void SetCurrentVariant_NewValueDiffersFromOld_ShouldFireVariantChangedEvent()
		{
			//Arrange
			var eventFired = false;
			_variantsSet.VariantChangedEvent += (s, args) => { eventFired = true; };
			_variantsSet.AddVariant(VariantsCreator.Variant(0));
			_variantsSet.AddVariant(VariantsCreator.Variant(1));
			//Act
			_variantsSet.SetCurrentVariant(_variantsSet.Variants.Last().Alias);
			//Assert
			Assert.That(eventFired, Is.EqualTo(eventFired));
		}

		[Test]
		public void SetCurrentVariant_EventFired_ShouldPassVariantSetToHandler()
		{
			//Arrange
			_variantsSet.AddVariant(VariantsCreator.Variant(0));
			_variantsSet.AddVariant(VariantsCreator.Variant(1));
			_variantsSet.VariantChangedEvent += (s, args) => Assert.That(s, Is.EqualTo(_variantsSet));
			//Act
			_variantsSet.SetCurrentVariant(_variantsSet.Variants.Last().Alias);
		}
	}
}
