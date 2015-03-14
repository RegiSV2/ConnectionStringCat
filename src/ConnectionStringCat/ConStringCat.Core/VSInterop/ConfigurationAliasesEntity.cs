using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ConStringCat.Core.VSInterop
{
	[ContractClass(typeof (ConfigurationAliasesEntityContracts))]
	public interface ConfigurationAliasesEntity
	{
		/// <summary>
		///     Returns the name of the element
		/// </summary>
		string Name { get; }

		/// <summary>
		///     Returns active variant alias
		/// </summary>
		string CurrentVariantAlias { get; }

		/// <summary>
		///     Returns all variant aliases registered at set
		/// </summary>
		IReadOnlyList<string> Aliases { get; }

		/// <summary>
		///     Sets active variant
		/// </summary>
		/// <param name="variantAlias">Variant alias to make active</param>
		/// <exception cref="ArgumentException">Thrown if variant alias is not registered at set</exception>
		void SetCurrentVariant(string variantAlias);
	}

	[ContractClassFor(typeof (ConfigurationAliasesEntity))]
	internal abstract class ConfigurationAliasesEntityContracts : ConfigurationAliasesEntity
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
			get
			{
				Contract.Ensures(Contract.Result<IReadOnlyCollection<string>>() != null);
				return null;
			}
		}

		public void SetCurrentVariant(string variantAlias)
		{
			Contract.Requires(!string.IsNullOrEmpty(variantAlias));
		}
	}
}