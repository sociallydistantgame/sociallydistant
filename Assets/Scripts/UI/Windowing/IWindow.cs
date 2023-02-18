using System;
using UnityEngine;

namespace UI.Windowing
{
	public interface IWindow
	{
		public event Action<IWindow>? WindowClosed; 

		string Title { get; set; }
		WindowState WindowState { get; set; }
		
		bool EnableCloseButton { get; set; }
		bool EnableMaximizeButton { get; }
		bool EnableMinimizeButton { get; }
		
		bool IsActive { get; }
		
		Vector2 MinimumSize { get; set; }
		Vector2 Position { get; set; }

		void ToggleMaximize();
		void Restore();
		void Minimize();
		void Close();
	}

	public interface IWindowCloseBlocker
	{
		bool CheckCanClose();
	}
}