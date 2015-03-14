using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using ConStringCat.Core.VSInterop;

namespace ConStringCat.Core.Model
{
	/// <remarks>Non thread-safe</remarks>
	public class ConfigurationValueVariantsSet : ConfigurationVariantsSet
	{
		#region Constants

		private const string VariantIsNotRegisteredMsg = "The variant \"{0}\" is not registered";

		#endregion

		#region Constructors

		public ConfigurationValueVariantsSet(string name)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			_variants = new Dictionary<string, string>();
			_updaters = new List<ConfigurationValueUpdater>();
			_readOnlyVariants = new ReadOnlyDictionary<string, string>(_variants);
			_readOnlyUpdaters = new ReadOnlyCollection<ConfigurationValueUpdater>(_updaters);

			CurrentVariantAlias = null;
			Name = name;
		}

		#endregion

		private void InvokeUpdaters(string oldAlias)
		{
			var exceptions = new List<Exception>();

			foreach (var updater in _updaters)
			{
				try
				{
					updater.SetNewValue(_variants[oldAlias]);
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
			}

			if (exceptions.Any())
				throw new AggregateException(exceptions);
		}

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(!string.IsNullOrEmpty(Name));
			Contract.Invariant(_variants != null);
			Contract.Invariant(_updaters != null);
		}

		#region State

		private readonly IDictionary<string, string> _variants;

		private readonly IList<ConfigurationValueUpdater> _updaters;

		private readonly IReadOnlyDictionary<string, string> _readOnlyVariants;

		private readonly IReadOnlyList<ConfigurationValueUpdater> _readOnlyUpdaters;

		public string Name { get; private set; }

		public string CurrentVariantAlias { get; private set; }

		#endregion

		#region Public Contract

		public IReadOnlyDictionary<string, string> Variants
		{
			get { return _readOnlyVariants; }
		}

		public IReadOnlyList<ConfigurationValueUpdater> Updaters
		{
			get { return _readOnlyUpdaters; }
		}

		public IReadOnlyList<string> Aliases
		{
			get { return new ReadOnlyCollection<string>(_variants.Keys.ToList()); }
		}

		public void AddVariant(string alias, string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(alias));
			Contract.Requires(!string.IsNullOrEmpty(value));

			_variants.Add(alias, value);
			if (CurrentVariantAlias == null)
				SetCurrentVariant(alias);
		}

		public void SetCurrentVariant(string variantAlias)
		{
			if (!_variants.ContainsKey(variantAlias))
				throw new ArgumentException(string.Format(VariantIsNotRegisteredMsg, variantAlias));

			var oldAlias = CurrentVariantAlias;
			CurrentVariantAlias = variantAlias;
			if (oldAlias != CurrentVariantAlias)
			{
				InvokeUpdaters(oldAlias);
			}
		}

		public void AddUpdater(ConfigurationValueUpdater updater)
		{
			Contract.Assert(!_updaters.Contains(updater));

			_updaters.Add(updater);
		}

		#endregion
	}
}