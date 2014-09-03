using System;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace ConStringCat.Core.VSInterop
{
	public class CommandBinderFactory
	{
		private const string FactoryInitializedMsg = "You must set commandsGuid before calling factory methods";
		private Guid _commandsGuid;

		private bool _isInitialized;

		public VSCommandBinder BindToOleMenuCommand(int commandId, object callbackTarget,
			Expression<Func<Delegate>> callback)
		{
			Contract.Requires(callbackTarget != null);
			Contract.Requires(callback != null);
			Contract.Requires(commandId != default(int));
			Contract.Assert(_isInitialized, FactoryInitializedMsg);

			return new OleMenuCommandBinder(new CommandID(_commandsGuid, commandId),
				callbackTarget, GetMethodInfoFromExpression(callback));
		}

		private static MethodInfo GetMethodInfoFromExpression(Expression<Func<Delegate>> callback)
		{
			var bodyExpr = callback.Body as UnaryExpression;
			var methodCallExpr = bodyExpr.Operand as MethodCallExpression;
			var constExpr = methodCallExpr.Object as ConstantExpression;
			var methodInfo = constExpr.Value as MethodInfo;
			return methodInfo;
		}

		public void SetCommandsGuid(Guid commandsGuid)
		{
			Contract.Requires(commandsGuid != default(Guid));
			_commandsGuid = commandsGuid;
			_isInitialized = true;
		}
	}
}