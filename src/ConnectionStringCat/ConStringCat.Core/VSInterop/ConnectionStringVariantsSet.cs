﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ConStringCat.Core.VSInterop
{
	/// <summary>
	///     Represents a set of connection string possible variants
	/// </summary>
	[ContractClass(typeof (ConnectionStringVariantsSetContracts))]
	public interface ConnectionStringVariantsSet : ConfigurationAliasesEntity
	{
		/// <summary>
		///     Returns all variants registered at set
		/// </summary>
		IReadOnlyDictionary<string, string> Variants { get; }
	}

	[ContractClassFor(typeof (ConnectionStringVariantsSet))]
	internal abstract class ConnectionStringVariantsSetContracts : ConnectionStringVariantsSet
	{
		public IReadOnlyDictionary<string, string> Variants
		{
			get
			{
				Contract.Ensures(Contract.Result<IReadOnlyDictionary<string, string>>() != null);
				return null;
			}
		}

		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		public string CurrentVariantAlias
		{
			get { throw new NotImplementedException(); }
		}

		public IReadOnlyList<string> Aliases
		{
			get { throw new NotImplementedException(); }
		}

		public void SetCurrentVariant(string variantAlias)
		{
			throw new NotImplementedException();
		}
	}
}