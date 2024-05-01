#nullable enable
using AcidicGui.Widgets;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityExtensions;

namespace UI.Animation
{
	public class RectangleBorderDriver : AnimatedHighlightDriver
	{
		[SerializeField]
		private Rectangle rectangle = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(RectangleBorderDriver));
		}

		/// <inheritdoc />
		public override Color CurrentColor
		{
			get => rectangle.ShapeProperties.OutlineColor;
			set
			{
				if (rectangle.ShapeProperties.OutlineColor == value)
					return;

				rectangle.ShapeProperties.OutlineColor = value;
				rectangle.ForceMeshUpdate();
			}
		}
	}
}