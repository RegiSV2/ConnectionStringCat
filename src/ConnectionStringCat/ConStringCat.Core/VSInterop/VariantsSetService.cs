using System.Diagnostics.Contracts;
using ConStringCat.Core.Model;

namespace ConStringCat.Core.VSInterop
{
	[ContractClass(typeof (VariantsSetServiceContracts))]
	public interface VariantsSetService
	{
		void SetVariantsSet(ConnectionStringVariantsSet set);

		string[] GetAliases();

		string GetSetCurrentVariant(string selectedAlias);
	}

	[ContractClassFor(typeof (VariantsSetService))]
	internal abstract class VariantsSetServiceContracts : VariantsSetService
	{
		public void SetVariantsSet(ConnectionStringVariantsSet set)
		{
			Contract.Requires(set != null);
		}

		public string[] GetAliases()
		{
			Contract.Ensures(Contract.Result<string[]>() != null);
			return null;
		}

		public string GetSetCurrentVariant(string selectedAlias)
		{
			return null;
		}
	}
}