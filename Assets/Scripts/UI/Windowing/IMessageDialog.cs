#nullable enable

using System;
using Architecture;

namespace UI.Windowing
{
	public interface IMessageDialog : ICloseable
	{
		string Title { get; set; }
		string Message { get; set; }
		MessageDialogIcon Icon { get; set; }

		ObservableList<MessageBoxButtonData> Buttons { get; }

		event Action<int>? ButtonPressed; 

		void Setup(IWindow window);
	}
}