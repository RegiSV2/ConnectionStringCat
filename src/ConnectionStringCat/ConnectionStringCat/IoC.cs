using System;
using Autofac;
using ConStringCat.Core.SettingsManagement;
using ConStringCat.Core.VSInterop;
using EnvDTE;

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
			builder.RegisterType<VariantsSettingsLoaderImpl>().As<VariantsSettingsLoader>();
			builder.RegisterType<VariantsSetServiceImpl>().As<VariantsSetService>();
		}
	}
}