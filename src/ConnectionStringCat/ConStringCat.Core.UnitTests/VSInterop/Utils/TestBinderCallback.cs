using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Moq;

namespace ConStringCat.Core.UnitTests.VSInterop.Utils
{
	public static class TestBinderCallback
	{
		public const string ConfiguredOpeartionArgument = "some string";

		public static Mock<ITestBinderCallback> CreateMock()
		{
			var binderCallback = new Mock<ITestBinderCallback>();
			binderCallback.Setup(x => x.ExecuteSomeOperation(ConfiguredOpeartionArgument))
				.Callback(
					() => binderCallback.SetupGet(x => x.IsExecuted).Returns(true));
			return binderCallback;
		}

		public static Expression<Func<Delegate>> CallbackMethod(ITestBinderCallback callback)
		{
			return () => new Func<string, string>(callback.ExecuteSomeOperation);
		}

		public static MethodInfo CallbackMethod()
		{
			return typeof (ITestBinderCallback).GetMethod("ExecuteSomeOperation");
		}

		public static Expression<Func<Delegate>> StaticCallbackMethod()
		{
			return () => new Action<StaticCallbackArgument>(StaticCallback);
		}

		public static MethodInfo StaticCallbackMethodInfo()
		{
			return typeof (TestBinderCallback).GetMethod("StaticCallback",
				BindingFlags.NonPublic | BindingFlags.Static);
		}

		private static void StaticCallback(StaticCallbackArgument argument)
		{
			argument.IsCallbackCalled = true;
			Debug.WriteLine("In test static callback method, argument:" + argument);
		}
	}
}