namespace ConStringCat.Core.Model
{
	/// <summary>
	///     Receives connection string values and updates something (files, for example) with received values
	/// </summary>
	public interface ConnectionStringUpdater
	{
		/// <summary>
		///     Sets new connection string value
		/// </summary>
		/// <exception cref="ConnectionStringUpdatingException">When didn't manage to update value</exception>
		void SetNewValue(string newValue);
	}
}