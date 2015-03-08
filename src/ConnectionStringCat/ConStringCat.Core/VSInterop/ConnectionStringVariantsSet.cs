using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ConStringCat.Core.VSInterop
{
	/// <summary>
	/// Represents a set of connection string possible variants
	/// </summary>
	[ContractClass(typeof(ConnectionStringVariantsSetContracts))]
	public interface ConnectionStringVariantsSet
	{
		/// <summary>
		/// Returns the name of the set
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns active variant alias
		/// </summary>
		string CurrentVariantAlias { get; }

		/// <summary>
		/// Returns all variants registered at set
		/// </summary>
		IReadOnlyDictionary<string, string> Variants { get; }

		/// <summary>
		/// Returns all variant aliases registered at set
		/// </summary>
		IList<string> Aliases { get; }

		/// <summary>
		/// Sets active variant
		/// </summary>
		/// <param name="variantAlias">Variant alias to make active</param>
		/// <exception cref="ArgumentException">Thrown if variant alias is not registered at set</exception>
		void SetCurrentVariant(string variantAlias);
	}

	[ContractClassFor(typeof(ConnectionStringVariantsSet))]
	abstract class ConnectionStringVariantsSetContracts : ConnectionStringVariantsSet
	{
		public string Name
		{
			get
			{
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				return null;
			}
		}

		public string CurrentVariantAlias
		{
			get { return null; }
		}

		public IReadOnlyDictionary<string, string> Variants
		{
			get
			{
				Contract.Ensures(Contract.Result<IReadOnlyDictionary<string, string>>() != null);
				return null;
			}
		}

		public IList<string> Aliases
		{
			get
			{
				Contract.Ensures(Contract.Result<IList<string>>() != null);
				return null;
			}
		}

		public void SetCurrentVariant(string variantAlias)
		{
			Contract.Requires(!string.IsNullOrEmpty(variantAlias));
		}
	}
}
