using System;
using System.Collections.Generic;
using ConStringCat.Core.VSInterop;

namespace ConStringCat.Core.Model
{
	/// <summary>
	///     Represents a null ConfugurationAliasesEntity
	/// </summary>
	public class NullConfigurationAliasesEntity : ConfigurationAliasesEntity
	{
		public string Name
		{
			get { return null; }
		}

		public string CurrentVariantAlias
		{
			get { return null; }
		}

		public IReadOnlyList<string> Aliases
		{
			get { return new List<string>(); }
		}

		public void SetCurrentVariant(string variantAlias)
		{
			throw new ArgumentException("Cannot set current variant of null entity");
		}

		#region Singleton

		private NullConfigurationAliasesEntity()
		{
		}

		public static readonly NullConfigurationAliasesEntity Instance = new NullConfigurationAliasesEntity();

		#endregion
	}
}