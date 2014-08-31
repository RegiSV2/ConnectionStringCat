namespace ConStringCat.Core.UnitTests
{
	public interface ITestDispatcherCallback
	{
		string ExecuteSomeOperation(string argument);

		bool IsExecuted { get; }
	}
}