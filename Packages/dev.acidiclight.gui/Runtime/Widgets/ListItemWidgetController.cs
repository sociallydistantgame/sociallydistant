using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AcidicGui.Widgets
{
	public class ListItemWidgetController : WidgetController
	{
		[SerializeField]
		private Toggle toggle;

		[SerializeField]
		private TextMeshProUGUI label = null!;
		
		public ToggleGroup? Group { get; set; }
		public Action? Callback { get; set; }
		public string? Title { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			label.SetText(Title);
			
			toggle.group = Group;
			toggle.onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(bool newValue)
		{
			if (!newValue)
				return;

			Callback?.Invoke();
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			toggle.onValueChanged.RemoveAllListeners();
			
			toggle.group = null;
			Callback = null;
			this.Group = null;
		}
	}
}