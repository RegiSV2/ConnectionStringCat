namespace ConStringCat.Core.UnitTests.VSInterop.Utils
{
	public interface ITestBinderCallback
	{
		string ExecuteSomeOperation(string argument);

		bool IsExecuted { get; }
	}
}