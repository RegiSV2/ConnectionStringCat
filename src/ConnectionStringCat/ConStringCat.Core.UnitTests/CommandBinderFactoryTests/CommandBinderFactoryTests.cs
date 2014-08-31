﻿using System;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.CommandBinderFactoryTests
{
	public class CommandBinderFactoryTests
	{
		protected const int TestCommandId = 100;

		protected static readonly Action SimpleCallback = () => { };

		protected Guid CommandsGuid;

		protected CommandBinderFactory Factory;

		[SetUp]
		public virtual void InitializeContext()
		{
			CommandsGuid = Guid.NewGuid();
			Factory = new CommandBinderFactory();
			Factory.SetCommandsGuid(CommandsGuid);
		}
	}
}
