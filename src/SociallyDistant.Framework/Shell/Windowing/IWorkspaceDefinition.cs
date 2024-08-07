﻿namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IWorkspaceDefinition
	{
		IReadOnlyList < IWindow > WindowList { get; }

		IFloatingGui CreateFloatingGui(string title);
	}
}