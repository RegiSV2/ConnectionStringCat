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
