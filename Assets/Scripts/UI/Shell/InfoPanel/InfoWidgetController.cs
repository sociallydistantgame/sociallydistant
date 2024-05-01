#nullable enable
using System;
using System.Text;
using Com.TheFallenGames.OSA.Util;
using Shell.InfoPanel;
using TMPro;
using UnityEngine;
using UnityExtensions;
using Utility;
using UnityEngine.UI;

namespace UI.Shell.InfoPanel
{
	public class InfoWidgetController : MonoBehaviour
	{
		private InfoWidgetData data;

		[Header("Dependencies")]
		[SerializeField]
		private InfoPanelService infoPanelService = null!;
		
		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI textIcon = null!;

		[SerializeField]
		private TextMeshProUGUI text = null!;

		[SerializeField]
		private Button closeButton = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(InfoWidgetController));
		}

		private void Start()
		{
			closeButton.onClick.AddListener(OnClose);
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

			closeButton.gameObject.SetActive(data.CreationData.Closeable);
		}

		private void OnClose()
		{
			infoPanelService.CloseWidget(data.Id);
		}
	}
}