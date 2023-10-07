using TMPro;
using UI.Widgets.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applications.Chat
{
	public class ChatBubbleWidgetController : SettingsWidgetController<ChatBubbleWidget>
	{
		[SerializeField]
		private TextMeshProUGUI label = null!;

		[SerializeField]
		private Image bubble = null!;
		
		private string? text;
		private bool usePlayerColor;
		
		/// <inheritdoc />
		public override void Setup(ChatBubbleWidget widget)
		{
			text = widget.Text;
			usePlayerColor = widget.UsePlayerColor;
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			label.SetText(text);

			bubble.color = usePlayerColor ? Color.black : Color.gray;
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			label.SetText(string.Empty);
		}
	}
}