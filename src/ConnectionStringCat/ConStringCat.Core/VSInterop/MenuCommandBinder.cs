using System;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;

namespace ConStringCat.Core.VSInterop
{
	public class MenuCommandBinder : VSCommandBinder
	{
		public MenuCommandBinder(CommandID commandId, Action callback)
		{
			Contract.Requires(commandId != null);
			Contract.Requires(callback != null);
			NativeCommand = new MenuCommand((s, e) => callback(), commandId);
		}

		public MenuCommand NativeCommand { get; private set; }
	}
}