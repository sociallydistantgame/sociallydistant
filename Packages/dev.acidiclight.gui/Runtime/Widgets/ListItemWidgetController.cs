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
		
		public bool Selected { get; set; }
		public ListWidget List { get; set; }
		public Action? Callback { get; set; }
		public string? Title { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			label.SetText(Title);
            
			toggle.group = List?.ToggleGroup;
			toggle.SetIsOnWithoutNotify(Selected);
			toggle.onValueChanged.AddListener(OnValueChanged);
		}

		private void Update()
		{
			// We do this here because we can't guarantee the toggle group will be instantiated by the time we
			// get built/removed from recycling.
			//
			// Cases where that's an issue:
			// - The user's using OSA (like in Socially Distant)
			// - The list is in a different widget section
			// - The list is further in the widget list than this list item
			this.toggle.group = List?.ToggleGroup;
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
			this.List = null;
		}
	}
}