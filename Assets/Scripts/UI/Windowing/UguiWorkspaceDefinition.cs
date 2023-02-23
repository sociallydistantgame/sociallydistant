using Architecture;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace UI.Windowing
{
	public class UguiWorkspaceDefinition : IClientWorkspaceDefinition<UguiWindow, RectTransform>
	{
		private readonly RectTransform workspaceArea;
		private readonly UguiWindow windowPrefab;
		
		/// <inheritdoc />
		public string Name { get; set; }

		/// <inheritdoc />
		public ObservableList<IWindow> WindowList { get; private set; } = new ObservableList<IWindow>();

		/// <inheritdoc />
		public UguiWindow CreateWindow(string title, RectTransform client)
		{
			UguiWindow newWindow = Object.Instantiate(windowPrefab, workspaceArea);
			newWindow.SetWorkspace(this);
			newWindow.Title = title;

			if (client != null)
				newWindow.SetClient(client);

			this.WindowList.Add(newWindow);
			newWindow.WindowClosed += HandleWindowClosed;

			return newWindow;
		}

		public IWindow CreateWindow(string title)
		{
			return CreateWindow(title, null);
		}
		
		private void HandleWindowClosed(IWindow win)
		{
			win.WindowClosed -= HandleWindowClosed;
			this.WindowList.Remove(win);
		}

		public UguiWorkspaceDefinition(RectTransform workspaceArea, UguiWindow windowPrefab)
		{
			this.workspaceArea = workspaceArea;
			this.windowPrefab = windowPrefab;
		}
	}
}