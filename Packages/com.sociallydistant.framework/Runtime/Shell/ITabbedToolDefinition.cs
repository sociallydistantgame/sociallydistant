using ContentManagement;

namespace Shell
{
	public interface ITabbedToolDefinition : IGameContent
	{
		IProgram Program { get; }
		bool AllowUserTabs { get; }
	}
}