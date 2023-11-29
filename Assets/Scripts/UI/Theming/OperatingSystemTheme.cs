#nullable enable

using System;
using System.Collections.Generic;
using UI.Themes.ThemeData;
using UnityEngine;
using System.Threading.Tasks;
using UI.Widgets;

namespace UI.Theming
{
	[CreateAssetMenu(menuName = "ScriptableObject/OS Theme")]
	public partial class OperatingSystemTheme :
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
		private string? filePath;
		
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
		private TextureDictionary assets = new TextureDictionary();
		
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

		public BackdropStyle BackdropStyle => backdropStyle;
		public WindowStyle WindowDecorations => windowStyle;
		public WidgetStyle WidgetStyles => widgetStyle;
		
		/// <inheritdoc />
		public string Id => name;
		public bool CanEdit => allowUserEditing;
		public bool CanCopy => allowUserCopying;
		public string Name => this.themeName;
		public string Author => this.author;
		public string Description => this.description;

		/// <inheritdoc />
		public string? FilePath => filePath;
		
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
		
		public class ThemeEditor : IGraphicPickerSource
		{
			private readonly OperatingSystemTheme theme;

			public OperatingSystemTheme Theme => theme;

			public NamedColorTable Colors => theme.colors;

			public string? FilePath
			{
				get => theme.filePath;
				set => theme.filePath = value;
			}
			
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

			public void BuildResourceMap(IThemeResourceStorage storage)
			{
				theme.assets.Clear();

				foreach (string textureName in storage.TextureNames)
				{
					if (!storage.TryGetTexture(textureName, out Texture2D? texture))
						continue;

					if (texture == null)
						continue;
					
					theme.assets[textureName] = texture;
				}
			}

			/// <inheritdoc />
			public IEnumerable<string> GetGraphicNames()
			{
				return theme.assets.Keys;
			}

			/// <inheritdoc />
			public Texture2D? GetGraphic(string graphicName)
			{
				theme.assets.TryGetValue(graphicName, out Texture2D? result);
				return result;
				
			}

			/// <inheritdoc />
			public void SetGraphic(string name, Texture2D? texture)
			{
				if (texture == null && theme.assets.ContainsKey(name))
					theme.assets.Remove(name);
				else if (texture != null)
					theme.assets[name] = texture;
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