#nullable enable
using AcidicGui.Widgets;
using ThisOtherThing.UI.Shapes;
using UI.Animation;
using UnityEngine;
using UnityExtensions;

namespace UI.Widgets
{
	public sealed class ButtonAnimation : MonoBehaviour
	{
		
		private bool useDangerTheme = false;
		
		
		private ButtonColors normalTheme = new();
		
		
		private ButtonColors dangerTheme = new();
		
		
        private RectangleHighlightDriver backgroundDriver;
        
		
        private RectangleBorderDriver borderDriver;
        
		
		private Rectangle rectangle;
		private AnimatedHighlight borderHighlight;
		private AnimatedHighlight backgroundHighlight;
		
		public bool UseDangerTheme
		{
			get => useDangerTheme;
			set
			{
				if (useDangerTheme == value)
					return;

				useDangerTheme = value;
				UpdateAnimations();
			}
		}
		
		private void Awake()
		{
			this.MustGetComponent(out rectangle);
			borderHighlight = this.gameObject.AddComponent<AnimatedHighlight>();
			backgroundHighlight = this.gameObject.AddComponent<AnimatedHighlight>();

			borderHighlight.Driver = borderDriver;
			backgroundHighlight.Driver = backgroundDriver;
			
			UpdateAnimations();
		}

		private void UpdateAnimations()
		{
			ButtonColors? theme = useDangerTheme ? dangerTheme : normalTheme;

			ApplyTheme(borderHighlight, theme.Border);
			ApplyTheme(backgroundHighlight, theme.Background);
		}

		private void ApplyTheme(AnimatedHighlight target, ButtonColorList theme)
		{
			target.NormalColor = theme.NormalColor;
			target.HoveredColor = theme.HighlightedColor;
			target.PressedColor = theme.PressedColor;
			target.ActiveColor = theme.FocusedColor;
		}
	}
}