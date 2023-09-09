#nullable enable
using System.Collections.Generic;
using Shell.Windowing;
using UnityEngine;
using System;

namespace UI.Windowing
{
	public class OverlayWorkspace : IClientWorkspaceDefinition<UguiWindow, RectTransform>
	{
		private readonly GameObject go;
		private readonly UguiWorkspaceDefinition workspace;

		public event Action? Closed;
		
		public OverlayWorkspace(GameObject go, UguiWorkspaceDefinition workspace)
		{
			this.go = go;
			this.workspace = workspace;
		}

		public void Close()
		{
			while (WindowList.Count > 0)
				WindowList[0].Close();

			Closed?.Invoke();
			UnityEngine.Object.Destroy(go);
		}
		
		/// <inheritdoc />
		public UguiWindow CreateWindow(string title, RectTransform? client = default)
		{
			UguiWindow? win = workspace.CreateWindow(title, client);

			win.WindowClosed += this.WindowClosed;
			
			return win;
		}

		/// <inheritdoc />
		public string Name
		{
			get => go.name;
			set => go.name = value;
		}

		/// <inheritdoc />
		public IReadOnlyList<IWindow> WindowList => workspace.WindowList;

		/// <inheritdoc />
		public IWindow CreateWindow(string title)
		{
			return CreateWindow(title, null);
		}

		private void WindowClosed(IWindow win)
		{
			if (WindowList.Count > 0)
				return;
			
			this.Close();
		}
	}
}