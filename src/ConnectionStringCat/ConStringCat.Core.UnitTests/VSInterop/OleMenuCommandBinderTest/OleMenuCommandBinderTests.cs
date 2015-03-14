using ConStringCat.Core.UnitTests.VSInterop.Utils;
using ConStringCat.Core.VSInterop;
using Microsoft.VisualStudio.Shell;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop.OleMenuCommandBinderTest
{
	public class OleMenuCommandBinderTests : CommandBinderTestsBase
	{
		protected Mock<ITestBinderCallback> Callback;
		protected OleMenuCommandBinder CommandBinder;

		protected void BuildBinderWithInstanceCallback()
		{
			Callback = TestBinderCallback.CreateMock();
			CommandBinder = OleMenuCommandBinder.BindToInstanceCallback(CommandId,
				Callback.Object, TestBinderCallback.CallbackMethod());
		}

		protected void BuildBinderWithStaticCallback()
		{
			CommandBinder = OleMenuCommandBinder.BindToStaticCallback(CommandId,
				TestBinderCallback.StaticCallbackMethodInfo());
		}
	}

	public class SetCommandAvailabilityCheckerOleMenuCommandBinderTests : OleMenuCommandBinderTests
	{
		private OleMenuCommand BinderNativeOleCommand
		{
			get { return (OleMenuCommand) CommandBinder.NativeCommand; }
		}

		[Test]
		public void SetCommandAvailabilityChecker_NullArgument_ShouldThrowAnException()
		{
			//Arrange
			BuildBinderWithInstanceCallback();
			//Assert
			Assert.That(() => CommandBinder.SetCommandAvailabilityChecker(null),
				Throws.Exception);
		}

		[Test]
		public void SetCommandAvailabilityChecker_NotNullArgument_CheckerShouldBeExecutedWhenCommandQueryStatusEventIsFired()
		{
			//Arrange
			BuildBinderWithInstanceCallback();
			const bool checkResult = false;
			CommandBinder.SetCommandAvailabilityChecker(() => checkResult);
			Assert.That(BinderNativeOleCommand.Enabled, Is.EqualTo(true));
			//Act
			var status = BinderNativeOleCommand.OleStatus;
			//Assert
			Assert.That(BinderNativeOleCommand.Enabled, Is.EqualTo(checkResult));
		}
	}
}