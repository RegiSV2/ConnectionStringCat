using Microsoft.VisualStudio.Shell;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.VSInterop.OleMenuCommandBinderTest
{
	public class NativeCommandOleMenuCommandBinderTests : OleMenuCommandBinderTests
	{
		protected void AssertThatNativeCommandIsCorrect()
		{
			Assert.That(CommandBinder.NativeCommand, Is.Not.Null);
			Assert.That(CommandBinder.NativeCommand, Is.InstanceOf<OleMenuCommand>());
		}
	}
}