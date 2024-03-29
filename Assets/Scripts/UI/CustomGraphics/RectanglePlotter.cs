#nullable enable

using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace UI.CustomGraphics
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class RectanglePlotter : MaskableGraphic
	{
		[Tooltip("Choose whether to render the default graphic when no rectangles are plotted.")]
		[SerializeField]
		private bool renderDefaultGraphic = false;
		
		private readonly ConcurrentBag<RectangleElement> rectangles = new ConcurrentBag<RectangleElement>();
		
		/// <inheritdoc />
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			
			if (this.rectangles.Count == 0)
			{
				RenderDefaultGraphic(vh);
				return;
			}
			
            Rect pixelRect = GetPixelAdjustedRect();

            if (pixelRect.width * pixelRect.height < 0)
            {
	            RenderDefaultGraphic(vh);
	            return;
            }

            RectangleElement[] elementArray = this.rectangles.ToArray();
            Vector3 scale = rectTransform.lossyScale;
            for (var i = 0; i < elementArray.Length; i++)
			{
				RectangleElement element = elementArray[i];

				Color32 rectColor = element.Color;

				if (rectColor.a == 0)
					continue;
				
				Rect rect = element.Rect;

				rect.x *= scale.x;
				rect.y *= scale.y;
				rect.width *= scale.x;
				rect.height *= scale.y;
				
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

		private void RenderDefaultGraphic(VertexHelper vh)
		{
			if (!this.renderDefaultGraphic)
			{
				Rect rect = this.GetPixelAdjustedRect();
				
				var max = new Vector2(rect.xMax, rect.yMax);
				var min = new Vector2(rect.xMin, rect.yMin);

				max.x /= rect.width;
				max.y /= rect.height;

				min.x /= rect.width;
				min.y /= rect.height;

				Vector2 pivot = this.rectTransform.pivot;

				max -= pivot;
				min -= pivot;

				max.x *= rect.width;
				min.x *= rect.width;

				max.y *= rect.height;
				min.y *= rect.height;
				
				int vertOffset = vh.currentVertCount;
				
				vh.AddVert(new Vector2(min.x, min.y), default, Vector4.zero);
				vh.AddVert(new Vector2(max.x, min.y), default, Vector4.zero);
				vh.AddVert(new Vector2(min.x, max.y), default, Vector4.zero);
				vh.AddVert(new Vector2(max.x, max.y), default, Vector4.zero);
				
				vh.AddTriangle(vertOffset, vertOffset + 1, vertOffset + 2);
				vh.AddTriangle(vertOffset + 2, vertOffset + 3, vertOffset + 1);
				
				return;
			}
			
			base.OnPopulateMesh(vh);
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