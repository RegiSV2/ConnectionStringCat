using System;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;

namespace ConStringCat.Core.VSInterop
{
	/// <summary>
	///     Binds Vusual Studio Commands to business logic methods
	/// </summary>
	[ContractClass(typeof (VSCommandBinderContacts))]
	public interface VSCommandBinder
	{
		/// <summary>
		///     Returns a native command, assotiated to this binder
		/// </summary>
		MenuCommand NativeCommand { get; }


	}

	[ContractClassFor(typeof (VSCommandBinder))]
	internal abstract class VSCommandBinderContacts : VSCommandBinder
	{
		public MenuCommand NativeCommand
		{
			get
			{
				Contract.Ensures(Contract.Result<MenuCommand>() != null);
				return null;
			}
		}
	}
}