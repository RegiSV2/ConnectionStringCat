namespace ConStringCat.Core.UnitTests
{
	public interface ITestBinderCallback
	{
		string ExecuteSomeOperation(string argument);

		bool IsExecuted { get; }
	}
}