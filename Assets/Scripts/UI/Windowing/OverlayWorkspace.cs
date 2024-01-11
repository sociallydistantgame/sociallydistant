#nullable enable
using System.Collections.Generic;
using Shell.Windowing;
using UnityEngine;
using System;

namespace UI.Windowing
{
	public class OverlayWorkspace : IWorkspaceDefinition
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
		public IFloatingGui CreateFloatingGui(string title)
		{
			IFloatingGui? win = workspace.CreateFloatingGui(title);

			win.WindowClosed += this.WindowClosed;
			
			return win;
		}
		
		/// <inheritdoc />
		public IReadOnlyList<IWindow> WindowList => workspace.WindowList;

		/// <inheritdoc />
		public IMessageDialog CreateMessageDialog(string title)
		{
			return null;
		}

		private void WindowClosed(IWindow win)
		{
			if (WindowList.Count > 0)
				return;
			
			this.Close();
		}
	}
}