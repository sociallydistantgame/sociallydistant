#nullable enable

using System;
using System.Collections.Generic;

namespace Shell.Windowing
{
	public interface IMessageDialog : ICloseable
	{
		string Title { get; set; }
		string Message { get; set; }
		MessageDialogIcon Icon { get; set; }

		IList<MessageBoxButtonData> Buttons { get; }

		event Action<int>? ButtonPressed; 

		void Setup(IWindow window);
	}
}