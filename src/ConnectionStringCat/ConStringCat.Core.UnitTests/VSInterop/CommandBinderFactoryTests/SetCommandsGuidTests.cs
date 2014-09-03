using System;
using ConStringCat.Core.UnitTests.VSInterop.Utils;
using ConStringCat.Core.VSInterop;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop.CommandBinderFactoryTests
{
	[TestFixture]
	public class SetCommandsGuidTests : CommandBinderFactoryTests
	{
		private VSCommandBinder CreateOleMenuCommandBinder()
		{
			var callback = TestBinderCallback.CreateMock().Object;
			return Factory.BindToOleMenuCommand(TestCommandId,
				callback, TestBinderCallback.CallbackMethod(callback));
		}

		[Test]
		public void FactoryMethods_SetCommandsGuidNotInvokedYet_ShouldThrowAnException()
		{
			Factory = new CommandBinderFactory();
			Assert.That(() => CreateOleMenuCommandBinder(), Throws.Exception);
		}

		[Test]
		public void SetCommandsGuid_ValidGuid_ShouldCreateCommandsWithSpecifiedGuidThen()
		{
			var comsGuid = Guid.NewGuid();
			Factory.SetCommandsGuid(comsGuid);
			Assert.That(CreateOleMenuCommandBinder().NativeCommand.CommandID.Guid,
				Is.EqualTo(comsGuid));
		}
	}
}