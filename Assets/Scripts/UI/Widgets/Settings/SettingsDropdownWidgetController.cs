using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public class SettingsDropdownWidgetController : SettingsWidgetController<SettingsDropdownWidget>
	{
		[SerializeField]
		private TextMeshProUGUI titleText = null!;
		
		[SerializeField]
		private TextMeshProUGUI descriptionText = null!;

		[SerializeField]
		private TMP_Dropdown dropdown = null!;

		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public int CurrentIndex { get; set; }
		public string[] Choices { get; set; } = Array.Empty<string>();
		public Action<int>? Callback { get; set; }

		/// <inheritdoc />
		public override void Setup(SettingsDropdownWidget widget)
		{
			this.Title = widget.Title;
			this.Description = widget.Description;
			this.CurrentIndex = widget.CurrentIndex;
			this.Choices = widget.Choices;
			this.Callback = widget.Callback;
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			this.titleText.SetText(Title);
			this.descriptionText.SetText(Description);

			this.dropdown.options.Clear();

			this.dropdown.options.AddRange(Choices.Select(x => new TMP_Dropdown.OptionData(x)));

			this.dropdown.value = CurrentIndex;
			
			this.dropdown.onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(int newIndex)
		{
			this.CurrentIndex = newIndex;
			this.Callback?.Invoke(newIndex);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			Callback = null;
			dropdown.onValueChanged.RemoveAllListeners();
		}
	}
}