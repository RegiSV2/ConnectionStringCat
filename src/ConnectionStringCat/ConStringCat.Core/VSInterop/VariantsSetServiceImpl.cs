using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using ConStringCat.Core.Model;
using EnvDTE;

namespace ConStringCat.Core.VSInterop
{
	public sealed class VariantsSetServiceImpl : VariantsSetService
	{
		private readonly DTE _solutionInteropObject;
		private readonly object _syncRoot = new object();
		private readonly VariantsSettingsLoader _variantsSettingsLoader;
		private string _loadedSolutionPath;
		private IList<ConfigurationAliasesEntity> _solutionAspects = new List<ConfigurationAliasesEntity>(); 
		private ConfigurationAliasesEntity _workingEntity = NullConfigurationAliasesEntity.Instance;

		public VariantsSetServiceImpl(DTE solutionInteropObject, VariantsSettingsLoader variantsSettingsLoader)
		{
			Contract.Requires(solutionInteropObject != null);
			Contract.Requires(variantsSettingsLoader != null);
			_solutionInteropObject = solutionInteropObject;
			_variantsSettingsLoader = variantsSettingsLoader;
		}

		private Solution Solution
		{
			get { return _solutionInteropObject.Solution; }
		}

		public string[] GetAspects()
		{
			UpdateAspects();
			return _solutionAspects.Select(x => x.Name).ToArray();
		}

		public string[] GetAliases()
		{
			UpdateAspects();
			return _workingEntity.Aliases.ToArray();
		}

		public string GetSetCurrentAspect(string selectedAspect)
		{
			UpdateAspects();
			if (selectedAspect != null)
				_workingEntity = _solutionAspects.FirstOrDefault(x => x.Name == selectedAspect) ?? NullConfigurationAliasesEntity.Instance;
			return _workingEntity.Name;
		}

		public string GetSetCurrentVariant(string selectedVariantAlias)
		{
			UpdateAspects();
			if (selectedVariantAlias != null)
				_workingEntity.SetCurrentVariant(selectedVariantAlias);
			return _workingEntity.CurrentVariantAlias;
		}

		public bool IsServiceAvailable
		{
			get { return Solution.IsOpen; }
		}

		private void UpdateAspects()
		{
			if (Solution.IsOpen)
				LoadAspectsIfNotLoaded();
			else
				RemoveAspectsIfNotRemoved();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void LoadAspectsIfNotLoaded()
		{
			if (WorkingSetChanged())
			{
				lock (_syncRoot)
				{
					if (WorkingSetChanged())
					{
						_loadedSolutionPath = Solution.FileName;
						_solutionAspects = _variantsSettingsLoader.LoadAspectsForSolution(Solution.FileName);
						_workingEntity = _solutionAspects.FirstOrDefault() ?? NullConfigurationAliasesEntity.Instance;
					}
				}
			}
		}

		private bool WorkingSetChanged()
		{
			return _solutionAspects == null || _loadedSolutionPath != Solution.FileName;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveAspectsIfNotRemoved()
		{
			lock (_syncRoot)
			{
				if (!Solution.IsOpen)
				{
					_workingEntity = _variantsSettingsLoader.GetEmptyAspect();
					_solutionAspects = new List<ConfigurationAliasesEntity>();
					_loadedSolutionPath = null;
				}
			}
		}
	}
}