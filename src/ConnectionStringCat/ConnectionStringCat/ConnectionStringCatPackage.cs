﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Autofac;
using ConStringCat.Core;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SergeyUskov.ConnectionStringCat
{
	/// <summary>
	///     This is the class that implements the package exposed by this assembly.
	///     The minimum requirement for a class to be considered a valid package for Visual Studio
	///     is to implement the IVsPackage interface and register itself with the shell.
	///     This package uses the helper classes defined inside the Managed Package Framework (MPF)
	///     to do it: it derives from the Package class that provides the implementation of the
	///     IVsPackage interface and uses the registration attributes defined in the framework to
	///     register itself and its components with the shell.
	/// </summary>
	// This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
	// a package.
	[PackageRegistration(UseManagedResourcesOnly = true)]
	// This attribute is used to register the information needed to show this package
	// in the Help/About dialog of Visual Studio.
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	// This attribute is needed to let the shell know that this package exposes some menus.
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(GuidList.guidConnectionStringCatPkgString)]
	public sealed class ConnectionStringCatPackage : Package
	{
		/// <summary>
		///     Default constructor of the package.
		///     Inside this method you can place any initialization code that does not require
		///     any Visual Studio service because at this point the package object is created but
		///     not sited yet inside Visual Studio environment. The place to do all the other
		///     initialization is the Initialize method.
		/// </summary>
		public ConnectionStringCatPackage()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
			IoC.Init();
		}

		#region Package Members

		/// <summary>
		///     Initialization of the package; this method is called right after the package is sited, so this is the place
		///     where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));
			base.Initialize();

			BindCommands();
		}

		private void BindCommands()
		{
			var mcs = GetOleMenuCommandService();
			if (null == mcs) return;

			foreach (var commandBinder in CreateCommandBindings(GetCommandFactory()))
			{
				mcs.AddCommand(commandBinder.NativeCommand);
			}
		}

		private IEnumerable<VSCommandBinder> CreateCommandBindings(CommandBinderFactory commandFactory)
		{

			yield return commandFactory.BindToMenuCommand((int) PkgCmdIdList.SetupConStringsCmdId, MenuItemCallback);
			yield return commandFactory.BindToOleMenuCommand((int) PkgCmdIdList.ConnectionStringsListId,
				this, () => new Func<string[]>(GetConnectionsStringList));
			yield return commandFactory.BindToOleMenuCommand((int) PkgCmdIdList.ConnectionStringsCombo,
				this, () => new Func<string, string>(ConStringsGetterSetter));

		}

		private OleMenuCommandService GetOleMenuCommandService()
		{
			var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
			return mcs;
		}

		private static CommandBinderFactory GetCommandFactory()
		{
			var commandFactory = IoC.Container.Resolve<CommandBinderFactory>();
			commandFactory.SetCommandsGuid(GuidList.guidConnectionStringCatCmdSet);
			return commandFactory;
		}

		#endregion

		private string[] GetConnectionsStringList()
		{
			return new[] {"Red", "Green", "Blue"};
		}

		private string ConStringsGetterSetter(string value)
		{
			if(value != null)
				_curValue = value;
			return _curValue;
		}

		private string _curValue = "Red";

		/////////////////////////////////////////////////////////////////////////////
		// Overridden Package Implementation

		/// <summary>
		///     This function is the callback used to execute a command when the a menu item is clicked.
		///     See the Initialize method to see how the menu item is associated to this function using
		///     the OleMenuCommandService service and the MenuCommand class.
		/// </summary>
		private void MenuItemCallback()
		{
			// Show a Message Box to prove we were here
			var uiShell = (IVsUIShell) GetService(typeof (SVsUIShell));
			Guid clsid = Guid.Empty;
			int result;
			ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
				0,
				ref clsid,
				"ConnectionStringCat",
				string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", ToString()),
				string.Empty,
				0,
				OLEMSGBUTTON.OLEMSGBUTTON_OK,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
				OLEMSGICON.OLEMSGICON_INFO,
				0, // false
				out result));
		}
	}
}