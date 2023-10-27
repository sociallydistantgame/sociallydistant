#nullable enable
using ThisOtherThing.UI.Shapes;
using UI.Theming;
using UnityEngine;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class DropdownBackgroundStyleUpdater : ShellElement
	{
		[SerializeField]
		private Rectangle background = null!;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DropdownBackgroundStyleUpdater));
			base.Awake();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			background.ApplyRectangleStyle(theme, theme.WidgetStyles.DropdownTheme.ContentBackground, Provider.UseDarkMode);
		}
	}
}