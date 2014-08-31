using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConStringCat.Core
{
	public class MenuCommandDispatcher : VSCommandDispatcher
	{
		public MenuCommand NativeCommand { get; private set; }

		public MenuCommandDispatcher(CommandID commandId, Action callback)
		{
			NativeCommand = new MenuCommand((s, e) => callback(), commandId);
		}
	}
}
