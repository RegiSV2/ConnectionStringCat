using System.Diagnostics.Contracts;

namespace ConStringCat.Core.VSInterop
{
	[ContractClass(typeof (VariantsSetServiceContracts))]
	public interface VariantsSetService
	{
		string[] GetAliases();

		string GetSetCurrentVariant(string selectedAlias);

		bool IsServiceAvailable { get; }
	}

	[ContractClassFor(typeof (VariantsSetService))]
	internal abstract class VariantsSetServiceContracts : VariantsSetService
	{
		public string[] GetAliases()
		{
			Contract.Ensures(Contract.Result<string[]>() != null);
			return null;
		}

		public string GetSetCurrentVariant(string selectedAlias)
		{
			return null;
		}

		public bool IsServiceAvailable
		{
			get { return false; }
		}
	}
}