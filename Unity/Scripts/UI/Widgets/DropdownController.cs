#nullable enable
using System;
using AcidicGui.Widgets;
using TMPro;
using UnityEngine;

namespace UI.Widgets
{
	public sealed class DropdownController : WidgetController
	{
		
		private TMP_Dropdown dropdown = null!;
		
		public int CurrentIndex { get; set; } = -1;
		public string[] Choices { get; set; } = Array.Empty<string>();
		public Action<int>? Callback { get; set; }

		private void Awake()
		{
			dropdown.onValueChanged.AddListener(OnItemselected);
		}

		private void OnItemselected(int index)
		{
			CurrentIndex = index;
			Callback?.Invoke(index);
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			dropdown.ClearOptions();

			foreach (string choice in Choices)
			{
				dropdown.options.Add(new TMP_Dropdown.OptionData(choice));
			}

			dropdown.SetValueWithoutNotify(CurrentIndex);
			dropdown.captionText.SetText(Choices[CurrentIndex]);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			Callback = null;
		}
	}
}