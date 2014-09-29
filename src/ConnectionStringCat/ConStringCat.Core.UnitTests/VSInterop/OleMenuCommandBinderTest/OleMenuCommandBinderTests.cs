using System;
using ConStringCat.Core.UnitTests.VSInterop.Utils;
using Microsoft.VisualStudio.Shell;
using Moq;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop.OleMenuCommandBinderTest
{
	public class OleMenuCommandBinderTests : CommandBinderTestsBase
	{
		protected Mock<ITestBinderCallback> Callback;

		protected Core.VSInterop.OleMenuCommandBinder CommandBinder;

		protected void BuildBinderWithInstanceCallback()
		{
			Callback = TestBinderCallback.CreateMock();
			CommandBinder = Core.VSInterop.OleMenuCommandBinder.BindToInstanceCallback(CommandId,
				Callback.Object, TestBinderCallback.CallbackMethod());
		}

		protected void BuildBinderWithStaticCallback()
		{
			CommandBinder = Core.VSInterop.OleMenuCommandBinder.BindToStaticCallback(CommandId,
				TestBinderCallback.StaticCallbackMethodInfo());
		}
	}

	public class SetCommandAvailabilityCheckerOleMenuCommandBinderTests : OleMenuCommandBinderTests
	{
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

		private OleMenuCommand BinderNativeOleCommand
		{
			get { return (OleMenuCommand) CommandBinder.NativeCommand; }
		}
	}
}