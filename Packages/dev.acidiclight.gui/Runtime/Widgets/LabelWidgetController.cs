using TMPro;
using UnityEngine;

namespace AcidicGui.Widgets
{
	public class LabelWidgetController : WidgetController
	{
		[SerializeField]
		private TextMeshProUGUI labelText = null!;

		[SerializeField]
		private LinkHelper linkHelper = null!;
		
		public string Text { get; set; } = string.Empty;
		public bool AllowHttpLinks { get; set; } = false;
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			linkHelper.AllowHttpLinks = AllowHttpLinks;
			labelText.SetText(ProcessLinks(Text));
		}

		private string ProcessLinks(string markup)
		{
			const string styleStart = @"<style=""Link"">";
			const string styleEnd = "</style>";

			const string linkEnd = "</link>";
			const string linkStart = "<link=";

			return markup.Replace(linkEnd, $"{linkEnd}{styleEnd}")
				.Replace(linkStart, $"{styleStart}{linkStart}");
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
		}
	}
}