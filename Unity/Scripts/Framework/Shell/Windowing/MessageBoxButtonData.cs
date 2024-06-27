#nullable enable
using System;

namespace Shell.Windowing
{
	public struct MessageBoxButtonData
	{
		public string Text;
		public Action? ClickHandler;
		public MessageDialogResult Result;
		
		public static implicit operator MessageBoxButtonData(string text)
		{
			return new MessageBoxButtonData
			{
				Result = MessageDialogResult.Ok,
				Text = text
			};
		}
	}
}