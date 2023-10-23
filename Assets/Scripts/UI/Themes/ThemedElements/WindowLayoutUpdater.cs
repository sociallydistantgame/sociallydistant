#nullable enable

using System;
using UI.Themes.ThemeData;
using UI.Theming;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class WindowLayoutUpdater : ShellElement
	{
		[SerializeField]
		private LayoutGroup layoutGroup = null!;

		[SerializeField]
		private RectTransform titleArea = null!;
		
		[SerializeField]
		private RectTransform iconRect = null!;

		[SerializeField]
		private RectTransform titleText = null;

		[SerializeField]
		private RectTransform closeButton = null!;

		[SerializeField]
		private RectTransform minimizeButton = null!;

		[SerializeField]
		private RectTransform maximizeButton = null!;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(WindowLayoutUpdater));
			base.Awake();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			WindowStyle windowDecorations = theme.WindowDecorations;

			layoutGroup.padding = windowDecorations.WindowBorderSizes;
			titleArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, windowDecorations.WindowBorderSizes.top);
			
			ApplyElementLayout(iconRect, windowDecorations.IconLayout);
			ApplyElementLayout(titleText, windowDecorations.TitleTextLayout);
			ApplyElementLayout(closeButton, windowDecorations.CloseButtonLayout);
			ApplyElementLayout(maximizeButton, windowDecorations.MaximizeButtonLayout);
			ApplyElementLayout(minimizeButton, windowDecorations.MinimizeButtonLayout);
			
		}

		private void ApplyElementLayout(RectTransform element, WindowElementLayout layout)
		{
			// Anchor
			switch (layout.Anchor)
			{
				default:
				case WindowAnchor.TopLeft:
					element.anchorMin = new Vector2(0, 1);
					element.anchorMax = new Vector2(0, 1);
					element.pivot = new Vector2(0f, 1f);
					break;
				case WindowAnchor.Top:
					element.anchorMin = new Vector2(0.5f, 1);
					element.anchorMax = new Vector2(0.5f, 1);
					element.pivot = new Vector2(0.5f, 1f);
					break;
				case WindowAnchor.TopRight:
					element.anchorMin = new Vector2(1f, 1);
					element.anchorMax = new Vector2(1f, 1);
					element.pivot = new Vector2(1f, 1f);
					break;
				case WindowAnchor.Left:
					element.anchorMin = new Vector2(0f, 0.5f);
					element.anchorMax = new Vector2(0f, 0.5f);
					element.pivot = new Vector2(0f, 0.5f);
					break;
				case WindowAnchor.Center:
					element.anchorMin = new Vector2(0.5f, 0.5f);
					element.anchorMax = new Vector2(0.5f, 0.5f);
					element.pivot = new Vector2(0.5f, 0.5f);
					break;
				case WindowAnchor.Right:
					element.anchorMin = new Vector2(1f, 0.5f);
					element.anchorMax = new Vector2(1f, 0.5f);
					element.pivot = new Vector2(1f, 0.5f);
					break;
				case WindowAnchor.BottomLeft:
					element.anchorMin = new Vector2(0f, 0f);
					element.anchorMax = new Vector2(0f, 0f);
					element.pivot = new Vector2(0f, 0f);
					break;
				case WindowAnchor.Bottom:
					element.anchorMin = new Vector2(0.5f, 0f);
					element.anchorMax = new Vector2(0.5f, 0f);
					element.pivot = new Vector2(0.5f, 0f);
					break;
				case WindowAnchor.BottomRight:
					element.anchorMin = new Vector2(1f, 0f);
					element.anchorMax = new Vector2(1f, 0f);
					element.pivot = new Vector2(1f, 0f);
					break;
			}
			
			// Offset
			element.anchoredPosition = layout.Offset;
			
            // Size
            element.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, layout.Size.x);
            element.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, layout.Size.y);
		}
	}
}