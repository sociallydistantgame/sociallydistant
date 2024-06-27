using TMPro;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Applications.Chat
{
	public class GuildHeaderWidgetController : SettingsWidgetController<GuildHeaderWidget>
	{
		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI guildNameText = null!;

		[SerializeField]
		private TextMeshProUGUI serverStatsText = null!;
		
		private string? guildName;
		private int memberCount;
		private int channelCount;

		/// <inheritdoc />
		public override void UpdateUI()
		{
			guildName = Widget.Name;
			memberCount = Widget.MemberCount;
			channelCount = Widget.ChannelCount;
			
			guildNameText.SetText(guildName);
			serverStatsText.SetText($"<b>{memberCount}</b> members  <b>{channelCount}</b> channels");
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
		}
	}
}