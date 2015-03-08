using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using EnvDTE;

namespace ConStringCat.Core.VSInterop
{
	public sealed class VariantsSetServiceImpl : VariantsSetService
	{
		private readonly DTE _solutionInteropObject;

		private readonly object _syncRoot = new object();

		private readonly VariantsSettingsLoader _variantsSettingsLoader;

		private string _loadedSolutionPath;

		private ConnectionStringVariantsSet _workingSet;

		public VariantsSetServiceImpl(DTE solutionInteropObject, VariantsSettingsLoader variantsSettingsLoader)
		{
			Contract.Requires(solutionInteropObject != null);
			Contract.Requires(variantsSettingsLoader != null);
			_solutionInteropObject = solutionInteropObject;
			_variantsSettingsLoader = variantsSettingsLoader;
		}

		public string[] GetAliases()
		{
			return WorkingSet.Aliases.ToArray();
		}

		public string GetSetCurrentVariant(string selectedAlias)
		{
			if (selectedAlias != null)
				WorkingSet.SetCurrentVariant(selectedAlias);
			return WorkingSet.CurrentVariantAlias;
		}

		public bool IsServiceAvailable
		{
			get { return Solution.IsOpen; }
		}

		private ConnectionStringVariantsSet WorkingSet
		{
			get
			{
				if (Solution.IsOpen)
					LoadWorkingSetIfNotLoaded();
				else
					RemoveWorkingSetIfNotRemoved();
				return _workingSet;
			}
		}

		private Solution Solution
		{
			get { return _solutionInteropObject.Solution; }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void LoadWorkingSetIfNotLoaded()
		{
			if (WorkingSetChanged())
			{
				lock (_syncRoot)
				{
					if (WorkingSetChanged())
					{
						_loadedSolutionPath = Solution.FileName;
						_workingSet = _variantsSettingsLoader.LoadVariantsSetForSolution(Solution.FileName);
					}
				}
			}
		}

		private bool WorkingSetChanged()
		{
			return _workingSet == null || _loadedSolutionPath != Solution.FileName;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveWorkingSetIfNotRemoved()
		{
			lock (_syncRoot)
			{
				if (!Solution.IsOpen)
				{
					_workingSet = _variantsSettingsLoader.GetEmptyVariantsSet();
					_loadedSolutionPath = null;
				}
			}
		}
	}
}