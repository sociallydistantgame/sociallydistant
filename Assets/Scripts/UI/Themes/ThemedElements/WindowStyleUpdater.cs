#nullable enable
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UI.Themes.ThemeData;
using UI.Theming;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class WindowStyleUpdater : ShellElement
	{
		[SerializeField]
		private Image decorations = null!;

		[SerializeField]
		private Image decorationMask = null!;

		[SerializeField]
		private TextMeshProUGUI titleText = null!;

		private bool useActiveDecorations;

		public bool UseActiveDecorations
		{
			get => useActiveDecorations;
			set
			{
				useActiveDecorations = value;
				NotifyThemePropertyChanged();
			}
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(WindowStyleUpdater));
			base.Awake();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			WindowStyle windowStyle = theme.WindowDecorations;
			ThemeGraphic? decorationStyle = useActiveDecorations ? windowStyle.ActiveDecorations : windowStyle.InactiveDecorations;

			this.decorations.color = theme.TranslateColor(decorationStyle.Color, Provider.UseDarkMode);
			
			if (decorationStyle.Texture != null)
			{
				int width = decorationStyle.Texture.width;
				int height = decorationStyle.Texture.height;
			
				var bounds = new Rect(0, 0, width, height);
				Vector2 pivot = new Vector2(0.5f, 0.5f);

				var decorationSprite = Sprite.Create(decorationStyle.Texture, bounds, pivot, 100, 0, SpriteMeshType.FullRect, decorationStyle.SpriteMargins.ToVector4());

				this.decorations.sprite = decorationSprite;
			}
			else
			{
				this.decorations.sprite = null;
			}

			//this.decorationMask.sprite = this.decorations.sprite;

			ThemeColor titleTextColor = useActiveDecorations
				? windowStyle.ActiveTitleTextColor
				: windowStyle.InactiveTitleTextColor;

			this.titleText.color = theme.TranslateColor(titleTextColor, Provider.UseDarkMode);
		}
	}
}