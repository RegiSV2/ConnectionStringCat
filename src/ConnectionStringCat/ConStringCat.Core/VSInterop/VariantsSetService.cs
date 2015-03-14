using System.Diagnostics.Contracts;

namespace ConStringCat.Core.VSInterop
{
	[ContractClass(typeof (VariantsSetServiceContracts))]
	public interface VariantsSetService
	{
		bool IsServiceAvailable { get; }
		string[] GetAspects();
		string[] GetAliases();
		string GetSetCurrentAspect(string selectedAspect);
		string GetSetCurrentVariant(string selectedVariantAlias);
	}

	[ContractClassFor(typeof (VariantsSetService))]
	internal abstract class VariantsSetServiceContracts : VariantsSetService
	{
		public string[] GetAspects()
		{
			Contract.Ensures(Contract.Result<string[]>() != null);
			return null;
		}

		public string[] GetAliases()
		{
			Contract.Ensures(Contract.Result<string[]>() != null);
			return null;
		}

		public string GetSetCurrentAspect(string selectedAspect)
		{
			return null;
		}

		public string GetSetCurrentVariant(string selectedVariantAlias)
		{
			return null;
		}

		public bool IsServiceAvailable
		{
			get { return false; }
		}
	}
}