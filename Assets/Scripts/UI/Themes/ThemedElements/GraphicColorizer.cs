#nullable enable

using UI.Theming;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	[RequireComponent(typeof(Graphic))]
	public class GraphicColorizer : ShellElement
	{
		[SerializeField]
		private GraphicColor graphicColor;

		[SerializeField]
		private bool useCustomColor;

		[SerializeField]
		private Color customLightColor;

		[SerializeField]
		private Color customDarkColor;
		
		private Graphic graphic;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.MustGetComponent(out graphic);
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			if (useCustomColor)
			{
				graphic.color = Provider.UseDarkMode ? customDarkColor : customLightColor;
			}
			else
			{
				graphic.color = theme.GetGraphicColor(graphicColor, Provider.UseDarkMode);
			}
		}
	}
}