namespace ConStringCat.Core.UnitTests.VSInterop.Utils
{
	public interface ITestBinderCallback
	{
		bool IsExecuted { get; }
		string ExecuteSomeOperation(string argument);
	}
}