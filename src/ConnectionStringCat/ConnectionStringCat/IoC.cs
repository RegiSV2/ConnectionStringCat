using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ConStringCat.Core;

namespace SergeyUskov.ConnectionStringCat
{
	public static class IoC
	{
		public static IContainer Container { get; private set; }

		public static void Init()
		{
			var builder = new ContainerBuilder();
			RegisterTypes(builder);
			Container = builder.Build();
		}

		private static void RegisterTypes(ContainerBuilder builder)
		{
			builder.RegisterType<CommandBinderFactory>();
		}
	}
}
