using System.Diagnostics.Contracts;
using System.Linq;
using ConStringCat.Core.Model;
using EnvDTE;

namespace ConStringCat.Core.VSInterop
{
	public class VariantsSetServiceImpl : VariantsSetService
	{
		private ConnectionStringVariantsSet _workingSet;

		private readonly DTE _solutionInteropObject;

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
			if (!_solutionInteropObject.Solution.IsOpen)
				return new string[0];
			return _workingSet.Aliases.ToArray();
		}

		public string GetSetCurrentVariant(string selectedAlias)
		{
			if (!_solutionInteropObject.Solution.IsOpen)
				return null;
			if(selectedAlias != null)
				_workingSet.SetCurrentVariant(selectedAlias);
			return _workingSet.CurrentVariant.Alias;
		}
	}
}
