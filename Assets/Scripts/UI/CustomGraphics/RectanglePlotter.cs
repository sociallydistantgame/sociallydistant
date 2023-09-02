#nullable enable

using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine.UI;
using UnityEngine;

namespace UI.CustomGraphics
{
	public class RectanglePlotter : MaskableGraphic
	{
		private readonly List<RectangleElement> rectangles = new List<RectangleElement>();

		/// <inheritdoc />
		protected override void Awake()
		{
			Plot(new Rect(0, 0, 32, 32), Color.red);
		}

		/// <inheritdoc />
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			const int vertCount = 4;

			Rect pixelRect = GetPixelAdjustedRect();
			
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
				
				int vertOffset = i * vertCount;
				
				vh.AddVert(new Vector2(min.x, min.y), rectColor, Vector4.zero);
				vh.AddVert(new Vector2(max.x, min.y), rectColor, Vector4.zero);
				vh.AddVert(new Vector2(min.x, max.y), rectColor, Vector4.zero);
				vh.AddVert(new Vector2(max.x, max.y), rectColor, Vector4.zero);
				
				vh.AddTriangle(vertOffset, vertOffset + 1, vertOffset + 2);
				vh.AddTriangle(vertOffset + 2, vertOffset + 3, vertOffset + 1);
			}
		}

		public void Clear()
		{
			rectangles.Clear();
			this.UpdateGeometry();
		}

		public void Plot(Rect rect, Color color)
		{
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
			this.UpdateGeometry();
		}

		private struct RectangleElement
		{
			public Color32 Color;
			public Rect Rect;
		}
	}
}