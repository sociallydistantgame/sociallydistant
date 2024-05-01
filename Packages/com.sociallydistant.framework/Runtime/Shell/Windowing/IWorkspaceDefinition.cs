using System.Collections.Generic;

namespace Shell.Windowing
{
	public interface IWorkspaceDefinition
	{
		IReadOnlyList < IWindow > WindowList { get; }

		IFloatingGui CreateFloatingGui(string title);
		IMessageDialog CreateMessageDialog(string title);
	}
}