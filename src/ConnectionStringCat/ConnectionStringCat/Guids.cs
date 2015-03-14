// Guids.cs
// MUST match guids.h

using System;

namespace SergeyUskov.ConnectionStringCat
{
	internal static class GuidList
	{
		public const string guidConnectionStringCatPkgString = "8d165b01-e0c7-48c4-a99f-beb0f25dda96";
		public const string guidConnectionStringCatCmdSetString = "ae366413-f8dd-4be1-8e7f-5b3219828d78";
		public static readonly Guid guidConnectionStringCatCmdSet = new Guid(guidConnectionStringCatCmdSetString);
	};
}