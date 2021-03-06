﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Autofac;
using ConStringCat.Core.VSInterop;
using Microsoft.VisualStudio;
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
			Debug.WriteLine(CultureInfo.CurrentCulture, string.Format("Entering constructor for: {0}", ToString()));
		}

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
			var clsid = Guid.Empty;
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

		#region Package Members

		/// <summary>
		///     Initialization of the package; this method is called right after the package is sited, so this is the place
		///     where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			IoC.Init(GetService);
			var bindings = CreateCommandBindings();
			BindCommands(bindings);
		}

		private void BindCommands(IEnumerable<VSCommandBinder> commandBinders)
		{
			var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
			if (null == mcs) return;

			foreach (var commandBinder in commandBinders)
			{
				mcs.AddCommand(commandBinder.NativeCommand);
			}
		}

		private IEnumerable<VSCommandBinder> CreateCommandBindings()
		{
			var service = IoC.Container.Resolve<VariantsSetService>();
			var commandFactory = GetCommandFactory();

			yield return commandFactory.BindToOleMenuCommand((int) PkgCmdIdList.SetupConStringsCmdId,
				() => new Action(MenuItemCallback));

			var variantsComboBoxCommand = commandFactory.BindToOleMenuCommand((int)PkgCmdIdList.VariantsListId,
				() => new Func<string[]>(service.GetAspects));
			variantsComboBoxCommand.SetCommandAvailabilityChecker(
				() => service.IsServiceAvailable);
			yield return variantsComboBoxCommand;

			var variantsComboSetterCommand = commandFactory.BindToOleMenuCommand((int)PkgCmdIdList.VariantsCombo,
				() => new Func<string, string>(service.GetSetCurrentAspect));
			variantsComboSetterCommand.SetCommandAvailabilityChecker(
				() => service.IsServiceAvailable);
			yield return variantsComboSetterCommand;

			var comboBoxCommand = commandFactory.BindToOleMenuCommand((int) PkgCmdIdList.ConnectionStringsListId,
				() => new Func<string[]>(service.GetAliases));
			comboBoxCommand.SetCommandAvailabilityChecker(
				() => service.IsServiceAvailable);
			yield return comboBoxCommand;

			var comboSetterCommand = commandFactory.BindToOleMenuCommand((int) PkgCmdIdList.ConnectionStringsCombo,
				() => new Func<string, string>(service.GetSetCurrentVariant));
			comboSetterCommand.SetCommandAvailabilityChecker(
				() => service.IsServiceAvailable);
			yield return comboSetterCommand;
		}

		private static CommandBinderFactory GetCommandFactory()
		{
			var commandFactory = IoC.Container.Resolve<CommandBinderFactory>();
			commandFactory.SetCommandsGuid(GuidList.guidConnectionStringCatCmdSet);
			return commandFactory;
		}

		#endregion
	}
}