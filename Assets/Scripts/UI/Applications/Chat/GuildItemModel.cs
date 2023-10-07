using Social;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applications.Chat
{
	public class GuildItemModel
	{
		public Texture2D GuildIcon { get; set; }
		public ToggleGroup ToggleGroup { get; set; }
		public IGuild? Guild { get; set; }
		public bool IsSelected { get; set; }
	}
}