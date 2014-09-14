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

		private VSCommandBinder InstantiateBinderWithInstanceCallback()
		{
			return Factory
				.BindToOleMenuCommand(TestCommandId, 
				TestBinderCallback.CallbackMethod(_binderCallback.Object));
		}

		private VSCommandBinder InstantiateBinderWithStaticCallback()
		{
			return Factory
				.BindToOleMenuCommand(TestCommandId, TestBinderCallback.StaticCallbackMethod());
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
			Assert.That(InstantiateBinderWithInstanceCallback(), Is.Not.Null);
		}

		[Test]
		public void BindToOleMenuCommand_CorrectArgument_BindersNativeCommandShouldBeAnOleCommand()
		{
			Assert.That(InstantiateBinderWithInstanceCallback(), Is.AssignableTo<OleMenuCommandBinder>());
		}

		[Test]
		public void BindToOleMenuCommand_CorrectArgument_BindersNativeCommandShouldExecuteCallback()
		{
			//Act
			var binder = InstantiateBinderWithInstanceCallback();
			binder.NativeCommand.Invoke(TestBinderCallback.ConfiguredOpeartionArgument);
			//Assert
			Assert.That(_binderCallback.Object.IsExecuted);
		}

		[Test]
		public void BindToOleMenuCommand_StaticCallback_ShouldReturnNewBinder()
		{
			Assert.That(InstantiateBinderWithStaticCallback(), Is.Not.Null);
		}

		[Test]
		public void BindToOleMenuCommand_ZeroId_ShouldThrowException()
		{
			var callbackTarget = TestBinderCallback.CreateMock().Object;
			Assert.That(() => Factory.BindToOleMenuCommand(
				0, TestBinderCallback.CallbackMethod(callbackTarget)),
				Throws.Exception);
		}

		[Test]
		public void BindToOleMenuCommand_NullCallback_ShouldThrowException()
		{
			Assert.That(() => Factory.BindToOleMenuCommand(TestCommandId, null),
				Throws.Exception);
		}
	}
}
