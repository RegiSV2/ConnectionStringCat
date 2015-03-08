using System;

namespace ConStringCat.Core.VSInterop
{
	/// <summary>
	/// An exception that indicates some error occured during variants settings loading
	/// </summary>
	public class VariantsSettingsLoadingException : Exception
	{
		public VariantsSettingsLoadingException(string message)
			:base(message)
		{ }

		public VariantsSettingsLoadingException(string message, Exception innerException)
			:base(message, innerException)
		{ }
	}
}