using ConStringCat.Core.Model;

namespace ConStringCat.Core.VSInterop
{
	/// <summary>
	/// Loads connection string variants for specified solution file
	/// </summary>
	public interface VariantsSettingsLoader
	{
		/// <summary>
		/// Loads ConnectionStringCat settings for specified solution file
		/// </summary>
		/// <param name="solutionFileName">A full or relative path to solution file</param>
		/// <returns>Fully initialized variants set</returns>
		/// <exception cref="VariantsSettingsLoadingException">Thrown when some error occured during settings loading</exception>
		ConnectionStringVariantsSet LoadVariantsSetForSolution(string solutionFileName);

		/// <summary>
		/// Returns empty set
		/// </summary>
		ConnectionStringVariantsSet GetEmptyVariantsSet();
	}
}