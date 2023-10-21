using System;
using System.Linq;
using AcidicGui.Widgets;
using TMPro;
using UI.Themes.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applications.Chat
{
	public class ThemeColorSelectWidgetController : WidgetController
	{
		[SerializeField]
		private TextMeshProUGUI nameLabel = null!;

		[SerializeField]
		private TextMeshProUGUI descriptionLabel = null!;

		[Header("Fields")]
		[SerializeField]
		private RectTransform customField = null!;

		[SerializeField]
		private RectTransform namedField = null!;

		[SerializeField]
		private Toggle useCustomToggle = null!;

		[SerializeField]
		private TMP_Dropdown namedColorDropdown = null!;

		[SerializeField]
		private TMP_InputField hexField = null!;

		[SerializeField]
		private Image colorPreview = null!;
		
		public string ElementName { get; set; } = string.Empty;
		public string ElementDescription { get; set; } = string.Empty;
		public IThemeEditContext? EditContext { get; set; }
		public bool IsCustomColor { get; set; }
		public string ColorData { get; set; } = string.Empty;
		public Unity.Plastic.Newtonsoft.Json.Serialization.Action<string, bool>? Callback { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			nameLabel.SetText(ElementName);
			descriptionLabel.SetText(ElementDescription);

			UpdateFields();
			
			useCustomToggle.onValueChanged.AddListener(OnCustomColorToggle);
			namedColorDropdown.onValueChanged.AddListener(OnNamedColorChange);
			
			hexField.onDeselect.AddListener(OnHexSubmit);
			hexField.onSubmit.AddListener(OnHexSubmit);
			hexField.onValueChanged.AddListener(OnHexChanged);
		}

		private void OnHexChanged(string newHex)
		{
			if (!IsCustomColor)
				return;

			if (!newHex.StartsWith("#"))
				newHex = "#" + newHex;

			if (!ColorUtility.TryParseHtmlString(newHex, out Color color))
				return;

			colorPreview.color = color;
			ColorData = ColorUtility.ToHtmlStringRGBA(colorPreview.color);
			Callback?.Invoke(ColorData, IsCustomColor);
		}

		private void OnHexSubmit(string newHex)
		{
			if (!IsCustomColor)
				return;
			
			if (!newHex.StartsWith("#"))
				newHex = "#" + newHex;

			if (ColorUtility.TryParseHtmlString(newHex, out Color color))
				colorPreview.color = color;

			ColorData = ColorUtility.ToHtmlStringRGBA(colorPreview.color);
			Callback?.Invoke(ColorData, IsCustomColor);
			UpdateFields();
		}

		private void UpdateFields()
		{
			colorPreview.color = GetColor();
			
			useCustomToggle.SetIsOnWithoutNotify(IsCustomColor);
			customField.gameObject.SetActive(IsCustomColor);
			namedField.gameObject.SetActive(!IsCustomColor);

			hexField.SetTextWithoutNotify(ColorUtility.ToHtmlStringRGBA(colorPreview.color));

			if (IsCustomColor)
				return;

			string[] colorNames = EditContext?.GetColorNames()
			                      ?? Array.Empty<string>();
			
			int colorIndex = Array.IndexOf(colorNames, ColorData);
			
			// In cases where the user has deleted a named color but the color is still used by an element,
			// this will prevent data loss by pretending the deleted color still exists.
			//
			// The player will still need to re-add the color before it can be used, but we can at least
			// warn them of the missing named color.
			if (colorIndex == -1 && !string.IsNullOrWhiteSpace(ColorData))
			{
				var newArray = new string[colorNames.Length + 1];
				newArray[0] = ColorData;
				
				Array.Copy(colorNames, 0, newArray, 1, colorNames.Length);

				colorNames = newArray;
				colorIndex++;
			}
			
			// Case where the list of named colors is empty; we become a custom color.
			if (colorNames.Length == 0)
			{
				IsCustomColor = true;
				ColorData = ColorUtility.ToHtmlStringRGBA(colorPreview.color);
				Callback?.Invoke(ColorData, IsCustomColor);
				UpdateFields();
				return;
			}
			
			// Case where we don't have a valid color selected, select the first.
			if (colorIndex == -1)
			{
				colorIndex = 0;
				ColorData = colorNames[colorIndex];
				Callback?.Invoke(ColorData, IsCustomColor);
				UpdateFields();
				return;
			}
			
			namedColorDropdown.ClearOptions();
			namedColorDropdown.AddOptions(colorNames.ToList());
			namedColorDropdown.SetValueWithoutNotify(colorIndex);
		}

		private Color GetColor()
		{
			if (IsCustomColor)
			{
				string hex = this.ColorData;
				if (!hex.StartsWith("#"))
					hex = "#" + hex;

				ColorUtility.TryParseHtmlString(hex, out Color color);
				return color;
			}

			return EditContext?.GetNamedColor(ColorData, EditContext.UseDarkMode) ?? default;
		}
		
		/// <inheritdoc />
		public override void OnRecycle()
		{
			useCustomToggle.onValueChanged.RemoveAllListeners();
			namedColorDropdown.onValueChanged.RemoveAllListeners();
		}

		private void OnCustomColorToggle(bool shouldUseCustom)
		{
			IsCustomColor = shouldUseCustom;

			if (shouldUseCustom)
			{
				ColorData = ColorUtility.ToHtmlStringRGBA(colorPreview.color);
			}
			else
			{
				ColorData = EditContext?.GetColorNames().FirstOrDefault() ?? string.Empty;
			}
			
			Callback?.Invoke(ColorData, IsCustomColor);
			UpdateFields();
		}
		
		private void OnNamedColorChange(int index)
		{
			if (IsCustomColor)
				return;

			ColorData = namedColorDropdown.options[index].text;
            Callback?.Invoke(ColorData, IsCustomColor);
            UpdateFields();
		}
	}
}