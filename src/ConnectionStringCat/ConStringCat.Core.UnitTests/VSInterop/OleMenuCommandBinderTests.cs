using ConStringCat.Core.UnitTests.VSInterop.Utils;
using ConStringCat.Core.VSInterop;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop
{
	[TestFixture]
	public class OleMenuCommandBinderTests : CommandBinderTestsBase
	{
		private Mock<ITestBinderCallback> _callback;

		private OleMenuCommandBinder _commandBinder;

		[SetUp]
		public void InitializeContext()
		{
			_callback = TestBinderCallback.CreateMock();
			_commandBinder = OleMenuCommandBinder.BindToInstanceCallback(CommandId,
				_callback.Object, TestBinderCallback.CallbackMethod());
		}

		[Test]
		public void NativeCommand_ShouldReturnAnOleMenuCommandObject()
		{
			Assert.That(_commandBinder.NativeCommand, Is.Not.Null);
		}

		[Test]
		public void CallbackShouldBeCalled_WhenNativeCommandInvokes()
		{
			//Act
			_commandBinder.NativeCommand.Invoke(TestBinderCallback.ConfiguredOpeartionArgument);
			//Assert
			Assert.That(_callback.Object.IsExecuted);
		}
	}
}