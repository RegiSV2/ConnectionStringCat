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

		private OleMenuCommandBinder(CommandID commandId, object callbackTarget, MethodInfo callback)
		{
			_callbackTarget = callbackTarget;
			_callback = callback;
			_callbackEmptyArgs = new object[callback.GetParameters().Length];
			NativeCommand = new OleMenuCommand(InvokeHandler, commandId);
		}

		public MenuCommand NativeCommand { get; private set; }

		public static OleMenuCommandBinder BindToStaticCallback(CommandID commandId, MethodInfo staticCallback)
		{
			Contract.Requires(commandId != null);
			Contract.Requires(staticCallback != null);

			return new OleMenuCommandBinder(commandId, null, staticCallback);
		}

		public static OleMenuCommandBinder BindToInstanceCallback(CommandID commandId,
			object target, MethodInfo callback)
		{
			Contract.Requires(commandId != null);
			Contract.Requires(target != null);
			Contract.Requires(callback != null);

			return new OleMenuCommandBinder(commandId, target, callback);
		}

		private void InvokeHandler(object sender, EventArgs eventArgs)
		{
			var oleEventArgs = (OleMenuCmdEventArgs) eventArgs;
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

		private object[] GetArguments(OleMenuCmdEventArgs oleEventArgs)
		{
			return oleEventArgs.InValue == null
				? _callbackEmptyArgs
				: new[] {oleEventArgs.InValue};
		}
	}
}