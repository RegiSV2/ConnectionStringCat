using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ConStringCat.Core;
using ConStringCat.Core.VSInterop;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace SergeyUskov.ConnectionStringCat
{
	public static class IoC
	{
		public static IContainer Container { get; private set; }

		public static void Init(Func<Type, object> getNativeService)
		{
			var builder = new ContainerBuilder();
			RegisterTypes(builder, getNativeService);
			Container = builder.Build();
		}

		private static void RegisterTypes(ContainerBuilder builder,
			Func<Type, object> getNativeService)
		{
			builder.RegisterType<CommandBinderFactory>();
			builder.Register(_ => getNativeService(typeof (DTE)) as DTE);
			builder.RegisterType<VariantsSetServiceImpl>().As<VariantsSetService>();
		}
	}
}
