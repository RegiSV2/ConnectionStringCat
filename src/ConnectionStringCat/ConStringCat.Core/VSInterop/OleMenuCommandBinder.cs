using System;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace ConStringCat.Core.VSInterop
{
	public class OleMenuCommandBinder : VSCommandBinder
	{
		private static readonly IntPtr MinusOneIntPtr = new IntPtr(-1);

		private readonly MethodInfo _callback;
		private readonly object[] _callbackEmptyArgs;
		private readonly object _callbackTarget;

		public OleMenuCommandBinder(CommandID commandId, object callbackTarget, MethodInfo callback)
		{
			Contract.Requires(commandId != null);
			Contract.Requires(callbackTarget != null);
			Contract.Requires(callback != null);

			_callbackTarget = callbackTarget;
			_callback = callback;
			_callbackEmptyArgs = new object[callback.GetParameters().Length];
			NativeCommand = new OleMenuCommand(InvokeHandler, commandId);
		}

		public MenuCommand NativeCommand { get; private set; }

		private void InvokeHandler(object sender, EventArgs eventArgs)
		{
			var oleEventArgs = (OleMenuCmdEventArgs) eventArgs;
			object[] arguments = GetArguments(oleEventArgs);
			object result = _callback.Invoke(_callbackTarget, arguments);

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

		private object[] GetArguments(OleMenuCmdEventArgs oleEventArgs)
		{
			return oleEventArgs.InValue == null
				? _callbackEmptyArgs
				: new[] {oleEventArgs.InValue};
		}
	}
}