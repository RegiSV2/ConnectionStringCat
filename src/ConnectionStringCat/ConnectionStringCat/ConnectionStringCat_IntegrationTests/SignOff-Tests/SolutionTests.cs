﻿using ConnectionStringCat_IntegrationTests.IntegrationTest_Library;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace ConnectionStringCat_IntegrationTests
{
	[TestClass]
	public class SolutionTests
	{
		#region properties

		/// <summary>
		///     Gets or sets the test context which provides
		///     information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		#endregion

		[TestMethod]
		[HostType("VS IDE")]
		public void CreateEmptySolution()
		{
			UIThreadInvoker.Invoke((ThreadInvoker) delegate
			{
				var testUtils = new TestUtils();
				testUtils.CloseCurrentSolution(__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave);
				testUtils.CreateEmptySolution(TestContext.TestDir, "EmptySolution");
			});
		}

		#region fields

		private delegate void ThreadInvoker();

		#endregion

		#region ctors

		#endregion
	}
}