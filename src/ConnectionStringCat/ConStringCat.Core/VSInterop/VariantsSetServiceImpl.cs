using System.Diagnostics.Contracts;
using System.Linq;
using ConStringCat.Core.Model;
using EnvDTE;

namespace ConStringCat.Core.VSInterop
{
	public sealed class VariantsSetServiceImpl : VariantsSetService
	{
		private readonly DTE _solutionInteropObject;
		private ConnectionStringVariantsSet _workingSet;
		private readonly string[] _emptyAliasesArray = new string[0];

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
				? _emptyAliasesArray
				: _workingSet.Aliases.ToArray();
		}

		public string GetSetCurrentVariant(string selectedAlias)
		{
			if (!_solutionInteropObject.Solution.IsOpen)
				return null;
			if (selectedAlias != null)
				_workingSet.SetCurrentVariant(selectedAlias);
			return _workingSet.CurrentVariant.Name;
		}
	}
}