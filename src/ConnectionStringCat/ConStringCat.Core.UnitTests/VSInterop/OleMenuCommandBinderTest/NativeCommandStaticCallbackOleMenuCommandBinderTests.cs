using ConStringCat.Core.UnitTests.VSInterop.Utils;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop.OleMenuCommandBinderTest
{
	[TestFixture]
	public class NativeCommandStaticCallbackOleMenuCommandBinderTests : NativeCommandOleMenuCommandBinderTests
	{
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
			CommandBinder.NativeCommand.Invoke(callbackArgument);
			//Assert
			Assert.That(callbackArgument.IsCallbackCalled);
		}
	}
}