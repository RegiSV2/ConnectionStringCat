using ConStringCat.Core.UnitTests.VSInterop.Utils;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop.OleMenuCommandBinderTest
{
	[TestFixture]
	public class NativeCommandInstanceCallbackOleMenuCommandBinderTests : NativeCommandOleMenuCommandBinderTests
	{
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
			CommandBinder.NativeCommand.Invoke(TestBinderCallback.ConfiguredOpeartionArgument);
			//Assert
			Assert.That(Callback.Object.IsExecuted);
		}
	}
}