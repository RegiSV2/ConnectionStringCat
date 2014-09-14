using ConStringCat.Core.UnitTests.VSInterop.Utils;
using ConStringCat.Core.VSInterop;
using Microsoft.VisualStudio.Shell;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop
{
	[TestFixture]
	public class OleMenuCommandBinderTests : CommandBinderTestsBase
	{
		private Mock<ITestBinderCallback> _callback;

		private OleMenuCommandBinder _commandBinder;

		[Test]
		public void NativeCommand_InstanceCallback_ShouldReturnAnOleMenuCommandObject()
		{
			//Arrange
			BuildBinderWithInstanceCallback();
			//Assert
			AssertThatNativeCommandIsCorrect();
		}

		[Test]
		public void NativeCommand_InstanceCallback_CallbackShouldBeCalledWhenNativeCommandInvokes()
		{
			//Arrange
			BuildBinderWithInstanceCallback();
			//Act
			_commandBinder.NativeCommand.Invoke(TestBinderCallback.ConfiguredOpeartionArgument);
			//Assert
			Assert.That(_callback.Object.IsExecuted);
		}

		[Test]
		public void NativeCommand_StaticCallback_ShouldReturnAnOleMenuCommandObject()
		{
			//Arrange
			BuildBinderWithStaticCallback();
			//Assert
			AssertThatNativeCommandIsCorrect();
		}

		[Test]
		public void NativeCommand_StaticCallback_CallbackShouldBeCalledWhenNativeCommandInvokes()
		{
			//Arrange
			BuildBinderWithStaticCallback();
			var callbackArgument = new StaticCallbackArgument();
			//Act
			_commandBinder.NativeCommand.Invoke(callbackArgument);
			//Assert
			Assert.That(callbackArgument.IsCallbackCalled);
		}

		private void BuildBinderWithInstanceCallback()
		{
			_callback = TestBinderCallback.CreateMock();
			_commandBinder = OleMenuCommandBinder.BindToInstanceCallback(CommandId,
				_callback.Object, TestBinderCallback.CallbackMethod());
		}

		private void BuildBinderWithStaticCallback()
		{
			_commandBinder = OleMenuCommandBinder.BindToStaticCallback(CommandId,
				TestBinderCallback.StaticCallbackMethodInfo());
		}

		private void AssertThatNativeCommandIsCorrect()
		{
			Assert.That(_commandBinder.NativeCommand, Is.Not.Null);
			Assert.That(_commandBinder.NativeCommand, Is.InstanceOf<OleMenuCommand>());
		}
	}
}