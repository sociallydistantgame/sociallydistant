using Core.Config;
using Core.Config.SystemConfigCategories;
using GamePlatform;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UI.Animation;
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
		private Rectangle bubble = null!;
		
		private string? text;
		private bool usePlayerColor;

		[SerializeField]
		private Color npcColor;

		[SerializeField]
		private Color playerColor;
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			ISettingsManager settings = GameManager.Instance.SettingsManager;
			var accessibilitySettings = new AccessibilitySettings(settings);
			
			text = Widget.Text;
			usePlayerColor = Widget.UsePlayerColor;
			
			label.SetText(text);

			Color borderColor = usePlayerColor ? playerColor : npcColor;
			Color backgroundColor = borderColor * (accessibilitySettings.UseChatBubbleOutlines ? 0f : 0.65f);

			this.bubble.ShapeProperties.OutlineColor = borderColor;
			this.bubble.ShapeProperties.FillColor = backgroundColor;

			bubble.ForceMeshUpdate();
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			label.SetText(string.Empty);
		}
	}
}