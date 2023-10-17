#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcidicGui.Common;
using AcidicGui.Widgets;
using GamePlatform;
using Modding;
using TMPro;
using UI.Theming;
using UI.Widgets;
using UnityEngine;
using UnityExtensions;
using UnityEngine.UI;

namespace UI.Editors.ThemeEditor
{
	public class ThemeEditorController : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		[Header("UI")]
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
		
		[Header("Pages")]
		[SerializeField]
		private VisibilityController newThemePage = null!;

		[SerializeField]
		private VisibilityController cloneThemePage = null!;
		
		private OperatingSystemTheme.ThemeEditor? themeEditor = null;
		private EditorMode editorMode;
		private IThemeAsset? themeToClone;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ThemeEditorController));
		}

		private void Start()
		{
			createEmptyButton.onClick.AddListener(CreateEmptyTheme);
			cloneThemeButton.onClick.AddListener(CreateEmptyTheme);
			
			SetEditorMode(EditorMode.NewTheme);
		}

		private async void CreateEmptyTheme()
		{
			if (themeToClone == null)
			{
				themeEditor = OperatingSystemTheme.CreateEmpty(true, true);
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
		}

		private void UpdateClonePage()
		{
			if (themeToClone == null)
				return;

			cloneImage.texture = themeToClone.PreviewImage;
			cloneTitle.SetText(themeToClone.Name);

			var sb = new StringBuilder();

			sb.Append("<b>Theme author:</b> ");
			sb.AppendLine(themeToClone.Author);
			sb.AppendLine();

			sb.Append(themeToClone.Description);
			
			this.cloneDescription.SetText(sb);
		}
		
		private void RefreshPreview()
		{
			newThemePage.Hide();
			cloneThemePage.Hide();

			switch (editorMode)
			{
				case EditorMode.NewTheme when themeToClone == null:
					newThemePage.Show();
					break;
				case EditorMode.NewTheme when themeToClone != null:
					cloneThemePage.Show();
					UpdateClonePage();
					break;
				case EditorMode.OpenTheme:
					break;
				case EditorMode.ThemeInfo:
					break;
				case EditorMode.Backdrop:
					break;
				case EditorMode.Shell:
					break;
				case EditorMode.Windows:
					break;
				case EditorMode.Widgets:
					break;
				case EditorMode.Terminal:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdateEditorWidgets()
		{
			switch (editorMode)
			{
				case EditorMode.NewTheme:
					SetupNewThemeWidgets();
					break;
				case EditorMode.OpenTheme:
					break;
				case EditorMode.ThemeInfo:
					break;
				case EditorMode.Backdrop:
					break;
				case EditorMode.Shell:
					break;
				case EditorMode.Windows:
					break;
				case EditorMode.Widgets:
					break;
				case EditorMode.Terminal:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
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
			
			categoryWidgetList.SetItems(builder.Build());
		}

		private IEnumerable<IThemeAsset> GetThemes()
		{
			if (gameManager.Value == null)
				yield break;

			foreach (IThemeAsset theme in gameManager.Value.ContentManager.GetContentOfType<IThemeAsset>())
				yield return theme;
		}
		
		
		private enum EditorMode
		{
			NewTheme,
			OpenTheme,
			ThemeInfo,
			Backdrop,
			Shell,
			Windows,
			Widgets,
			Terminal,
			Colors
		}
	}
}