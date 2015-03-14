using System.Linq;
using ConStringCat.Core.Model;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.Model
{
	[TestFixture]
	public class NullConfigurationAliasesEntityTests
	{
		private readonly NullConfigurationAliasesEntity _entity = NullConfigurationAliasesEntity.Instance;

		[Test]
		public void AllProperties_ShouldBeEmpty()
		{
			Assert.That(!_entity.Aliases.Any());
			Assert.That(_entity.CurrentVariantAlias, Is.Null);
			Assert.That(_entity.Name, Is.Null);
		}

		[Test]
		public void SetCurrentVariant_ShouldThrowArgumentException()
		{
			Assert.That(() => _entity.SetCurrentVariant("some variant alias"),
				Throws.ArgumentException);
		}
	}
}