using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ConStringCat.Core.Model
{
	public sealed class ConnectionStringVariantsSet
	{
		private readonly IReadOnlyList<ConnectionStringVariant> _readOnlyVariantsVersion;
		private readonly IList<ConnectionStringVariant> _variants;

		public string VariantsSetName { get; private set; }

		public ConnectionStringVariant CurrentVariant { get; private set; }

		#region Constructors

		public ConnectionStringVariantsSet(string variantSetName)
		{
			Contract.Requires(!string.IsNullOrEmpty(variantSetName));
			_variants = new List<ConnectionStringVariant>();
			_readOnlyVariantsVersion = new ReadOnlyCollection<ConnectionStringVariant>(_variants);
			CurrentVariant = ConnectionStringVariant.Null;
			VariantsSetName = variantSetName;
		}

		#endregion

		#region Public Contract

		public IReadOnlyList<ConnectionStringVariant> Variants
		{
			get { return _readOnlyVariantsVersion; }
		}

		public IList<string> Aliases
		{
			get { return _variants.Select(x => x.Alias).ToList(); }
		}

		public void AddVariant(ConnectionStringVariant newVariant)
		{
			Contract.Requires(newVariant != null);
			_variants.Add(newVariant);
			if (CurrentVariant == ConnectionStringVariant.Null)
				CurrentVariant = newVariant;
		}

		public void SetCurrentVariant(string variantAlias)
		{
			var oldVariant = CurrentVariant;
			CurrentVariant = Variants.First(x => x.Alias == variantAlias);
			if (oldVariant != CurrentVariant)
				OnVariantChangedEvent();
		}

		#endregion

		public event EventHandler VariantChangedEvent;

		private void OnVariantChangedEvent()
		{
			var handler = VariantChangedEvent;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(!string.IsNullOrEmpty(VariantsSetName));
			Contract.Invariant(_variants != null);
		}
	}
}