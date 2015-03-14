using System.ComponentModel.Design;
using System.Globalization;
using ConnectionStringCat_IntegrationTests.IntegrationTest_Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using SergeyUskov.ConnectionStringCat;

namespace ConnectionStringCat_IntegrationTests
{
	[TestClass]
	public class MenuItemTest
	{
		/// <summary>
		///     Gets or sets the test context which provides
		///     information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		///     A test for lauching the command and closing the associated dialogbox
		/// </summary>
		[TestMethod]
		[HostType("VS IDE")]
		public void LaunchCommand()
		{
			UIThreadInvoker.Invoke((ThreadInvoker) delegate
			{
				var menuItemCmd = new CommandID(GuidList.guidConnectionStringCatCmdSet, (int) PkgCmdIdList.SetupConStringsCmdId);

				// Create the DialogBoxListener Thread.
				var expectedDialogBoxText = string.Format(CultureInfo.CurrentCulture, "{0}\n\nInside {1}.MenuItemCallback()",
					"ConnectionStringCat", "SergeyUskov.ConnectionStringCat.ConnectionStringCatPackage");
				var purger = new DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText);

				try
				{
					purger.Start();

					var testUtils = new TestUtils();
					testUtils.ExecuteCommand(menuItemCmd);
				}
				finally
				{
					Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The dialog box has not shown");
				}
			});
		}

		private delegate void ThreadInvoker();
	}
}