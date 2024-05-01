#nullable enable

using System;
using AcidicGui.Widgets;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityExtensions;

namespace UI.Animation
{
	public class RectangleHighlightDriver : AnimatedHighlightDriver
	{
		[SerializeField]
		private Rectangle rectangle = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(RectangleHighlightDriver));
		}

		/// <inheritdoc />
		public override Color CurrentColor
		{
			get => rectangle.ShapeProperties.FillColor;
			set
			{
				if (rectangle.ShapeProperties.FillColor == value)
					return;

				rectangle.ShapeProperties.FillColor = value;
				rectangle.ForceMeshUpdate();
			}
		}
	}
}