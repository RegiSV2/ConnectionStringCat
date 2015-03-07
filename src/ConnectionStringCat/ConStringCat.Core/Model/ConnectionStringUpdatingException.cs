using System;
using System.Diagnostics.Contracts;

namespace ConStringCat.Core.Model
{
	/// <summary>
	/// Thrown when we didn't manage to update connection string value
	/// </summary>
	public class ConnectionStringUpdatingException : Exception
	{
		public ConnectionStringUpdatingException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public ConnectionStringUpdatingException(string message)
			:base(message)
		{
		}
	}
}
