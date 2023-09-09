#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace AcidicGui.Components
{
	public class ListItemWithHeader : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		private Button activator = null!;

		[SerializeField]
		private TextMeshProUGUI headerText = null!;

		[SerializeField]
		private TextMeshProUGUI itemText = null!;

		[SerializeField]
		private string? title;

		[SerializeField]
		private string value = string.Empty;

		public string Value
		{
			get => value;
			set
			{
				if (this.value == value)
					return;

				this.value = value;

				if (itemText == null)
					return;
				
				itemText.SetText(value);
			}
		}
		
		public string? Title
		{
			get => title;
			set
			{
				if (title == value)
					return;
				
				title = value;

				if (headerText == null)
					return;
				
				headerText.enabled = !string.IsNullOrWhiteSpace(title);
				headerText.SetText(title);
			}
		}

		public Button Activator => activator;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ListItemWithHeader));
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (itemText == null)
				return;

			if (headerText == null)
				return;

			headerText.enabled = !string.IsNullOrWhiteSpace(title);
			headerText.SetText(title);
            itemText.SetText(value);
		}
		#endif
	}
}