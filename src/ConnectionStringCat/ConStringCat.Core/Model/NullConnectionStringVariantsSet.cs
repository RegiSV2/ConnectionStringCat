using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ConStringCat.Core.VSInterop;

namespace ConStringCat.Core.Model
{
	/// <summary>
	/// Represents an empty set with no behavior
	/// </summary>
	public class NullConnectionStringVariantsSet : ConnectionStringVariantsSet
	{
		#region Singleton

		private NullConnectionStringVariantsSet()
		{ }

		public static readonly NullConnectionStringVariantsSet Instance = new NullConnectionStringVariantsSet();

		#endregion

		private const string CannotSetCurrentVariantMsg = "Cannot select variant because ConnectionStringCat is inactive";

		public string Name
		{
			get { return "-"; }
		}

		public string CurrentVariantAlias
		{
			get { return null; }
		}

		public IReadOnlyDictionary<string, string> Variants
		{
			get { return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()); }
		}

		public IList<string> Aliases
		{
			get { return new List<string>(); }
		}

		public void SetCurrentVariant(string variantAlias)
		{
			throw new ArgumentException(CannotSetCurrentVariantMsg);
		}
	}
}