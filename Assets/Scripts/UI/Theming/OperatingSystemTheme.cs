#nullable enable

using System;
using UI.Themes.ThemeData;
using UnityEngine;
using System.Threading.Tasks;

namespace UI.Theming
{
	[CreateAssetMenu(menuName = "ScriptableObject/OS Theme")]
	public class OperatingSystemTheme :
		ScriptableObject,
		IOperatingSystemTheme,
		IThemeAsset
	{
		[Header("Editor Settings")]
		[SerializeField]
		private bool allowUserEditing = false;

		[SerializeField]
		private bool allowUserCopying = false;
		
		[Header("Metadata")]
		[SerializeField]
		private string themeName = string.Empty;

		[SerializeField]
		private string author = string.Empty;

		[TextArea]
		[SerializeField]
		private string description = string.Empty;
		
		[SerializeField]
		private Texture2D? previewImage;

		[Header("Theme Data")]
		[SerializeField]
		private ShellStyle shellStyle = new ShellStyle();

		[SerializeField]
		private TerminalStyle terminalStyle = new TerminalStyle();
		
		[SerializeField]
		private NamedColorTable colors = new NamedColorTable();

		[SerializeField]
		private BackdropStyle backdropStyle = new BackdropStyle();
		
		[SerializeField]
		private WidgetStyle widgetStyle = new WidgetStyle();

		[SerializeField]
		private WindowStyle windowStyle = new WindowStyle();
		
		private readonly OperatingSystemThemeEngine engine = new OperatingSystemThemeEngine();

		public bool CanEdit => allowUserEditing;
		public bool CanCopy => allowUserCopying;
		public string Name => this.themeName;
		public string Author => this.author;
		public string Description => this.description;

		/// <inheritdoc />
		public Texture2D? PreviewImage => previewImage;

		/// <inheritdoc />
		public Task<OperatingSystemTheme> LoadAsync()
		{
			return Task.FromResult(this);
		}

		public ThemeEditor GetUserEditor()
		{
			if (!CanEdit)
				throw new InvalidOperationException("This theme cannot be edited.");

			return new ThemeEditor(this);
		}
		
		/// <inheritdoc />
		public Color GetAccentColor(SystemAccentColor accentColorName)
		{
			return engine.GetAccentColor(accentColorName);
		}

		public static ThemeEditor CreateEmpty(bool allowUserEditing, bool allowUserCloning)
		{
			var theme = ScriptableObject.CreateInstance<OperatingSystemTheme>();
			theme.allowUserEditing = allowUserEditing;
			theme.allowUserCopying = allowUserCloning;
			return new ThemeEditor(theme);
		}

		public async Task ExportAsync(string path)
		{
			var editor = new ThemeEditor(this);

			using var themeSaver = new ThemeSaver(path, editor);

			await themeSaver.SaveAsync();
		}
		
		public class ThemeEditor
		{
			private readonly OperatingSystemTheme theme;

			public OperatingSystemTheme Theme => theme;

			public NamedColorTable Colors => theme.colors;

			public string Name
			{
				get => theme.themeName;
				set => theme.themeName = value;
			}

			public string LocalAuthor
			{
				get => theme.author;
				set => theme.author = value;
			}

			public string Description
			{
				get => theme.description;
				set => theme.description = value;
			}

			public Texture2D? PreviewImage
			{
				get => theme.previewImage;
				set => theme.previewImage = value;
			}

			public BackdropStyle BackdropStyle => theme.backdropStyle;
			public WidgetStyle WidgetStyle => theme.widgetStyle;
			public WindowStyle WindowStyle => theme.windowStyle;
			public ShellStyle ShellStyle => theme.shellStyle;
			public TerminalStyle TerminalStyle => theme.terminalStyle;
			
			internal ThemeEditor(OperatingSystemTheme theme)
			{
				this.theme = theme;
			}
		}
		
		#if UNITY_EDITOR

		public void SetPreviewImage(Texture2D? newPreview)
		{
			this.previewImage = newPreview;
		}
		
		#endif
	}
}