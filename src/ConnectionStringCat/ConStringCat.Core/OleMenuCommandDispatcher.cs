using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace ConStringCat.Core
{
	public class OleMenuCommandDispatcher : VSCommandDispatcher
	{
		private static readonly object[] CallbackEmptyArgs = new object[0];

		private static readonly IntPtr MinusOneIntPtr = new IntPtr(-1);

		private readonly object _callbackTarget;

		private readonly MethodInfo _callback;

		public MenuCommand NativeCommand { get; private set; }

		public OleMenuCommandDispatcher(CommandID commandId, object callbackTarget, MethodInfo callback)
		{
			_callbackTarget = callbackTarget;
			_callback = callback;
			NativeCommand = new OleMenuCommand(InvokeHandler, commandId);
		}

		private void InvokeHandler(object sender, EventArgs eventArgs)
		{
			var oleEventArgs = (OleMenuCmdEventArgs)eventArgs;
			var arguments = GetArguments(oleEventArgs);
			var result = _callback.Invoke(_callbackTarget, arguments);

			if (ShouldProvideResult(oleEventArgs))
			{
				Marshal.GetNativeVariantForObject(result, oleEventArgs.OutValue);
			}
			
		}

		private static bool ShouldProvideResult(OleMenuCmdEventArgs oleEventArgs)
		{
			return oleEventArgs.OutValue != IntPtr.Zero
				&& oleEventArgs.OutValue != MinusOneIntPtr;
		}

		private static object[] GetArguments(OleMenuCmdEventArgs oleEventArgs)
		{
			var arguments = oleEventArgs.InValue == null
				? CallbackEmptyArgs
				: new[] {oleEventArgs.InValue};
			return arguments;
		}
	}
}