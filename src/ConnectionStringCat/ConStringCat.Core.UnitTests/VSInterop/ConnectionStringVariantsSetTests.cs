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

		private ConnectionStringVariantsSetImpl _variantsSet;

		private void SelectLastVariant()
		{
			_variantsSet.SetCurrentVariant(_variantsSet.Variants.Last().Key);
		}

		[SetUp]
		public void InitializeContext()
		{
			_variantsSet = new ConnectionStringVariantsSetImpl(VariantsSetName);
		}

		[Test]
		public void AddVariant_NotNullVariant_ShouldAddNewVariant()
		{
			//Arrange
			var addedVariant = VariantsCreator.CreateVariant(0);

			//Act
			_variantsSet.AddVariant(addedVariant.Key, addedVariant.Value);

			//Assert
			Assert.That(_variantsSet.Variants.Count, Is.EqualTo(1));
			Assert.That(_variantsSet.Variants.First(), Is.EqualTo(addedVariant));
		}

		[Test]
		[TestCase(null, null)]
		[TestCase("", null)]
		[TestCase(null, "")]
		[TestCase("", "")]
		public void AddVariant_NullVariant_ShouldThrowException(string alias, string value)
		{
			Assert.That(() => _variantsSet.AddVariant(alias, value), Throws.Exception);
		}

		[Test]
		public void Aliases_ShouldReturnVariantsAliases()
		{
			//Arrange
			InitVariants();

			//Act
			var aliases = _variantsSet.Aliases;
			//Assert
			foreach (var idx in Enumerable.Range(0, 3))
				Assert.That(aliases[idx] == VariantsCreator.VariantAlias(idx));
		}

		[Test]
		public void CurrentVariant_NoVariantsAdded_ShouldReturnNullVariant()
		{
			Assert.That(_variantsSet.CurrentVariantAlias, Is.Null);
		}

		[Test]
		public void CurrentVariant_SomeVariantsAdded_ShouldReturnFirstAddedVariant()
		{
			//Arrange
			InitVariants();

			//Assert
			Assert.That(_variantsSet.CurrentVariantAlias, Is.EqualTo(_variantsSet.Variants.First().Key));
		}

		[Test]
		public void SetCurrentVariant_SetOwnedVariant_ShouldSetVariantProperly()
		{
			//Arrange
			InitVariants();

			//Act
			SelectLastVariant();

			//Assert
			Assert.That(_variantsSet.CurrentVariantAlias, Is.EqualTo(_variantsSet.Variants.Last().Key));
		}

		[Test]
		public void SetCurrentVariant_SetNotOwnedVariant_ShouldThrowArgumentException()
		{
			Assert.That(() => _variantsSet.SetCurrentVariant(VariantsCreator.VariantAlias(0)),
				Throws.ArgumentException);
		}

		[Test]
		public void SetCurrentVariant_NewValueDiffersFromOld_ShouldCallAllUpdaters()
		{
			//Arrange
			var variantToSet = InitVariants();
			var updaters = InitUpdaters(variantToSet.Value);

			//Act
			_variantsSet.SetCurrentVariant(variantToSet.Key);

			//Assert
			VerifyAllUpdatersCalled(updaters, variantToSet.Value);
		}

		[Test]
		public void SetCurrentVariant_SomeOfUpdatersThrowExceptions_AllUpdatersShouldBeCalled()
		{
			//Arrange
			var variantToSet = InitVariants();
			var updaters = InitUpdatersWithPossibleFails(variantToSet.Value);

			//Act
			try
			{
				_variantsSet.SetCurrentVariant(variantToSet.Key);
			}
			catch (Exception)
			{
				// ignored
			}

			//Assert
			VerifyAllUpdatersCalled(updaters, variantToSet.Value);
		}

		[Test]
		public void SetCurrentVariant_SomeOfUpdatersThrowExceptions_ShouldThrowAggregateException()
		{
			//Arrange
			var variantToSet = InitVariants();
			InitUpdatersWithPossibleFails(variantToSet.Value);

			//Assert
			try
			{
				_variantsSet.SetCurrentVariant(variantToSet.Key);
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

		private KeyValuePair<string, string> InitVariants()
		{
			foreach (var idx in Enumerable.Range(0, 3))
				VariantsCreator.AddVariant(_variantsSet, idx);
			return _variantsSet.Variants.Last();
		}

		private List<Mock<ConnectionStringUpdater>> InitUpdatersWithPossibleFails(string valueToSet)
		{
			var updaters = InitUpdaters(valueToSet);
			ThrowExceptionOnInvoke(updaters.First());
			ThrowExceptionOnInvoke(updaters.Last());
			return updaters;
		}

		private List<Mock<ConnectionStringUpdater>> InitUpdaters(string variantValueToSet)
		{
			var updaters = new List<Mock<ConnectionStringUpdater>>();
			for (var i = 0; i < 5; i++)
			{
				var updater = new Mock<ConnectionStringUpdater>();
				updater.Setup(x => x.SetNewValue(variantValueToSet)).Verifiable();
				updaters.Add(updater);
				_variantsSet.AddUpdater(updater.Object);
			}
			return updaters;
		}

		private static void VerifyAllUpdatersCalled(List<Mock<ConnectionStringUpdater>> updaters, string valueToSet)
		{
			foreach (var updater in updaters)
				updater.Verify(x => x.SetNewValue(valueToSet), Times.Once);
		}

		private static void ThrowExceptionOnInvoke(Mock<ConnectionStringUpdater> updater)
		{
			updater.Setup(x => x.SetNewValue(It.IsAny<string>()))
				.Throws(new ConnectionStringUpdatingException(ConnectionStringUpdateFailReason));
		}
	}
}
