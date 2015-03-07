using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace ConStringCat.Core.Model
{
	public sealed class ConnectionStringVariantsSet
	{
		private readonly IList<ConnectionStringVariant> _variants;
		private readonly IList<ConnectionStringUpdater> _updaters;

		private readonly IReadOnlyList<ConnectionStringVariant> _readOnlyVariants;
		private readonly IReadOnlyList<ConnectionStringUpdater> _readOnlyUpdaters; 

		public string Name { get; private set; }

		public ConnectionStringVariant CurrentVariant { get; private set; }

		#region Constructors

		public ConnectionStringVariantsSet(string name)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			_variants = new List<ConnectionStringVariant>();
			_updaters = new List<ConnectionStringUpdater>();
			_readOnlyVariants = new ReadOnlyCollection<ConnectionStringVariant>(_variants);
			_readOnlyUpdaters = new ReadOnlyCollection<ConnectionStringUpdater>(_updaters);

			CurrentVariant = ConnectionStringVariant.Null;
			Name = name;
		}

		#endregion

		#region Public Contract

		public IReadOnlyList<ConnectionStringVariant> Variants
		{
			get { return _readOnlyVariants; }
		}

		public IReadOnlyList<ConnectionStringUpdater> Updaters
		{
			get { return _readOnlyUpdaters; }
		}

		public IList<string> Aliases
		{
			get { return _variants.Select(x => x.Name).ToList(); }
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
			CurrentVariant = Variants.First(x => x.Name == variantAlias);
			if (oldVariant != CurrentVariant)
				Parallel.ForEach(_updaters, updater => updater.SetNewValue(CurrentVariant.Value));
		}

		public void AddUpdater(ConnectionStringUpdater updater)
		{
			Contract.Assert(!_updaters.Contains(updater));

			_updaters.Add(updater);
		}

		#endregion

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(!string.IsNullOrEmpty(Name));
			Contract.Invariant(_variants != null);
		}
	}
}