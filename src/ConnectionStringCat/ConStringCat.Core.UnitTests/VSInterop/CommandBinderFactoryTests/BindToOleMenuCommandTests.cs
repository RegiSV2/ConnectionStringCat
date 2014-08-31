using ConStringCat.Core.UnitTests.VSInterop.Utils;
using ConStringCat.Core.VSInterop;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop.CommandBinderFactoryTests
{
	[TestFixture]
	public class BindToOleMenuCommandTests : CommandBinderFactoryTests
	{
		private Mock<ITestBinderCallback> _binderCallback;

		private VSCommandBinder InstantiateBinderFromFactory()
		{
			return Factory
				.BindToOleMenuCommand(TestCommandId, _binderCallback.Object,
					TestBinderCallback.CallbackMethod(_binderCallback.Object));
		}

		[SetUp]
		public override void InitializeContext()
		{
			base.InitializeContext();
			_binderCallback = TestBinderCallback.CreateMock();
		}

		[Test]
		public void BindToOleMenuCommand_CorrectArguments_ShouldReturnNewBinder()
		{
			Assert.That(InstantiateBinderFromFactory(), Is.Not.Null);
		}

		[Test]
		public void BindToOleMenuCommand_CorrectArgument_BindersNativeCommandShouldBeAnOleCommand()
		{
			Assert.That(InstantiateBinderFromFactory(), Is.AssignableTo<OleMenuCommandBinder>());
		}

		[Test]
		public void BindToOleMenuCommand_CorrectArgument_BindersNativeCommandShouldExecuteCallback()
		{
			//Act
			var binder = InstantiateBinderFromFactory();
			binder.NativeCommand.Invoke(TestBinderCallback.ConfiguredOpeartionArgument);
			//Assert
			Assert.That(_binderCallback.Object.IsExecuted);
		}

		[Test]
		public void BindToOleMenuCommand_ZeroId_ShouldThrowException()
		{
			var callbackTarget = TestBinderCallback.CreateMock().Object;
			Assert.That(() => Factory.BindToOleMenuCommand(
				0, callbackTarget, TestBinderCallback.CallbackMethod(callbackTarget)),
				Throws.Exception);
		}

		[Test]
		public void BindToOleMenuCommand_NullCallbackTarget_ShouldThrowException()
		{
			var callbackTarget = TestBinderCallback.CreateMock().Object;
			Assert.That(() => Factory.BindToOleMenuCommand(
				0, null, TestBinderCallback.CallbackMethod(callbackTarget)),
				Throws.Exception);
		}

		[Test]
		public void BindToOleMenuCommand_NullCallback_ShouldThrowException()
		{
			Assert.That(() => Factory.BindToOleMenuCommand(
				TestCommandId, TestBinderCallback.CreateMock().Object, null), Throws.Exception);
		}
	}
}
