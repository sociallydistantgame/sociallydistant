using AcidicGui.Widgets;
using UI.Widgets;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Applications.Chat
{
	public class GuildHeaderWidget : SettingsWidget
	{
		public string? Name { get; set; }
		public int MemberCount { get; set; }
		public int ChannelCount { get; set; }
		
		/// <inheritdoc />
		public override WidgetController BuildSettingsWidget(SystemWidgets assembler, RectTransform destination)
		{
			GuildHeaderWidgetController controller = assembler.GetGuildHeader(destination);
			controller.Setup(this);
			return controller;
		}
	}
}