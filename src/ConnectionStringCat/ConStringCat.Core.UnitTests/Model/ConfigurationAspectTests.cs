using System.Collections.Generic;
using System.Linq;
using ConStringCat.Core.Model;
using ConStringCat.Core.VSInterop;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.Model
{
	[TestFixture]
	public class ConfigurationAspectTests
	{
		private const string CorrectAspectName = "aspect name";
		private const string AliasName = "alias1";
		private ConfigurationAspect _configurationAspect;

		[SetUp]
		public void SetUp()
		{
			_configurationAspect = new ConfigurationAspect(CorrectAspectName);
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		public void Constructor_NullOrEmptyName_ShouldFail(string name)
		{
			Assert.That(() => new ConfigurationAspect(name),
				Throws.Exception);
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		public void AddAlias_NullOrEmpty_ShouldFail(string alias)
		{
			Assert.That(() => _configurationAspect.AddAlias(alias),
				Throws.Exception);
		}

		[Test]
		public void AddAlias_AliasNotAdded_ShouldAddNewAlias()
		{
			_configurationAspect.AddAlias(AliasName);

			Assert.That(_configurationAspect.Aliases.Contains(AliasName));
		}

		[Test]
		public void AddAlias_AliasAdded_ShouldThrowArgumentException()
		{
			_configurationAspect.AddAlias(AliasName);

			Assert.That(() => _configurationAspect.AddAlias(AliasName), Throws.ArgumentException);
		}

		[Test]
		public void CurrentVariantAlias_NoAliases_ShouldBeNull()
		{
			Assert.That(_configurationAspect.CurrentVariantAlias == null);
		}

		[Test]
		public void CurrentVariantAlias_FirstAliasAdded_ShouldBecomeFirstAlias()
		{
			_configurationAspect.AddAlias(AliasName);
			_configurationAspect.AddAlias(AliasName + AliasName);

			Assert.That(_configurationAspect.CurrentVariantAlias == AliasName);
		}

		[Test]
		public void SetCurrentVariant_AliasAdded_ShouldChangeCurrentVariantAlias()
		{
			_configurationAspect.AddAlias(AliasName);
			_configurationAspect.AddAlias(AliasName + AliasName);
			_configurationAspect.SetCurrentVariant(AliasName + AliasName);

			Assert.That(_configurationAspect.CurrentVariantAlias == AliasName + AliasName);
		}

		[Test]
		public void SetCurrentVariant_AliasNotAdded_ShouldThrowArgumentException()
		{
			Assert.That(() => _configurationAspect.SetCurrentVariant(AliasName),
				Throws.ArgumentException);
		}

		[Test]
		public void SetCurrentVariant_HasRegisteredVariantSets_ShouldInvokeAllVariantsSetsWhichHaveAlias()
		{
			//Arrange
			_configurationAspect.AddAlias(AliasName + AliasName);
			_configurationAspect.AddAlias(AliasName);

			var setNotToInvoke = AddVariantsSetMock(new string[0], AliasName);
			var setToInvoke = AddVariantsSetMock(new[] {AliasName}, AliasName);

			//Act
			_configurationAspect.SetCurrentVariant(AliasName);

			//Assert
			setNotToInvoke.Verify(x => x.SetCurrentVariant(AliasName), Times.Never);
			setToInvoke.Verify(x => x.SetCurrentVariant(AliasName), Times.Once);
		}

		[Test]
		public void AddVariantsSet_NullSet_ShouldFail()
		{
			Assert.That(() => _configurationAspect.AddVariantsSet(null),
				Throws.Exception);
		}

		[Test]
		public void AddVariantsSet_SetAlreadyAdded_ShouldFail()
		{
			var variantsSet = new Mock<ConfigurationVariantsSet>().Object;
			_configurationAspect.AddVariantsSet(variantsSet);

			Assert.That(() => _configurationAspect.AddVariantsSet(variantsSet),
				Throws.ArgumentException);
		}

		private Mock<ConfigurationVariantsSet> AddVariantsSetMock(IReadOnlyList<string> containedAliases,
			string expectedAlias)
		{
			var set = new Mock<ConfigurationVariantsSet>();
			set.Setup(x => x.Aliases).Returns(containedAliases);
			set.Setup(x => x.SetCurrentVariant(expectedAlias)).Verifiable();
			_configurationAspect.AddVariantsSet(set.Object);
			return set;
		}
	}
}