#nullable enable
using System.Text;
using AcidicGui.Widgets;
using Shell.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets.Settings
{
	public sealed class SettingsFieldController : WidgetController
	{
		
		private TextMeshProUGUI text;

		
		private HorizontalLayoutGroup layout = null!;
		
		
		private CompositeIconWidget iconWidget = null!;

		
		private RectTransform slot = null!;
		
		public string Title { get; set; }
		public string Description { get; set; }
		public bool UseReverseLayout { get; set; }
		public CompositeIcon Icon { get; set; }
		public WidgetController? SlotWidget { get; set; }
		
		public RectTransform Slot => slot;

		private void UpdateText()
		{
			var builder = new StringBuilder();
			builder.Append("<b>");
			builder.Append(Title);
			builder.AppendLine("</b>");
			builder.Append(Description);
			
			this.text.SetText(builder);
		}
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			UpdateText();

			this.layout.reverseArrangement = UseReverseLayout;

			this.iconWidget.Icon = Icon;

			if (this.SlotWidget != null)
			{
				this.SlotWidget.UpdateUI();
				this.SlotWidget.gameObject.SetActive(true);
			}
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			if (SlotWidget != null)
			{
				SlotWidget.gameObject.SetActive(false);
				SlotWidget.OnRecycle();
			}
		}
	}
}