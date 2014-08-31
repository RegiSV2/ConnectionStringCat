using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConStringCat.Core.Model;

namespace ConStringCat.Core
{
	public interface VariantsSetService
	{
		void SetVariantsSet(ConnectionStringVariantsSet set);

		string[] GetAliases();

		string GetSetCurrentVariant(string selectedAlias);
	}
}
