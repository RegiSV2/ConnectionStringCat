using System;
using System.Collections.Generic;
using System.Linq;
using ConStringCat.Core.Model;
using ConStringCat.Core.UnitTests.Utils;
using ConStringCat.Core.VSInterop;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop
{
	[TestFixture]
	public class ConnectionStringVariantsSetTests
	{
		private const string VariantsSetName = "Some name";

		private const string ConnectionStringUpdateFailReason = "Some reason";

		private ConnectionStringVariantsSet _variantsSet;

		private void SelectLastVariant()
		{
			_variantsSet.SetCurrentVariant(_variantsSet.Variants.Last().Name);
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
		public void SetCurrentVariant_NewValueDiffersFromOld_ShouldCallAllUpdaters()
		{
			//Arrange
			var variantToSet = InitVariants();
			var updaters = InitUpdaters(variantToSet);

			//Act
			_variantsSet.SetCurrentVariant(variantToSet.Name);

			//Assert
			VerifyAllUpdatersCalled(updaters, variantToSet);
		}

		[Test]
		public void SetCurrentVariant_SomeOfUpdatersThrowExceptions_AllUpdatersShouldBeCalled()
		{
			//Arrange
			var variantToSet = InitVariants();
			var updaters = InitUpdatersWithPossibleFails(variantToSet);

			//Act
			try
			{
				_variantsSet.SetCurrentVariant(variantToSet.Name);
			}
			catch (Exception)
			{
				// ignored
			}

			//Assert
			VerifyAllUpdatersCalled(updaters, variantToSet);
		}

		[Test]
		public void SetCurrentVariant_SomeOfUpdatersThrowExceptions_ShouldThrowAggregateException()
		{
			//Arrange
			var variantToSet = InitVariants();
			InitUpdatersWithPossibleFails(variantToSet);

			//Assert
			try
			{
				_variantsSet.SetCurrentVariant(variantToSet.Name);
				Assert.Fail();
			}
			catch (AggregateException ex)
			{
				Assert.That(ex.InnerExceptions.Count == 2);
				Assert.That(ex.InnerExceptions.OfType<ConnectionStringUpdatingException>().Count() == 2);
			}
		}

		[Test]
		public void AddUpdater_UpdaterNotAdded_ShouldAddNewUpdaterToUpdaters()
		{
			var updater = new Mock<ConnectionStringUpdater>().Object;

			_variantsSet.AddUpdater(updater);

			Assert.That(_variantsSet.Updaters.Contains(updater));
		}

		[Test]
		public void AddUpdater_UpdaterAlreadyAdded_ShouldThrowExcpetion()
		{
			var updater = new Mock<ConnectionStringUpdater>().Object;

			_variantsSet.AddUpdater(updater);

			Assert.That(() => _variantsSet.AddUpdater(updater), Throws.Exception);
		}

		private ConnectionStringVariant InitVariants()
		{
			_variantsSet.AddVariant(VariantsCreator.Variant(0));
			_variantsSet.AddVariant(VariantsCreator.Variant(1));
			return _variantsSet.Variants.Last();
		}

		private List<Mock<ConnectionStringUpdater>> InitUpdaters(ConnectionStringVariant variantToSet)
		{
			var updaters = new List<Mock<ConnectionStringUpdater>>();
			for (var i = 0; i < 5; i++)
			{
				var updater = new Mock<ConnectionStringUpdater>();
				updater.Setup(x => x.SetNewValue(variantToSet.Value)).Verifiable();
				updaters.Add(updater);
				_variantsSet.AddUpdater(updater.Object);
			}
			return updaters;
		}

		private static void VerifyAllUpdatersCalled(List<Mock<ConnectionStringUpdater>> updaters, ConnectionStringVariant variantToSet)
		{
			foreach (var updater in updaters)
				updater.Verify(x => x.SetNewValue(variantToSet.Value), Times.Once);
		}

		private static void ThrowExceptionOnInvoke(Mock<ConnectionStringUpdater> updater)
		{
			updater.Setup(x => x.SetNewValue(It.IsAny<string>()))
				.Throws(new ConnectionStringUpdatingException(ConnectionStringUpdateFailReason));
		}

		private List<Mock<ConnectionStringUpdater>> InitUpdatersWithPossibleFails(ConnectionStringVariant variantToSet)
		{
			var updaters = InitUpdaters(variantToSet);
			ThrowExceptionOnInvoke(updaters.First());
			ThrowExceptionOnInvoke(updaters.Last());
			return updaters;
		}
	}
}
