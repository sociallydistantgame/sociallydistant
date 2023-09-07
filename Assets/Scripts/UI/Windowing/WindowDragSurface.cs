#nullable enable

using System;
using Shell.Windowing;
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

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(WindowDragSurface));
		}

		/// <inheritdoc />
		public void OnDrag(PointerEventData eventData)
		{
			if (isDragging)
			{
				this.window.Position += eventData.delta / this.window.RectTransform.lossyScale;
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