using System;
using System.ComponentModel.Design;

namespace ConStringCat.Core.UnitTests
{
	public class CommandBinderTestsBase
	{
		protected readonly CommandID CommandId = new CommandID(Guid.NewGuid(), new Random().Next());
	}
}