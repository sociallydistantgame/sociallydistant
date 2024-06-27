#nullable enable

using System;
using AcidicGui.Widgets;
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
		private RectTransform headerRect = null!;
		
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

		[SerializeField]
		private AnimatedHighlight animatedHighlight = null!;
		
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

		public bool IsActive
		{
			get => animatedHighlight.IsActive;
			set => animatedHighlight.IsActive = value;
		}

		public bool ShowTitle
		{
			get => headerText.gameObject.activeSelf;
			set => headerRect.gameObject.SetActive(value);
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