#nullable enable

using System;
using Shell.Windowing;
using UI.PlayerUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Windowing
{
	public class WindowDragSurface : 
		MonoBehaviour,
		IDragHandler,
		IBeginDragHandler,
		IEndDragHandler
	{
		private bool isDragging;
		
		[SerializeField]
		private UguiWindow window = null!;

		private Canvas rootCanvas;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(WindowDragSurface));
			this.MustGetComponentInParent(out Canvas windowCanvas);
			rootCanvas = windowCanvas.rootCanvas;
		}

		/// <inheritdoc />
		public void OnDrag(PointerEventData eventData)
		{
			if (isDragging)
			{
				Vector2 delta = Bullshit.GetGuiMouseCoords(rootCanvas, eventData.delta);
				this.window.Position += delta;
			}
		}

		/// <inheritdoc />
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (window.WindowState != WindowState.Maximized)
					isDragging = true;
			}
		}

		/// <inheritdoc />
		public void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
				isDragging = false;
		}
	}
}