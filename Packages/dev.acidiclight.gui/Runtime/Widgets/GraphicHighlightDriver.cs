using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace AcidicGui.Widgets
{
	[RequireComponent(typeof(Graphic))]
	public sealed class GraphicHighlightDriver : AnimatedHighlightDriver
	{
		[SerializeField]
		private Graphic graphic;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(GraphicHighlightDriver));
		}

		/// <inheritdoc />
		public override Color CurrentColor
		{
			get => graphic.color;
			set => graphic.color = value;
		}
	}
}