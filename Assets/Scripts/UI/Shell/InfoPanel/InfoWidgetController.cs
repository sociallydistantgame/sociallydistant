#nullable enable
using System;
using System.Text;
using TMPro;
using UnityEngine;
using Utility;

namespace UI.Shell.InfoPanel
{
	public class InfoWidgetController : MonoBehaviour
	{
		private InfoWidgetData data;

		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI textIcon = null!;

		[SerializeField]
		private TextMeshProUGUI text = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(InfoWidgetController));
		}

		public void SetData(InfoWidgetData data)
		{
			this.data = data;
			this.UpdateUI();
		}

		private void UpdateUI()
		{
			this.textIcon.SetText(data.CreationData.Icon);

			var sb = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(data.CreationData.Title))
			{
				sb.Append("<b>");
				sb.Append(data.CreationData.Title);
				sb.Append("</b>");
			}

			if (!string.IsNullOrWhiteSpace(data.CreationData.Text))
			{
				if (sb.Length > 0)
					sb.AppendLine();
				sb.Append(data.CreationData.Text);
			}
			
			this.text.SetText(sb);
		}
	}
}