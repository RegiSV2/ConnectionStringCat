using System;
using System.ComponentModel.Design;

namespace ConStringCat.Core.UnitTests
{
	public class CommandDispatcherTestsBase
	{
		protected CommandID CommandId = new CommandID(Guid.NewGuid(), new Random().Next());
	}
}