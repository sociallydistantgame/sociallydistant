using AcidicGui.Widgets;
using UI.Widgets;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Applications.Chat
{
	public class ChatBubbleWidget : SettingsWidget
	{
		public string? Text { get; set; }
		public bool UsePlayerColor { get; set; }
		
		/// <inheritdoc />
		public override WidgetController BuildSettingsWidget(SystemWidgets assembler, RectTransform destination)
		{
			ChatBubbleWidgetController controller = assembler.GetChatBubble(destination);
			controller.Setup(this);
			return controller;
		}
	}
}