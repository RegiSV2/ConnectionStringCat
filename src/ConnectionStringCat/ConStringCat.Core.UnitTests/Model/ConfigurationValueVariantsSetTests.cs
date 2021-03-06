﻿using System;
using System.Collections.Generic;
using System.Linq;
using ConStringCat.Core.Model;
using ConStringCat.Core.UnitTests.Utils;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.Model
{
	[TestFixture]
	public class ConfigurationValueVariantsSetTests
	{
		private const string VariantsSetName = "Some name";
		private const string ConnectionStringUpdateFailReason = "Some reason";
		private ConfigurationValueVariantsSet _variantsSet;

		private void SelectLastVariant()
		{
			_variantsSet.SetCurrentVariant(_variantsSet.Variants.Last().Key);
		}

		[SetUp]
		public void InitializeContext()
		{
			_variantsSet = new ConfigurationValueVariantsSet(VariantsSetName);
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
				Assert.That(aliases[idx] == VariantsCreator.CreateAlias(idx));
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
			Assert.That(() => _variantsSet.SetCurrentVariant(VariantsCreator.CreateAlias(0)),
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
				Assert.That(ex.InnerExceptions.OfType<ConfigurationValueUpdatingException>().Count() == 2);
			}
		}

		[Test]
		public void AddUpdater_UpdaterNotAdded_ShouldAddNewUpdaterToUpdaters()
		{
			var updater = new Mock<ConfigurationValueUpdater>().Object;

			_variantsSet.AddUpdater(updater);

			Assert.That(_variantsSet.Updaters.Contains(updater));
		}

		[Test]
		public void AddUpdater_UpdaterAlreadyAdded_ShouldThrowExcpetion()
		{
			var updater = new Mock<ConfigurationValueUpdater>().Object;

			_variantsSet.AddUpdater(updater);

			Assert.That(() => _variantsSet.AddUpdater(updater), Throws.Exception);
		}

		[Test]
		public void RefreshSelectedVariant_CurrentVariantIsNotNull_ShouldInvokeUpdaters()
		{
			//Arrange
			InitVariants();
			var expectedVariant = _variantsSet.Variants[_variantsSet.CurrentVariantAlias];
			var updaters = InitUpdaters(expectedVariant);

			//Act
			_variantsSet.RefreshSelectedVariant();

			//Assert
			updaters.ForEach(updater => updater.Verify(x => x.SetNewValue(expectedVariant), Times.Once));
		}

		[Test]
		public void RefreshSelectedVariant_CurrentVariantIsNull_ShouldNotInvokeUpdaters()
		{
			//Arrange
			var updaters = InitUpdaters(It.IsAny<string>());

			//Act
			_variantsSet.RefreshSelectedVariant();

			//Assert
			updaters.ForEach(updater => updater.Verify(x => x.SetNewValue(It.IsAny<string>()), Times.Never));
		}

		private KeyValuePair<string, string> InitVariants()
		{
			foreach (var idx in Enumerable.Range(0, 3))
			{
				var variant = VariantsCreator.CreateVariant(idx);
				_variantsSet.AddVariant(variant.Key, variant.Value);
			}
			return _variantsSet.Variants.Last();
		}

		private List<Mock<ConfigurationValueUpdater>> InitUpdatersWithPossibleFails(string valueToSet)
		{
			var updaters = InitUpdaters(valueToSet);
			ThrowExceptionOnInvoke(updaters.First());
			ThrowExceptionOnInvoke(updaters.Last());
			return updaters;
		}

		private List<Mock<ConfigurationValueUpdater>> InitUpdaters(string variantValueToSet)
		{
			var updaters = new List<Mock<ConfigurationValueUpdater>>();
			for (var i = 0; i < 5; i++)
			{
				var updater = new Mock<ConfigurationValueUpdater>();
				updater.Setup(x => x.SetNewValue(variantValueToSet)).Verifiable();
				updaters.Add(updater);
				_variantsSet.AddUpdater(updater.Object);
			}
			return updaters;
		}

		private static void VerifyAllUpdatersCalled(List<Mock<ConfigurationValueUpdater>> updaters, string valueToSet)
		{
			foreach (var updater in updaters)
				updater.Verify(x => x.SetNewValue(valueToSet), Times.Once);
		}

		private static void ThrowExceptionOnInvoke(Mock<ConfigurationValueUpdater> updater)
		{
			updater.Setup(x => x.SetNewValue(It.IsAny<string>()))
				.Throws(new ConfigurationValueUpdatingException(ConnectionStringUpdateFailReason));
		}
	}
}