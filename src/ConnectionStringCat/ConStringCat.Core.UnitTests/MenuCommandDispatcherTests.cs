using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests
{
	[TestFixture]
	public class MenuCommandDispatcherTests : CommandDispatcherTestsBase
	{
		[Test]
		public void NativeCommand_ShouldReturnAMenuCommandObject()
		{
			//Arrange
			var commandDispatcher = new MenuCommandDispatcher(CommandId, () => { });
			//Assert
			Assert.That(commandDispatcher.NativeCommand, Is.Not.Null);
		}

		[Test]
		public void CallbackShouldBeCalled_WhenNativeCommandInvokes()
		{
			//Arrange
			var callbackIsCalled = false;
			var commandDispatcher = new MenuCommandDispatcher(CommandId, () => { callbackIsCalled = true; });
			//Act
			commandDispatcher.NativeCommand.Invoke();
			//Assert
			Assert.That(callbackIsCalled);
		}
	}
}
