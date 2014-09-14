using System.Diagnostics.Contracts;
using System.Linq;
using ConStringCat.Core.Model;
using EnvDTE;

namespace ConStringCat.Core.VSInterop
{
	public class VariantsSetServiceImpl : VariantsSetService
	{
		private readonly DTE _solutionInteropObject;
		private ConnectionStringVariantsSet _workingSet;

		public VariantsSetServiceImpl(DTE solutionInteropObject)
		{
			Contract.Requires(solutionInteropObject != null);
			_solutionInteropObject = solutionInteropObject;
		}

		public void SetVariantsSet(ConnectionStringVariantsSet set)
		{
			_workingSet = set;
		}

		public string[] GetAliases()
		{
			return !_solutionInteropObject.Solution.IsOpen
				? new string[0]
				: _workingSet.Aliases.ToArray();
		}

		public string GetSetCurrentVariant(string selectedAlias)
		{
			if (!_solutionInteropObject.Solution.IsOpen)
				return null;
			if (selectedAlias != null)
				_workingSet.SetCurrentVariant(selectedAlias);
			return _workingSet.CurrentVariant.Alias;
		}
	}
}