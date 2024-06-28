#nullable enable
using System;
using Core;
using Shell;
using Shell.Windowing;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityExtensions;

namespace UI.Windowing
{
	public sealed class DecorationManager : 
		MonoBehaviour,
		IColorable
	{
		
		private CommonColor borderColor = CommonColor.Cyan;
		
		
		private Rectangle borderRectangle = null!;

		private bool useClientBackground = true;

		public bool UseClientBackground
		{
			get => useClientBackground;
			set
			{
				if (useClientBackground == value)
					return;

				useClientBackground = value;
				UpdateDecorations();
			}
		}
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DecorationManager));
		}

		private void UpdateDecorations()
		{
			this.borderRectangle.ShapeProperties.OutlineColor = this.borderColor.GetColor();
			this.borderRectangle.ShapeProperties.DrawFill = useClientBackground;
			this.borderRectangle.ForceMeshUpdate();
		}

		/// <inheritdoc />
		public CommonColor Color
		{
			get => borderColor;
			set
			{
				if (borderColor == value)
					return;

				borderColor = value;
				UpdateDecorations();
			}
		}
	}
}