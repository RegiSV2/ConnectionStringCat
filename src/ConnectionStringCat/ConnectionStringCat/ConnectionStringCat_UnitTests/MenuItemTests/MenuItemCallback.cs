/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using SergeyUskov.ConnectionStringCat;

namespace ConnectionStringCat_UnitTests.MenuItemTests
{
	[TestClass]
	public class MenuItemTest
	{
		/// <summary>
		///     Verify that a new menu command object gets added to the OleMenuCommandService.
		///     This action takes place In the Initialize method of the Package object
		/// </summary>
		[TestMethod]
		public void InitializeMenuCommand()
		{
			// Create the package
			IVsPackage package = new ConnectionStringCatPackage();
			Assert.IsNotNull(package, "The object does not implement IVsPackage");

			// Create a basic service provider
			var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

			// Site the package
			Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

			//Verify that the menu command can be found
			var menuCommandID = new CommandID(GuidList.guidConnectionStringCatCmdSet, (int) PkgCmdIdList.SetupConStringsCmdId);
			var info = typeof (Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.IsNotNull(info);
			var mcs = info.Invoke(package, new object[] {(typeof (IMenuCommandService))}) as OleMenuCommandService;
			Assert.IsNotNull(mcs.FindCommand(menuCommandID));
		}
	}
}