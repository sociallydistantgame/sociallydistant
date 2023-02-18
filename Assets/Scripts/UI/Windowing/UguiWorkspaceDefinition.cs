using Architecture;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace UI.Windowing
{
	public class UguiWorkspaceDefinition : IWorkspaceDefinition<RectTransform>
	{
		private readonly RectTransform workspaceArea;
		private readonly UguiWindow windowPrefab;
		
		/// <inheritdoc />
		public string Name { get; set; }

		/// <inheritdoc />
		public ObservableList<IWindow<RectTransform>> WindowList { get; private set; } = new ObservableList<IWindow<RectTransform>>();

		/// <inheritdoc />
		public IWindow<RectTransform> CreateWindow(string title, RectTransform client = default)
		{
			UguiWindow newWindow = Object.Instantiate(windowPrefab, workspaceArea);

			newWindow.Title = title;

			if (client != null)
				newWindow.SetClient(client);

			this.WindowList.Add(newWindow);

			return newWindow;
		}

		public UguiWorkspaceDefinition(RectTransform workspaceArea, UguiWindow windowPrefab)
		{
			this.workspaceArea = workspaceArea;
			this.windowPrefab = windowPrefab;
		}
	}
}