using System.Collections.Generic;
using Architecture;
using Shell.Windowing;
using UnityEngine;

namespace UI.Windowing
{
	public class UguiWorkspaceDefinition : IWorkspaceDefinition
	{
		private readonly RectTransform workspaceArea;
		private readonly UguiWindow windowPrefab;
		private readonly ObservableList<IWindow> windows = new ObservableList<IWindow>();

		/// <inheritdoc />
		public IReadOnlyList<IWindow> WindowList => windows;

		/// <inheritdoc />
		public IFloatingGui CreateFloatingGui(string title)
		{
			UguiWindow newWindow = Object.Instantiate(windowPrefab, workspaceArea);
			newWindow.SetWorkspace(this);
			newWindow.ActiveContent.Title = title;
			
			this.windows.Add(newWindow);
			newWindow.WindowClosed += HandleWindowClosed;

			return newWindow;
		}

		public IMessageDialog CreateMessageDialog(string title)
		{
			return null;
		}
		
		private void HandleWindowClosed(IWindow win)
		{
			win.WindowClosed -= HandleWindowClosed;
			this.windows.Remove(win);
		}

		public UguiWorkspaceDefinition(RectTransform workspaceArea, UguiWindow windowPrefab)
		{
			this.workspaceArea = workspaceArea;
			this.windowPrefab = windowPrefab;
		}
	}
}