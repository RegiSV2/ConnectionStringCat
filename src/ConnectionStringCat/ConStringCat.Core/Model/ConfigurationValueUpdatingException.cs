using System;

namespace ConStringCat.Core.Model
{
	/// <summary>
	///     Thrown when we didn't manage to update connection string value
	/// </summary>
	public class ConfigurationValueUpdatingException : Exception
	{
		public ConfigurationValueUpdatingException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public ConfigurationValueUpdatingException(string message)
			: base(message)
		{
		}
	}
}