using System;
using UnityEngine;

namespace UI.Windowing
{
	public interface IWindow<TWindowClient>
	{
		public event Action<IWindow<TWindowClient>>? WindowClosed; 

		string Title { get; set; }
		WindowState WindowState { get; set; }
		
		bool EnableCloseButton { get; set; }
		bool EnableMaximizeButton { get; }
		bool EnableMinimizeButton { get; }
		
		bool IsActive { get; }
		
		Vector2 MinimumSize { get; set; }
		Vector2 Position { get; set; }
		
		TWindowClient Client { get; }

		void SetClient(TWindowClient newClient);

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