using ConStringCat.Core.UnitTests.VSInterop.Utils;
using ConStringCat.Core.VSInterop;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop
{
	[TestFixture]
	public class MenuCommandBinderTests : CommandBinderTestsBase
	{
		[Test]
		public void NativeCommand_ShouldReturnAMenuCommandObject()
		{
			//Arrange
			var commandBinder = new MenuCommandBinder(CommandId, () => { });
			//Assert
			Assert.That(commandBinder.NativeCommand, Is.Not.Null);
		}

		[Test]
		public void CallbackShouldBeCalled_WhenNativeCommandInvokes()
		{
			//Arrange
			var callbackIsCalled = false;
			var commandBinder = new MenuCommandBinder(CommandId, () => { callbackIsCalled = true; });
			//Act
			commandBinder.NativeCommand.Invoke();
			//Assert
			Assert.That(callbackIsCalled);
		}
	}
}
