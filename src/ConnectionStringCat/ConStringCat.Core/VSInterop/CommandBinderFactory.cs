using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConStringCat.Core.VSInterop
{
	public class CommandBinderFactory
	{
		private const string FactoryInitializedMsg = "You must set commandsGuid before calling factory methods";

		private Guid _commandsGuid;

		private bool _isInitialized;

		public VSCommandBinder BindToOleMenuCommand(int commandId,
			Expression<Func<Delegate>> callback)
		{
			Contract.Requires(callback != null);
			Contract.Requires(commandId != default(int));
			Contract.Assert(_isInitialized, FactoryInitializedMsg);

			var id = new CommandID(_commandsGuid, commandId);
			var callExpression = GetMethodCallExpression(callback);
			var callbackTarget = GetTargetFromExpression(callback, callExpression);
			var callbackMethod = GetMethodInfoFromExpression(callExpression);

			if (callbackTarget == null)
				return OleMenuCommandBinder.BindToStaticCallback(id, callbackMethod);
			return OleMenuCommandBinder.BindToInstanceCallback(id, callbackTarget, callbackMethod);
		}

		private static MethodInfo GetMethodInfoFromExpression(MethodCallExpression callback)
		{
			Debug.Assert(callback.Object is ConstantExpression);
			var constExpr = callback.Object as ConstantExpression;
			return constExpr.Value as MethodInfo;
		}

		private static MethodCallExpression GetMethodCallExpression(Expression<Func<Delegate>> callback)
		{
			var bodyExpr = callback.Body as UnaryExpression;
			var methodCallExpr = bodyExpr.Operand as MethodCallExpression;
			return methodCallExpr;
		}

		private static object GetTargetFromExpression(Expression<Func<Delegate>> callback,
			MethodCallExpression methodCallExpression)
		{
			var lambda = Expression.Lambda(methodCallExpression.Arguments.Last(),
				callback.Parameters);
			return lambda.Compile().DynamicInvoke(new object[0]);
		}

		public void SetCommandsGuid(Guid commandsGuid)
		{
			Contract.Requires(commandsGuid != default(Guid));
			_commandsGuid = commandsGuid;
			_isInitialized = true;
		}
	}
}