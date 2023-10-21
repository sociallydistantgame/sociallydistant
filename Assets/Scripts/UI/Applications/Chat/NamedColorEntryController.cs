using AcidicGui.Widgets;
using TMPro;
using UI.Themes.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applications.Chat
{
	public class NamedColorEntryController : WidgetController
	{
		[SerializeField]
		private TextMeshProUGUI nameLabel = null!;

		[SerializeField]
		private TMP_InputField nameEditInput = null!;

		[SerializeField]
		private TMP_InputField darkInput = null!;

		[SerializeField]
		private TMP_InputField lightInput = null!;

		[SerializeField]
		private Image lightPreview = null!;

		[SerializeField]
		private Image darkPreview = null!;

		[SerializeField]
		private TextMeshProUGUI nameError = null!;

		[SerializeField]
		private Button deleteButton = null!;

		[SerializeField]
		private Button editNameButton = null!;
		
		private string newName = string.Empty;
		private bool isEditingName;
		private bool allowNameSubmit = false;
		
		public IThemeEditContext? EditContext { get; set; }
		public string ColorName { get; set; } = string.Empty;
		public Color DarkColor { get; set; }
		public Color LightColor { get; set; }
		public bool IsTemporary { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			CheckNewName();
			
			darkPreview.color = DarkColor;
			lightPreview.color = LightColor;
			
			lightInput.SetTextWithoutNotify(ColorUtility.ToHtmlStringRGBA(LightColor));
			darkInput.SetTextWithoutNotify(ColorUtility.ToHtmlStringRGBA(DarkColor));

			isEditingName = string.IsNullOrWhiteSpace(ColorName) || IsTemporary;
			
			nameLabel.SetText(ColorName);
			nameEditInput.SetTextWithoutNotify(ColorName);

			UpdateNameEditing();
			
			nameEditInput.onValueChanged.AddListener(OnNameChange);
			nameEditInput.onSubmit.AddListener(OnNameSubmit);
			
			lightInput.onValueChanged.AddListener(OnLightColorChange);
			darkInput.onValueChanged.AddListener(OnDarkColorChange);
			lightInput.onDeselect.AddListener(OnColorDeselect);
			darkInput.onDeselect.AddListener(OnColorDeselect);
			lightInput.onSubmit.AddListener(OnColorDeselect);
			darkInput.onSubmit.AddListener(OnColorDeselect);
			
			nameEditInput.onDeselect.AddListener(OnColorNameDeselect);
			
			deleteButton.onClick.AddListener(Delete);
			editNameButton.onClick.AddListener(StartRename);
		}

		private void StartRename()
		{
			isEditingName = true;
			UpdateNameEditing();
		}

		private void OnColorNameDeselect(string newNewName)
		{
			if (!isEditingName)
				return;

			newName = newNewName;
			CheckNewName();

			if (!allowNameSubmit)
			{
				if (IsTemporary)
				{
					EditContext?.CancelTemporaryColor();
					return;
				}

				newName = string.Empty;
				isEditingName = false;
				nameEditInput.SetTextWithoutNotify(ColorName);
				UpdateNameEditing();
				return;
			}
			
			OnNameSubmit(newNewName);
		}

		private void OnColorDeselect(string arg0)
		{
			OnLightColorChange(lightInput.text);
			OnDarkColorChange(darkInput.text);
			
            lightInput.SetTextWithoutNotify(ColorUtility.ToHtmlStringRGBA(LightColor));
			darkInput.SetTextWithoutNotify(ColorUtility.ToHtmlStringRGBA(DarkColor));
		}
		
		private void OnDarkColorChange(string color)
		{
			if (!color.StartsWith("#"))
				color = "#" + color;

			if (ColorUtility.TryParseHtmlString(color, out Color actualColor))
			{
				DarkColor = actualColor;
				darkPreview.color = DarkColor;
				
				EditContext?.UpdateColors(ColorName, DarkColor, LightColor);
			}
		}

		private void OnLightColorChange(string color)
		{
			if (!color.StartsWith("#"))
				color = "#" + color;

			if (ColorUtility.TryParseHtmlString(color, out Color actualColor))
			{
				LightColor = actualColor;
				lightPreview.color = LightColor;
				
				EditContext?.UpdateColors(ColorName, DarkColor, LightColor);
			}
		}

		private void UpdateNameEditing()
		{
			editNameButton.gameObject.SetActive(!isEditingName);
			nameEditInput.gameObject.SetActive(isEditingName);
			nameLabel.gameObject.SetActive(!isEditingName);

			if (isEditingName)
			{
				nameEditInput.ActivateInputField();
			}
		}
		
		private void OnNameSubmit(string newNewName)
		{
			if (!isEditingName)
				return;
			
			newName = newNewName;
			CheckNewName();

			if (!allowNameSubmit)
			{
				newName = string.Empty;
				nameEditInput.SetTextWithoutNotify(ColorName);

				if (IsTemporary)
					return;

				isEditingName = false;
				UpdateNameEditing();
			}

			if (EditContext == null)
				return;

			EditContext.RenameColor(ColorName, newName);

			if (IsTemporary)
				return;

			isEditingName = false;
			UpdateNameEditing();
		}

		private void Delete()
		{
			if (EditContext == null)
				return;

			if (IsTemporary)
				EditContext.CancelTemporaryColor();
			else
				EditContext.DeleteColor(ColorName);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			nameEditInput.onValueChanged.RemoveAllListeners();
			nameEditInput.onSubmit.RemoveAllListeners();
			lightInput.onValueChanged.RemoveAllListeners();
			darkInput.onValueChanged.RemoveAllListeners();
			lightInput.onDeselect.RemoveAllListeners();
			darkInput.onDeselect.RemoveAllListeners();
			nameEditInput.onDeselect.RemoveAllListeners();
			lightInput.onSubmit.RemoveAllListeners();
			darkInput.onSubmit.RemoveAllListeners();
			deleteButton.onClick.RemoveAllListeners();
			editNameButton.onClick.RemoveAllListeners();
			EditContext = null;
		}

		private void CheckNewName()
		{
			nameError.SetText(string.Empty);

			if (!isEditingName)
				return;
			
			if (string.IsNullOrWhiteSpace(newName))
			{
				if (IsTemporary)
					nameError.SetText("Please enter a name for the color");
				else
					nameError.SetText("Color name must not be blank or whitespace");
			}

			if (EditContext != null && newName != ColorName && EditContext.ColorWithNameExists(newName))
			{
				nameError.SetText("Another color with the same name exists!");
			}

			allowNameSubmit = string.IsNullOrWhiteSpace(nameError.text);
		}
		
		private void OnNameChange(string newNewName)
		{
			if (!isEditingName)
				return;
			
			this.newName = newNewName;
			CheckNewName();
		}
	}
}