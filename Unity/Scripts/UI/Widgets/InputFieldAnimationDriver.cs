#nullable enable
using System;
using AcidicGui.Widgets;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets
{
	public sealed class InputFieldAnimationDriver : AnimatedHighlightDriver
	{
		
		private Image underline = null!;

		
		private Rectangle fullBorder = null!;
        
		private bool useFullBorder = false;
		
		public bool UseFullBorder
		{
			get => useFullBorder;
			set
			{
				if (UseFullBorder == value)
					return;

				useFullBorder = value;
				UpdateVisibility();
			}
		}
		
		/// <inheritdoc />
		public override Color CurrentColor
		{
			get => GetCurrentColor();
			set => SetColor(value);
		}

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(InputFieldAnimationDriver));
		}

		private void OnEnable()
		{
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			fullBorder.gameObject.SetActive(useFullBorder);
			underline.gameObject.SetActive(!useFullBorder);
			
			if (useFullBorder)
				fullBorder.ForceMeshUpdate();
		}

		private Color GetCurrentColor()
		{
			if (useFullBorder)
				return fullBorder.ShapeProperties.OutlineColor;
			return underline.color;
		}

		private void SetColor(Color newColor)
		{
			underline.color = newColor;
			fullBorder.ShapeProperties.OutlineColor = newColor;

			if (useFullBorder)
				fullBorder.ForceMeshUpdate();
		}
	}
}