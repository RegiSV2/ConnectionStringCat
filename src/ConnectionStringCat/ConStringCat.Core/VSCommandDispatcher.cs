using System.ComponentModel.Design;

namespace ConStringCat.Core
{
	public interface VSCommandDispatcher
	{
		MenuCommand NativeCommand { get; }
	}


}