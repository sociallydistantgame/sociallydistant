using TMPro;
using UnityEngine;

namespace AcidicGui.Widgets
{
	public class SectionWidgetController : WidgetController
	{
		[SerializeField]
		private TextMeshProUGUI sectionText = null!;
		
		public string Text { get; set; } = string.Empty;

		/// <inheritdoc />
		public override void UpdateUI()
		{
			sectionText.SetText(Text);
		}

		/// <inheritdoc />
		public override void OnRecycle() {}
	}
}