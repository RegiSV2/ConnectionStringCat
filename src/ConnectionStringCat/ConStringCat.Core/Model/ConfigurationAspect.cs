using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ConStringCat.Core.VSInterop;

namespace ConStringCat.Core.Model
{
	/// <summary>
	///     An aspect of solution configuration that has some options to choose from
	/// </summary>
	public class ConfigurationAspect : ConfigurationAliasesEntity
	{
		private const string AliasAlreadyAddedMsg = "The alias \"{0}\" already exists";
		private const string AliasNotFoundMsg = "The alias \"{0}\" was not found";
		private const string SetAlreadyAddedMsg = "The variants set \"{0}\" already exists";
		private readonly ISet<string> _aliases = new HashSet<string>();
		private readonly ISet<ConfigurationVariantsSet> _variantsSet = new HashSet<ConfigurationVariantsSet>();

		public ConfigurationAspect(string name)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Name = name;
		}

		public IReadOnlyCollection<ConfigurationVariantsSet> Sets
		{
			get { return _variantsSet.ToList(); }
		}

		public string Name { get; private set; }
		public string CurrentVariantAlias { get; private set; }

		public IReadOnlyList<string> Aliases
		{
			get { return _aliases.ToList(); }
		}

		public void SetCurrentVariant(string variantAlias)
		{
			if (!_aliases.Contains(variantAlias))
				throw new ArgumentException(string.Format(AliasNotFoundMsg, variantAlias));

			var oldAlias = CurrentVariantAlias;
			CurrentVariantAlias = variantAlias;

			if (oldAlias != CurrentVariantAlias)
				UpdateVariantInSets();
		}

		public void AddAlias(string alias)
		{
			Contract.Requires(!string.IsNullOrEmpty(alias));
			if (_aliases.Contains(alias))
				throw new ArgumentException(string.Format(AliasAlreadyAddedMsg, alias));

			_aliases.Add(alias);
			if (CurrentVariantAlias == null)
				SetCurrentVariant(alias);
		}

		public void AddVariantsSet(ConfigurationVariantsSet variantsSet)
		{
			Contract.Requires(variantsSet != null);
			if (_variantsSet.Contains(variantsSet))
				throw new ArgumentException(string.Format(SetAlreadyAddedMsg, variantsSet.Name));

			_variantsSet.Add(variantsSet);
		}

		public void RefreshSetVariants()
		{
			foreach (var set in _variantsSet)
				set.RefreshSelectedVariant();
		}

		private void UpdateVariantInSets()
		{
			foreach (var set in _variantsSet.Where(x => x.Aliases.Contains(CurrentVariantAlias)))
				set.SetCurrentVariant(CurrentVariantAlias);
		}
	}
}