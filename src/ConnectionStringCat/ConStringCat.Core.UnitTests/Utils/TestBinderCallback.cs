using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace ConStringCat.Core.UnitTests.Utils
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
	}
}
