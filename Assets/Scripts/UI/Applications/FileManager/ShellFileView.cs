#nullable enable
using System;
using TMPro;
using UI.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;
using UnityEngine.UI;

namespace UI.Applications.FileManager
{
	public class ShellFileView : 
		MonoBehaviour,
		IPointerClickHandler
	{
		[SerializeField]
		private CompositeIconWidget icon = null!;

		[SerializeField]
		private TextMeshProUGUI text = null!;

		private Button button;
		private string path = string.Empty;
		
		public Action<string>? Callback { get; set; }
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ShellFileView));
			this.MustGetComponent(out button);
		}

		public void UpdateData(ShellFileModel data)
		{
			this.path = data.Path;
            
			icon.Icon = data.Icon;
			text.SetText(data.Name);
		}

		/// <inheritdoc />
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (eventData.clickCount != 2)
				return;
			
			Callback?.Invoke(path);
		}
	}
}