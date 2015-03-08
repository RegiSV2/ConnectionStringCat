using System.Linq;
using ConStringCat.Core.Model;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop
{
	[TestFixture]
	public class NullConnectionStringVariantsSetTests
	{
		private readonly NullConnectionStringVariantsSet _set = NullConnectionStringVariantsSet.Instance;

		[Test]
		public void AllProperties_ShouldBeEmpty()
		{
			Assert.That(!_set.Aliases.Any());
			Assert.That(_set.CurrentVariantAlias, Is.Null);
			Assert.That(_set.Name, Is.EqualTo("-"));
			Assert.That(!_set.Variants.Any());
		}

		[Test]
		public void SetCurrentVariant_ShouldThrowArgumentException()
		{
			Assert.That(() => _set.SetCurrentVariant("some variant alias"),
				Throws.ArgumentException);
		}
	}
}