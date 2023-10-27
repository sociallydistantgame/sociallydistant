#nullable enable
using ThisOtherThing.UI.Shapes;
using TMPro;
using UI.Theming;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class DropdownCustomizer : SelectableShellElement
	{
		[SerializeField]
		private TextMeshProUGUI text = null!;

		[SerializeField]
		private Image arrow = null!;

		[SerializeField]
		private Rectangle arrowContainer = null!;
		
		[SerializeField]
		private Rectangle background = null!;

		private LayoutGroup layoutGroup = null!;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DropdownCustomizer));
			this.MustGetComponent(out layoutGroup);
			base.Awake();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			text.color = theme.TranslateColor(theme.WidgetStyles.DropdownTheme.LabelColor, Provider.UseDarkMode);
			arrow.ApplyThemeGraphic(theme, theme.WidgetStyles.DropdownTheme.ArrowGraphic, Provider.UseDarkMode);
			layoutGroup.padding = theme.WidgetStyles.DropdownTheme.ContentPadding;
            
			arrowContainer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, theme.WidgetStyles.DropdownTheme.ArrowContainerWidth);
			arrow.rectTransform.anchoredPosition = Vector3.zero;
			
			if (IsPressed)
				background.ApplyRectangleStyle(theme, theme.WidgetStyles.DropdownTheme.BackgroundPressed, Provider.UseDarkMode);
			else if (IsSelected)
				background.ApplyRectangleStyle(theme, theme.WidgetStyles.DropdownTheme.BackgroundActive, Provider.UseDarkMode);
			else if (IsHovered)
				background.ApplyRectangleStyle(theme, theme.WidgetStyles.DropdownTheme.BackgroundHover, Provider.UseDarkMode);
			else
				background.ApplyRectangleStyle(theme, theme.WidgetStyles.DropdownTheme.Background, Provider.UseDarkMode);

			if (background.ShapeProperties.DrawOutline)
				arrowContainer.ShapeProperties.FillColor = background.ShapeProperties.OutlineColor;
			else
				arrowContainer.ShapeProperties.FillColor = background.ShapeProperties.FillColor;
			
			arrowContainer.ForceMeshUpdate();
		}
	}
}