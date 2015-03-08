using ConStringCat.Core.Model;

namespace ConStringCat.Core.VSInterop
{
	/// <summary>
	/// Loads connection string variants for specified solution file
	/// </summary>
	public interface VariantsSettingsLoader
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="solutionFileName"></param>
		/// <returns></returns>
		ConnectionStringVariantsSet LoadVariantsSetForSolution(string solutionFileName);

		ConnectionStringVariantsSet GetEmptyVariantsSet();
	}
}