namespace Shell
{
	public interface ITabbedToolDefinition
	{
		IProgram Program { get; }
		bool AllowUserTabs { get; }
	}
}