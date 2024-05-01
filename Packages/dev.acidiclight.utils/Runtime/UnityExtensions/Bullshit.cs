#nullable enable
using System;
using UnityEngine;

namespace UnityExtensions
{
	public static class Bullshit
	{
		public static string ReplaceLineEndings(this string source)
		{
			return source.Replace("\r\n", "\n")
				.Replace("\n\r", "\n")
				.Replace("\r", "\n");
		}
		
		public static Vector2 GetGuiMouseCoords(Canvas canvas, Vector2 screenMouseCoords)
		{
			var rectTransform = (canvas.transform as RectTransform)!;
			Rect rect = rectTransform.rect;
            
			switch (canvas.renderMode)
			{
				case RenderMode.ScreenSpaceOverlay:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenMouseCoords, null, out Vector2 c);
					return c - rect.position;
				}
				case RenderMode.ScreenSpaceCamera:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenMouseCoords, canvas.worldCamera, out Vector2 c);
					return c - rect.position;
				}
				case RenderMode.WorldSpace:
					return canvas.worldCamera.ScreenToWorldPoint(screenMouseCoords);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}