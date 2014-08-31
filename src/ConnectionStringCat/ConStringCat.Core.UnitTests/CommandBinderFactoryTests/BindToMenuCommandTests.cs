using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.CommandBinderFactoryTests
{
	[TestFixture]
	public class BindToMenuCommandTests : CommandBinderFactoryTests
	{
		[Test]
		public void BindToMenuCommand_CorrectArguments_ShouldReturnNewBinder()
		{
			//Arrange
			var createdBinder = Factory.BindToMenuCommand(TestCommandId, SimpleCallback);
			//Assert
			Assert.That((object) createdBinder, Is.Not.Null);
		}

		[Test]
		public void BindToMenuCommand_CorrectArguments_BinderNativeCommandShouldHaveCorrectId()
		{
			//Arrange
			var menuCommand = Factory.BindToMenuCommand(TestCommandId, SimpleCallback)
				.NativeCommand;
			//Assert
			Assert.That(menuCommand.CommandID.Guid == CommandsGuid
			            && menuCommand.CommandID.ID == TestCommandId);
		}

		[Test]
		public void BindMenuCommand_CorrectArguments_BinderNativeCommandShouldExecuteCallback()
		{
			//Arrange
			var callbackIsCalled = false;
			var menuCommand = Factory.BindToMenuCommand(TestCommandId,
				() => { callbackIsCalled = true; })
				.NativeCommand;
			//Act
			menuCommand.Invoke();
			//Assert
			Assert.That(callbackIsCalled);
		}

		[Test]
		public void BindToMenuCommand_ZeroId_ShouldThrowException()
		{
			Assert.That(() => Factory.BindToMenuCommand(0, SimpleCallback), Throws.Exception);
		}

		[Test]
		public void BindToMenuCommand_NullCallback_ShouldThrowException()
		{
			Assert.That(() => Factory.BindToMenuCommand(TestCommandId, null), Throws.Exception);
		}
	}
}