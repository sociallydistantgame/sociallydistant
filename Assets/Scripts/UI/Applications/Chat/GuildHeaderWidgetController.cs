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
		public override void Setup(GuildHeaderWidget widget)
		{
			guildName = widget.Name;
			memberCount = widget.MemberCount;
			channelCount = widget.ChannelCount;
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			guildNameText.SetText(guildName);
			serverStatsText.SetText($"<b>{memberCount}</b> members  <b>{channelCount}</b> channels");
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
		}
	}
}