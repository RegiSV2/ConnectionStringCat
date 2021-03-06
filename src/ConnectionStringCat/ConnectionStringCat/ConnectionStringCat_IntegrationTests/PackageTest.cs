﻿using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using SergeyUskov.ConnectionStringCat;

namespace ConnectionStringCat_IntegrationTests
{
	/// <summary>
	///     Integration test for package validation
	/// </summary>
	[TestClass]
	public class PackageTest
	{
		/// <summary>
		///     Gets or sets the test context which provides
		///     information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		[TestMethod]
		[HostType("VS IDE")]
		public void PackageLoadTest()
		{
			UIThreadInvoker.Invoke((ThreadInvoker) delegate
			{
				//Get the Shell Service
				var shellService = VsIdeTestHostContext.ServiceProvider.GetService(typeof (SVsShell)) as IVsShell;
				Assert.IsNotNull(shellService);

				//Validate package load
				IVsPackage package;
				var packageGuid = new Guid(GuidList.guidConnectionStringCatPkgString);
				Assert.IsTrue(0 == shellService.LoadPackage(ref packageGuid, out package));
				Assert.IsNotNull(package, "Package failed to load");
			});
		}

		private delegate void ThreadInvoker();
	}
}