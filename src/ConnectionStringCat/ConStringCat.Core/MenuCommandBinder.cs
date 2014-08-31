using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConStringCat.Core
{
	public class MenuCommandBinder : VSCommandBinder
	{
		public MenuCommand NativeCommand { get; private set; }

		public MenuCommandBinder(CommandID commandId, Action callback)
		{
			Contract.Requires(commandId != null);
			Contract.Requires(callback != null);
			NativeCommand = new MenuCommand((s, e) => callback(), commandId);
		}
	}
}
