using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ConStringCat.Core.VSInterop
{
	/// <summary>
	///     Loads connection string variants for specified solution file
	/// </summary>
	[ContractClass(typeof (VariantsSettingsLoaderContracts))]
	public interface VariantsSettingsLoader
	{
		/// <summary>
		///     Loads ConnectionStringCat settings for specified solution file
		/// </summary>
		/// <param name="solutionFileName">A full or relative path to solution file</param>
		/// <returns>Fully initialized variants set</returns>
		/// <exception cref="VariantsSettingsLoadingException">Thrown when some error occured during settings loading</exception>
		IList<ConfigurationAliasesEntity> LoadAspectsForSolution(string solutionFileName);

		/// <summary>
		///     Returns empty set
		/// </summary>
		ConfigurationAliasesEntity GetEmptyAspect();
	}

	[ContractClassFor(typeof (VariantsSettingsLoader))]
	internal abstract class VariantsSettingsLoaderContracts : VariantsSettingsLoader
	{
		public IList<ConfigurationAliasesEntity> LoadAspectsForSolution(string solutionFileName)
		{
			Contract.Ensures(Contract.Result<IList<ConfigurationAliasesEntity>>() != null);
			return null;
		}

		public ConfigurationAliasesEntity GetEmptyAspect()
		{
			Contract.Ensures(Contract.Result<ConfigurationAliasesEntity>() != null);
			return null;
		}
	}
}