using TMPro;
using UnityEngine;

namespace AcidicGui.Widgets
{
	public class LabelWidgetController : WidgetController
	{
		[SerializeField]
		private TextMeshProUGUI labelText = null!;

		public string Text { get; set; } = string.Empty;
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			labelText.SetText(Text);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
		}
	}
}