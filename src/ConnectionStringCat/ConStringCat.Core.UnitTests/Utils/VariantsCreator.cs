using System.Collections.Generic;
using ConStringCat.Core.VSInterop;
using Moq;

namespace ConStringCat.Core.UnitTests.Utils
{
	public static class VariantsCreator
	{
		private const string VariantStrValue = "Some str name";
		private const string VariantAliasName = "Some var name";

		public static void AddVariant(Mock<ConfigurationAliasesEntity> entity, int aliasIndex)
		{
			var alias = CreateAlias(aliasIndex);
			var oldList = entity.Object.Aliases ?? new List<string>();
			var newList = new List<string>(oldList);
			newList.Add(alias);
			entity.SetupGet(x => x.Aliases).Returns(newList);
		}

		public static KeyValuePair<string, string> CreateVariant(int aliasIndex)
		{
			return new KeyValuePair<string, string>(CreateAlias(aliasIndex), CreateVariantValue(aliasIndex));
		}

		public static string CreateAlias(int aliasIndex)
		{
			return VariantAliasName + aliasIndex;
		}

		public static string CreateVariantValue(int valueIndex)
		{
			return VariantStrValue + valueIndex;
		}
	}
}