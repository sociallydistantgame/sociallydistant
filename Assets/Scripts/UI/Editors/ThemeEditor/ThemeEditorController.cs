#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcidicGui.Common;
using AcidicGui.Widgets;
using Codice.Client.GameUI.Update;
using GamePlatform;
using Modding;
using PlasticGui.WorkspaceWindow.CodeReview;
using TMPro;
using UI.Applications.Chat;
using UI.PlayerUI;
using UI.Themes.Importers;
using UI.Themes.Serialization;
using UI.Themes.ThemedElements;
using UI.Theming;
using UI.UiHelpers;
using UI.Widgets;
using UI.Widgets.Settings;
using UnityEngine;
using UnityExtensions;
using UnityEngine.UI;
using ButtonWidget = AcidicGui.Widgets.ButtonWidget;

namespace UI.Editors.ThemeEditor
{
	public class ThemeEditorController : 
		MonoBehaviour,
		IThemeEditContext
	{
		private static readonly string useDarkModeKey = "themeEditor.useDarkModeForPreview";
		
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;

		[Header("UI")]
		[SerializeField]
		private ShellThemePreview themePreview = null!;
		
		[SerializeField]
		private WidgetList categoryWidgetList = null!;

		[SerializeField]
		private WidgetList editorWidgetList = null!;

		[SerializeField]
		private Button createEmptyButton = null!;

		[SerializeField]
		private TextMeshProUGUI cloneTitle = null!;
		
		[SerializeField]
		private TextMeshProUGUI cloneDescription = null!;

		[SerializeField]
		private RawImage cloneImage = null!;
		
		[SerializeField]
		private Button cloneThemeButton = null!;

		[SerializeField]
		private WidgetList previewWidgetList = null!;
		
		[Header("Toolbar")]
		[SerializeField]
		private Button newButton = null!;

		[SerializeField]
		private Button openButton = null!;

		[SerializeField]
		private Button saveButton = null!;

		[SerializeField]
		private Button saveAsButton = null!;

		[SerializeField]
		private Button uploadButton = null!;
		
		
		[Header("Pages")]
		[SerializeField]
		private VisibilityController newThemePage = null!;

		[SerializeField]
		private VisibilityController cloneThemePage = null!;

		[SerializeField]
		private VisibilityController backdropPage = null!;

		[SerializeField]
		private VisibilityController desktopPreview = null!;

		[SerializeField]
		private VisibilityController windowPreview = null!;

		[SerializeField]
		private VisibilityController widgetPreviewPage = null!;

		[Header("Settings")]
		[SerializeField]
		private Toggle darkModeToggle = null!;
        
		private readonly List<EditableNamedColor> namedColors = new List<EditableNamedColor>();

		private UiManager uiManager = null!;
		private EditableNamedColor? temporaryColor;
		private bool saving = false;
		private bool hasUnsavedChanges = false;
		private bool useDarkMode = false;
		private OperatingSystemTheme.ThemeEditor? themeEditor = null;
		private EditorMode editorMode;
		private IThemeAsset? themeToClone;
		private IThemeAsset? themeToOpen;
		private DialogHelper dialogHlper = null!;

		/// <inheritdoc />
		public bool UseDarkMode
		{
			get => useDarkMode;
			set
			{
				useDarkMode = value;
				UpdateWidgets();
			}
		}
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ThemeEditorController));
			this.MustGetComponent(out dialogHlper);
			this.MustGetComponentInParent(out uiManager);
		}

		private void Start()
		{
			if (gameManager.Value != null)
				useDarkMode = gameManager.Value.SettingsManager.GetBool(useDarkModeKey, false);
			
			darkModeToggle.SetIsOnWithoutNotify(useDarkMode);
			darkModeToggle.onValueChanged.AddListener(ToggleDarkPreview);
			createEmptyButton.onClick.AddListener(CreateEmptyTheme);
			cloneThemeButton.onClick.AddListener(CreateEmptyTheme);
			
			newButton.onClick.AddListener(OnNewClicked);
			saveButton.onClick.AddListener(async () =>
			{
				await Save();
			});
			
			saveAsButton.onClick.AddListener(async () =>
			{
				await SaveAs();
			});
			
			
			
			SetEditorMode(EditorMode.NewTheme);
			BuildPreviewWidgets();
			RefreshPreview();
		}

		private void ToggleDarkPreview(bool dark)
		{
			this.UseDarkMode = dark;
			UpdateWidgets();
			RefreshPreview();

			if (gameManager.Value != null)
			{
				gameManager.Value.SettingsManager.SetBool(useDarkModeKey, useDarkMode);
				gameManager.Value.SettingsManager.Save();
			}
		}
		
		private void ResetEditableColors()
		{
			namedColors.Clear();

			if (themeEditor == null)
				return;

			namedColors.AddRange(themeEditor.Colors.Keys.Select(
				key => new EditableNamedColor
				{
					name = key,
					dark = themeEditor.Colors[key].darkColor,
					light = themeEditor.Colors[key].lightColor
				}
			));
		}
		
		private async void CreateEmptyTheme()
		{
			if (themeToOpen != null)
			{
				OperatingSystemTheme theme = await themeToOpen.LoadAsync();
				themeEditor = theme.GetUserEditor();
				ResetEditableColors();
				SetEditorMode(EditorMode.ThemeInfo);
				return;
			}
			
			if (themeToClone == null)
			{
				themeEditor = OperatingSystemTheme.CreateEmpty(true, true);
				hasUnsavedChanges = true;
			}
			else
			{
				// Load the theme
				OperatingSystemTheme theme = await themeToClone.LoadAsync();

				// This is where we must save the copied theme
				string themePath = UserThemeSource.GetNewThemePath(theme.Name);
                
				// Extract the theme to the new path. We don't want to edit the existing theme
				await theme.ExportAsync(themePath);
				
				// Now we can load the new theme and it'll be editable!
				var storage = new BasicThemeResourceStorage();
				using ThemeLoader loader = ThemeLoader.FromFile(themePath, true, true);

				OperatingSystemTheme newTheme = await loader.LoadThemeAsync(storage);

				this.themeEditor = newTheme.GetUserEditor();
			}

			ResetEditableColors();
			SetEditorMode(EditorMode.ThemeInfo);
		}
		
		private void SetEditorMode(EditorMode mode)
		{
			this.editorMode = mode;
			this.UpdateWidgets();
		}

		private void UpdateWidgets()
		{
			UpdateCategoryWidgets();
			UpdateEditorWidgets();
			RefreshPreview();
			UpdateToolbar();
		}

		private void UpdateToolbar()
		{
			newButton.enabled = themeEditor != null && !saving;
			openButton.enabled = newButton.enabled;
			saveButton.enabled = openButton.enabled && hasUnsavedChanges;
			saveAsButton.enabled = openButton.enabled;
			uploadButton.enabled = saveAsButton.enabled;
		}

		private void UpdateClonePage()
		{
			var sb = new StringBuilder();
			
			if (themeToOpen != null)
			{
				cloneImage.texture = themeToOpen.PreviewImage;
				cloneTitle.SetText(themeToOpen.Name);
				
				sb.Append("<b>Theme author:</b> ");
				sb.AppendLine(themeToOpen.Author);
				sb.AppendLine();

				sb.Append(themeToOpen.Description);
			
				this.cloneDescription.SetText(sb);
				
				this.cloneThemeButton.MustGetComponentInChildren<TextMeshProUGUI>()
					.SetText("Open theme");
				
				return;
			}
			
			if (themeToClone == null)
				return;

			cloneImage.texture = themeToClone.PreviewImage;
			cloneTitle.SetText(themeToClone.Name);

			sb.Append("<b>Theme author:</b> ");
			sb.AppendLine(themeToClone.Author);
			sb.AppendLine();

			sb.Append(themeToClone.Description);
			
			this.cloneDescription.SetText(sb);
			
			this.cloneThemeButton.MustGetComponentInChildren<TextMeshProUGUI>()
				.SetText("Create new theme");
		}
		
		private void RefreshPreview()
		{
			if (themeEditor != null)
				themePreview.SetPreviewTheme(themeEditor.Theme, this.useDarkMode);
			else
				themePreview.SetPreviewTheme(uiManager.CurrentTheme, uiManager.UseDarkMode);
			
			newThemePage.Hide();
			cloneThemePage.Hide();
			backdropPage.Hide();
			desktopPreview.Hide();
			windowPreview.Hide();
			widgetPreviewPage.Hide();

			switch (editorMode)
			{
				case EditorMode.NewTheme when themeToClone == null:
					newThemePage.Show();
					break;
				case EditorMode.NewTheme when themeToClone != null:
					cloneThemePage.Show();
					UpdateClonePage();
					break;
				case EditorMode.OpenTheme when themeToOpen != null:
					cloneThemePage.Show();
					UpdateClonePage();
					break;
				case EditorMode.ThemeInfo:
					break;
				case EditorMode.Backdrop:
					backdropPage.Show();
					break;
				case EditorMode.Shell:
					backdropPage.Show();
					desktopPreview.Show();
					break;
				case EditorMode.Windows:
					backdropPage.Show();
					windowPreview.Show();
					break;
				case EditorMode.Typography:
				case EditorMode.Widgets:
					widgetPreviewPage.Show();
					break;
				case EditorMode.Terminal:
					break;
				case EditorMode.Colors:
					widgetPreviewPage.Show();
					break;
			}
		}

		private void MarkDirty()
		{
			hasUnsavedChanges = true;
			UpdateToolbar();

			RefreshPreview();
		}
		
		private void UpdateEditorWidgets()
		{
			switch (editorMode)
			{
				case EditorMode.NewTheme:
					SetupNewThemeWidgets();
					break;
				case EditorMode.OpenTheme:
					SetupOpenThemeWidgets();
					break;
				case EditorMode.ThemeInfo:
					SetupThemeInfo();
					break;
				case EditorMode.Typography:
					SetupTypography();
					break;
				case EditorMode.Backdrop:
					SetupBackdrop();
					break;
				case EditorMode.Shell:
					SetupShell();
					break;
				case EditorMode.Windows:
					SetupWindows();
					break;
				case EditorMode.Widgets:
					SetupWidgets();
					break;
				case EditorMode.Terminal:
					SetupTerminal();
					break;
				case EditorMode.Colors:
					SetupColors();
					break;
			}
		}

		private async void OpenShiftOSSkin()
		{
			string skinPath = await this.dialogHlper.OpenFile("Open ShiftOS Skin", Environment.CurrentDirectory, "ShiftOS skin file|*.skn");

			if (string.IsNullOrWhiteSpace(skinPath))
				return;

			try
			{
				OperatingSystemTheme theme = await ShiftOSSkinFactory.ImportTheme(skinPath);
				themeEditor = theme.GetUserEditor();
				hasUnsavedChanges = true;
				ResetEditableColors();
				SetEditorMode(EditorMode.ThemeInfo);
			}
			catch (Exception ex)
			{
				this.dialogHlper.ShowMessage("Skin import error", $"An error has occurred while importing the selected ShiftOS skin. {ex.Message} - more information in the game's log.", null, null);
				
				Debug.LogException(ex);
			}
		}
		
		private void SetupColors()
		{
			var builder = new WidgetBuilder();
			builder.Begin();

			builder.AddSection("Colors", out SectionWidget section);

			builder.AddLabel("Use this section to add, edit, and delete colors that you can use repeatedly throughout the theme. You can use this to give frequent colors a name, and they will appear inside color pickers.", section);

			var colorCount = 0;
			foreach (EditableNamedColor color in namedColors)
			{
				builder.AddWidget(new NamedColorEntry
				{
					ColorName = color.name,
					DarkColor = color.dark,
					LightColor = color.light,
					EditContext = this,
					IsTemporary = false
				}, section);
				
				colorCount++;
			}

			if (temporaryColor != null)
			{
				builder.AddWidget(new NamedColorEntry
				{
					ColorName = temporaryColor.name,
					DarkColor = temporaryColor.dark,
					LightColor = temporaryColor.light,
					EditContext = this,
					IsTemporary = true
				}, section);
			}
			else
			{
				if (colorCount == 0)
					builder.AddLabel("This theme haws no named colors yet. Click the button below to add a color.", section);

				builder.AddButton("Add color", () =>
				{
					temporaryColor = new EditableNamedColor();
					SetupColors();
				}, section);
			}

			editorWidgetList.SetItems(builder.Build());
		}

		private void SetupTerminal()
		{
			var builder = new WidgetBuilder();

			builder.Begin();
			
			themeEditor?.TerminalStyle.BuildWidgets(builder, MarkDirty, this);
			
			editorWidgetList.SetItems(builder.Build());
		}

		private void SetupTypography()
		{
			var builder = new WidgetBuilder();

			builder.Begin();
			
			themeEditor?.WidgetStyle.Typography.BuildWidgets(builder, MarkDirty, this);
			
			editorWidgetList.SetItems(builder.Build());
		}
		
		private void SetupBackdrop()
		{
			var builder = new WidgetBuilder();

			builder.Begin();
			
			themeEditor?.BackdropStyle.BuildWidgets(builder, MarkDirty, this);
			
			editorWidgetList.SetItems(builder.Build());
		}

		private void SetupShell()
		{
			var builder = new WidgetBuilder();

			builder.Begin();
			
			themeEditor?.ShellStyle.BuildWidgets(builder, MarkDirty, this);
			
			editorWidgetList.SetItems(builder.Build());
		}

		private void SetupWidgets()
		{
			var builder = new WidgetBuilder();

			builder.Begin();
			
			themeEditor?.WidgetStyle.BuildWidgets(builder, MarkDirty, this);
			
			editorWidgetList.SetItems(builder.Build());
		}

		private void SetupWindows()
		{
			var builder = new WidgetBuilder();

			builder.Begin();
			
			themeEditor?.WindowStyle.BuildWidgets(builder, MarkDirty, this);
			
			editorWidgetList.SetItems(builder.Build());
		}

		private void SetupThemeInfo()
		{
			if (themeEditor == null)
				return;
			
			var builder = new WidgetBuilder();

			builder.Begin();

			builder.AddSection("Theme Info", out SectionWidget section)
				.AddWidget(new LabelWidget
				{
					Text = "Enter your theme's name and description, and add an optional preview image. This is what will be displayed on Steam Workshop and in the game's Customization Settings. You can change these later after publishing the theme too."
				}, section);

			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Theme name",
				CurrentValue = themeEditor.Name,
				Callback = (v) =>
				{
					hasUnsavedChanges = true;
					themeEditor.Name = v;
					UpdateToolbar();
				}
			}, section);
			
			
			
			editorWidgetList.SetItems(builder.Build());
		}
        
		private void SetupOpenThemeWidgets()
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			builder.AddSection("Open Existing Theme", out SectionWidget section);

			var list = new ListWidget
			{
				AllowSelectNone = false
			};

			builder.AddWidget(list, section);

			var themeCount = 0;
			foreach (IThemeAsset theme in GetThemes().Where(x => x.CanEdit))
			{
				builder.AddWidget(new ListItemWidget<IThemeAsset>()
				{
					List = list,
					Title = theme.Name,
					Selected = themeToOpen == theme,
					Callback = (t) =>
					{
						themeToOpen = t;
						RefreshPreview();
					},
					Data = theme
				}, section);

				themeCount++;
			}

			if (themeCount == 0)
			{
				builder.AddWidget(new LabelWidget()
				{
					Text = "You don't have any themes to open yet."
				}, section);
			}

			builder.AddSection("Import from ShiftOS Skin", out SectionWidget shiftosSection);

			builder.AddLabel(
				"You can import shiftOS skins from ShiftOS 0.0.7, 0.0.8, and 1.0. They will be converted to Socially Distant themes, where you can further edit them. To begin, just click the Open ShiftOS Skin button below.",
				shiftosSection
			);

			builder.AddWidget(new ButtonWidget
			{
				Text = "Open ShiftOS Skin",
				ClickAction = OpenShiftOSSkin
			}, shiftosSection);
			
			editorWidgetList.SetItems(builder.Build());
		}
		
		private void SetupNewThemeWidgets()
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			builder.AddSection("Create new theme", out SectionWidget section);

			var list = new ListWidget
			{
				AllowSelectNone = false
			};
			
			builder.AddWidget(list, section);
			
			builder.AddWidget(new LabelWidget
			{
				Text = "You can either create a theme from scratch, or create a theme based on one of the other themes installed on your system."
			}, section);

			builder.AddWidget(new ListItemWidget<bool>
			{
				List = list,
				Title = "Create from scratch",
				Selected = themeToClone == null,
				Callback = _ =>
				{
					themeToClone = null;
					RefreshPreview();
				}
			}, section);

			builder.AddSection("Clone existing theme", out SectionWidget themeSection);
            
			var themeCount = 0;
			foreach (IThemeAsset theme in GetThemes().Where(x => x.CanCopy))
			{
				builder.AddWidget(new ListItemWidget<IThemeAsset>()
				{
					Data = theme,
					Selected = themeToClone == theme,
					Callback = (t) =>
					{
						themeToClone = t;
						RefreshPreview();
					},
					Title = theme.Name,
					List = list
				}, themeSection);
				
				themeCount++;
			}

			if (themeCount == 0)
			{
				builder.AddWidget(new LabelWidget
				{
					Text = "There are no copyable themes installed."
				}, themeSection);
			}
			
			editorWidgetList.SetItems(builder.Build());
		}
		
		private void UpdateCategoryWidgets()
		{
			var builder = new WidgetBuilder();

			builder.Begin();
			
			builder.AddSection(
				this.themeEditor == null
					? "Theme Editor"
					: "Elements",
				out SectionWidget section
			);

			var list = new ListWidget
			{
				AllowSelectNone = false
			};

			builder.AddWidget(list, section);
			
			if (themeEditor == null)
			{
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.NewTheme,
					Title = "Create New Theme",
					Selected = this.editorMode == EditorMode.NewTheme,
					List = list
				}, section);
				
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.OpenTheme,
					Title = "Open Existing Theme",
					Selected = this.editorMode == EditorMode.OpenTheme,
					List = list
				}, section);
			}
			else
			{
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.ThemeInfo,
					Title = "Theme Info",
					Selected = this.editorMode == EditorMode.ThemeInfo,
					List = list
				}, section);
				
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.Colors,
					Title = "Colors",
					Selected = this.editorMode == EditorMode.Colors,
					List = list
				}, section);
				
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.Typography,
					Title = "Typography",
					Selected = this.editorMode == EditorMode.Typography,
					List = list
				}, section);
				
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.Backdrop,
					Title = "Backdrops",
					Selected = this.editorMode == EditorMode.Backdrop,
					List = list
				}, section);
				
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.Shell,
					Title = "Desktop/Shell",
					Selected = this.editorMode == EditorMode.Shell,
					List = list
				}, section);
				
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.Windows,
					Title = "Window Decorations",
					Selected = this.editorMode == EditorMode.Windows,
					List = list
				}, section);
				
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.Widgets,
					Title = "Widgets",
					Selected = this.editorMode == EditorMode.Widgets,
					List = list
				}, section);
				
				builder.AddWidget(new ListItemWidget<EditorMode>
				{
					Callback = SetEditorMode,
					Data = EditorMode.Terminal,
					Title = "Terminal Style",
					Selected = this.editorMode == EditorMode.Terminal,
					List = list
				}, section);
			}
			
			categoryWidgetList.SetItems(builder.Build());
		}

		private IEnumerable<IThemeAsset> GetThemes()
		{
			if (gameManager.Value == null)
				yield break;

			foreach (IThemeAsset theme in gameManager.Value.ContentManager.GetContentOfType<IThemeAsset>())
				yield return theme;
		}

		private async Task<string?> SaveAs()
		{
			if (themeEditor == null)
				return null;
			
			if (string.IsNullOrWhiteSpace(themeEditor.Name))
			{
				// We need to get a name for the theme!
				// Set it as an unnamed theme because I'm too lazy to implement text entry dialogs.
				themeEditor.Name = "Unnamed theme";
			}

			string userThemesPath = Path.Combine(Application.persistentDataPath, "themes");

			string filename = string.Empty;
			string fullPath = string.Empty;
			var index = 0;

			do
			{
				if (index == 0)
				{
					filename = $"{themeEditor.Name}.sdtheme";
				}
				else
				{
					filename = $"{themeEditor.Name} ({index}).sdtheme";
				}

				fullPath = Path.Combine(userThemesPath, filename);
				
				index++;
			} while (File.Exists(fullPath));


			await SaveInternal(fullPath, themeEditor);
			
			return fullPath;
		} 
		
		private async Task Save()
		{
			if (themeEditor == null)
				return;
			
			hasUnsavedChanges = false;

			if (string.IsNullOrWhiteSpace(themeEditor.FilePath))
			{
				themeEditor.FilePath = await SaveAs();
				return;
			}

			await SaveInternal(themeEditor.FilePath, themeEditor);
			
			if (gameManager.Value == null)
				return;

			await gameManager.Value.ContentManager.RefreshContentDatabaseAsync();
		}

		private async Task SaveInternal(string path, OperatingSystemTheme.ThemeEditor editor)
		{
			saving = true;
			UpdateToolbar();
			
			using var saver = new ThemeSaver(path, editor);

			await saver.SaveAsync();

			saving = false;
			UpdateToolbar();
		}
		
		private Task<bool?> AskForSave()
		{
			var completionSource = new TaskCompletionSource<bool?>();

			this.dialogHlper.AskYesNoCancel(
				"Save and continue",
				"Your theme currently has unsaved changes. Would you like to save changes before continuing?",
				null,
				completionSource.SetResult
			);
			
			return completionSource.Task;
		}
		
		private async Task<bool> SaveIfNeeded()
		{
			if (!hasUnsavedChanges)
				return true;

			bool? shouldSave = await AskForSave();

			if (shouldSave == null)
				return false;


			if (shouldSave == true)
				await Save();

			return true;
		}

		private async void OnNewClicked()
		{
			bool shouldContinue = await SaveIfNeeded();

			if (!shouldContinue)
				return;
			
			themeEditor = null;
			
			SetEditorMode(EditorMode.NewTheme);
		}

		private void RebuildThemeColors(bool updateUi = true)
		{
			if (themeEditor == null)
				return;

			themeEditor.Colors.Clear();

			foreach (EditableNamedColor editable in namedColors)
			{
				if (string.IsNullOrWhiteSpace(editable.name))
					continue;

				themeEditor.Colors[editable.name] = new NamedColor
				{
					darkColor = editable.dark,
					lightColor = editable.light
				};
			}


			MarkDirty();

			RefreshPreview();

			if (this.editorMode == EditorMode.Colors && updateUi)
				SetupColors();
		}

		private class EditableNamedColor
		{
			public string? name;
			public Color dark;
			public Color light;
		}

		private void BuildPreviewWidgets()
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			builder.AddSection("User information", out SectionWidget section);
			builder.AddLabel("Please log into Online Baking", section);
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Username",
				CurrentValue = "admin",
			}, section);
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Password",
				Description = "If you've forgotten your password, contact the office IT department for assistance.",
				CurrentValue = "password123",
			}, section);
			
			builder.AddWidget(new SettingsToggleWidget()
			{
				Title = "Remember me",
				CurrentValue = true
			}, section);
			
			builder.AddWidget(new SettingsToggleWidget()
			{
				Title = "I am human",
				CurrentValue = false
			}, section);

			builder.AddWidget(new ButtonWidget
			{
				Text = "Log in"
			}, section);
			
			builder.AddSection("Advanced login settings", out section);

			builder.AddWidget(new SettingsDropdownWidget
			{
				Title = "Cookie policy",
				Description = "Determine how we should manage cookies during this session.",
				CurrentIndex = 0,
				Choices = new string[]
				{
					"Allow tracking cookies",
					"Necessary cookies only"
				}
			}, section);

			
			
			
			
			
			previewWidgetList.SetItems(builder.Build());
		}
		
		private enum EditorMode
		{
			NewTheme,
			OpenTheme,
			ThemeInfo,
			Typography,
			Backdrop,
			Shell,
			Windows,
			Widgets,
			Terminal,
			Colors
		}

		/// <inheritdoc />
		public bool ColorWithNameExists(string colorName)
		{
			return namedColors.Any(x => x.name == colorName);
		}

		/// <inheritdoc />
		public void RenameColor(string colorName, string newName)
		{
			if (colorName == newName)
				return;
			
			if (string.IsNullOrWhiteSpace(colorName) && temporaryColor == null)
				throw new InvalidOperationException("RenameColor called with an empty color name, but the temporary color is null.");
			
			if (string.IsNullOrWhiteSpace(newName))
				throw new InvalidOperationException("RenameColor called with a new color name that's blank or whitespace.");

			if (ColorWithNameExists(newName))
				throw new InvalidOperationException("Attempting to rename a color to another color that already exists.");

			if (temporaryColor != null && string.IsNullOrWhiteSpace(colorName))
			{
				temporaryColor.name = newName;
				namedColors.Add(temporaryColor);
				temporaryColor = null;
				RebuildThemeColors();
				return;
			}

			EditableNamedColor? existingColor = namedColors.FirstOrDefault(x => x.name == colorName);
			if (existingColor == null)
				throw new InvalidOperationException("Color with name " + colorName + " does not exist.");

			existingColor.name = newName;
			RebuildThemeColors();
		}

		/// <inheritdoc />
		public void CancelTemporaryColor()
		{
			temporaryColor = null;
			RebuildThemeColors();
		}

		/// <inheritdoc />
		public void UpdateColors(string colorName, Color dark, Color light)
		{
			EditableNamedColor? color = namedColors.FirstOrDefault(x => x.name == colorName);
			if (color == null)
				throw new InvalidOperationException($"Color {colorName} does not exist.");

			color.dark = dark;
			color.light = light;
			
			RebuildThemeColors(false);
		}

		/// <inheritdoc />
		public void DeleteColor(string colorName)
		{
			EditableNamedColor? color = namedColors.FirstOrDefault(x => x.name == colorName);
			if (color == null)
				return;

			namedColors.Remove(color);
			RebuildThemeColors();
		}

		/// <inheritdoc />
		public Color? GetNamedColor(string colorName, bool useDark)
		{
			EditableNamedColor? color = namedColors.FirstOrDefault(x => x.name == colorName);
			if (color == null)
				return null;

			return useDark ? color.dark : color.light;
		}

		/// <inheritdoc />
		public string[] GetColorNames()
		{
			return namedColors.Select(x => x.name)
				.ToArray()!;
		}

		/// <inheritdoc />
		public IEnumerable<string> GetGraphicNames()
		{
			if (themeEditor == null)
				return Enumerable.Empty<string>();

			return themeEditor.GetGraphicNames();
		}

		/// <inheritdoc />
		public Texture2D? GetGraphic(string graphicName)
		{
			return themeEditor?.GetGraphic(graphicName);
		}

		/// <inheritdoc />
		public void SetGraphic(string name, Texture2D? texture)
		{
			themeEditor?.SetGraphic(name, texture);
		}
	}
}