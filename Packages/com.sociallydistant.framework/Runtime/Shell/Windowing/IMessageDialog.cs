#nullable enable

using System;
using System.Collections.Generic;

namespace Shell.Windowing
{
	public interface IMessageDialog : IWindow
	{
		string Message { get; set; }
		CommonColor Color { get; set; }

		IList<MessageBoxButtonData> Buttons { get; }

		Action<MessageDialogResult>? DismissCallback { get; set; }
		
		void Setup(IWindow window);
	}
}