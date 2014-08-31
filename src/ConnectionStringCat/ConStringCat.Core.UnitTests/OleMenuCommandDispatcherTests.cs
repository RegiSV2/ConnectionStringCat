using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests
{
	[TestFixture]
	public class OleMenuCommandDispatcherTests : CommandDispatcherTestsBase
	{
		[Test]
		public void NativeCommand_ShouldReturnAnOleMenuCommandObject()
		{
			//Arrange
			var callback = new Mock<ITestDispatcherCallback>();
			var methodInfo = callback.Object.GetType()
				.GetMethod("ExecuteSomeOperation");
			var commandDispatcher = new OleMenuCommandDispatcher(CommandId,
				callback.Object, methodInfo);
			//Assert
			Assert.That(commandDispatcher.NativeCommand, Is.Not.Null);
		}

		[Test]
		public void CallbackShouldBeCalled_WhenNativeCommandInvokes()
		{
			//Arrange
			var callback = new Mock<ITestDispatcherCallback>();
			callback.Setup(x => x.ExecuteSomeOperation(It.IsAny<string>()))
				.Callback(() => callback.SetupGet(x => x.IsExecuted).Returns(true));
			var methodInfo = callback.Object.GetType()
				.GetMethod("ExecuteSomeOperation");
			var commandDispatcher = new OleMenuCommandDispatcher(CommandId,
				callback.Object, methodInfo);
			//Act
			commandDispatcher.NativeCommand.Invoke("someStr");
			Assert.That(callback.Object.IsExecuted);
		}
	}
}