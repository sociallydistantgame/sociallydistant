#nullable enable

using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine.UI;
using UnityEngine;

namespace UI.CustomGraphics
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class RectanglePlotter : MaskableGraphic
	{
		private readonly List<RectangleElement> rectangles = new List<RectangleElement>();
		
		/// <inheritdoc />
		protected override void OnPopulateMesh(VertexHelper vh)
		{
            vh.Clear();

            Vector3 scale = rectTransform.lossyScale;
            
            Rect pixelRect = GetPixelAdjustedRect();

			if (pixelRect.width * pixelRect.height < 0)
				return;
			
			for (var i = 0; i < rectangles.Count; i++)
			{
				RectangleElement element = rectangles[i];

				Color32 rectColor = element.Color;

				if (rectColor.a == 0)
					continue;
				
				Rect rect = element.Rect;

				var max = new Vector2(rect.xMax, pixelRect.height - rect.yMax);
				var min = new Vector2(rect.xMin, pixelRect.height - rect.yMin);

				max.x /= pixelRect.width;
				max.y /= pixelRect.height;

				min.x /= pixelRect.width;
				min.y /= pixelRect.height;

				Vector2 pivot = this.rectTransform.pivot;

				max -= pivot;
				min -= pivot;

				max.x *= pixelRect.width;
				min.x *= pixelRect.width;

				max.y *= pixelRect.height;
				min.y *= pixelRect.height;
				
				int vertOffset = vh.currentVertCount;
				
				vh.AddVert(new Vector2(min.x, min.y), rectColor, Vector4.zero);
				vh.AddVert(new Vector2(max.x, min.y), rectColor, Vector4.zero);
				vh.AddVert(new Vector2(min.x, max.y), rectColor, Vector4.zero);
				vh.AddVert(new Vector2(max.x, max.y), rectColor, Vector4.zero);
				
				vh.AddTriangle(vertOffset, vertOffset + 1, vertOffset + 2);
				vh.AddTriangle(vertOffset + 2, vertOffset + 3, vertOffset + 1);
			}
		}

		public void Refresh()
		{
			this.UpdateGeometry();
		}
		
		public void Clear()
		{
			rectangles.Clear();
		}

		public void Plot(Rect rect, Color color)
		{
			Vector3 scale = rectTransform.lossyScale;

			rect.x *= scale.x;
			rect.y *= scale.y;
			rect.width *= scale.x;
			rect.height *= scale.y;
			
			if (color.a == 0)
				return;

			if (rect.width <= 0)
				return;

			if (rect.height <= 0)
				return;
			
			var element = new RectangleElement
			{
				Color = color,
				Rect = rect
			};

			this.rectangles.Add(element);
		}

		private struct RectangleElement
		{
			public Color32 Color;
			public Rect Rect;
		}
	}
}