using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConStringCat.Core.Model;

namespace ConStringCat.Core
{
	public class VariantsSetServiceImpl : VariantsSetService
	{
		private ConnectionStringVariantsSet _workingSet;

		public void SetVariantsSet(ConnectionStringVariantsSet set)
		{
			_workingSet = set;
		}

		public string[] GetAliases()
		{
			return _workingSet.Aliases.ToArray();
		}

		public string GetSetCurrentVariant(string selectedAlias)
		{
			if(selectedAlias != null)
				_workingSet.SetCurrentVariant(selectedAlias);
			return _workingSet.CurrentVariant.Alias;
		}
	}
}
