#nullable enable
using UI.Theming;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	[RequireComponent(typeof(Selectable))]
	public class SelectableColorUpdater : 
		ShellElement,
		ISelectableUpdater
	{
		private Selectable selectable;
		private bool useActiveAsIdle;

		public bool UseActiveAsIdle
		{
			get => useActiveAsIdle;
			set
			{
				useActiveAsIdle = value;
				NotifyThemePropertyChanged();
			}
		}

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.MustGetComponent(out selectable);
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			Color selectionColor = theme.GetGraphicColor(GraphicColor.Selected, Provider.UseDarkMode);

			selectable.transition = Selectable.Transition.ColorTint;
			selectable.colors = new ColorBlock
			{
				normalColor = useActiveAsIdle ? selectionColor : default,
				disabledColor = default,
				highlightedColor = selectionColor * 0.75f,
				pressedColor = selectionColor * 0.5f,
				selectedColor = selectionColor,
				colorMultiplier = 1,
				fadeDuration = 0.1f
			};
		}
	}
}