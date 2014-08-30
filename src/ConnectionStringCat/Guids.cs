// Guids.cs
// MUST match guids.h
using System;

namespace SergeyUskov.ConnectionStringCat
{
    static class GuidList
    {
        public const string guidConnectionStringCatPkgString = "bf2c1368-abc7-4dd2-a198-8374f7a26974";
        public const string guidConnectionStringCatCmdSetString = "9bebfb67-4d8e-4275-b3a1-f29a81be5e2e";

        public static readonly Guid guidConnectionStringCatCmdSet = new Guid(guidConnectionStringCatCmdSetString);
    };
}