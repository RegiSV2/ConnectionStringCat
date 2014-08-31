﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConStringCat.Core.Model;

namespace ConStringCat.Core.UnitTests.Utils
{
	public static class VariantsCreator
	{
		private const string VariantStrName = "Some str name";

		private const string VariantAliasName = "Some var name";

		public static ConnectionStringVariant Variant(int aliasIndex)
		{
			return new ConnectionStringVariant(VariantAlias(aliasIndex), VariantStrName);
		}

		public static string VariantAlias(int aliasIndex)
		{
			return VariantAliasName + aliasIndex;
		} 
	}
}
