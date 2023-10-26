#nullable enable
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Themes.ThemedElements
{
	public abstract class SelectableShellElement : 
		ShellElement,
		IPointerEnterHandler,
		IPointerExitHandler,
		ISelectHandler,
		IDeselectHandler,
		IPointerDownHandler,
		IPointerUpHandler
	{
		private bool hovered;
		private bool pressed;
		private bool active;

		public bool IsPressed => pressed;
		public bool IsSelected => active;
		public bool IsHovered => hovered;

		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{
			hovered = true;
			NotifyThemePropertyChanged();
		}

		/// <inheritdoc />
		public void OnPointerExit(PointerEventData eventData)
		{
			hovered = false;
			NotifyThemePropertyChanged();
		}

		/// <inheritdoc />
		public void OnSelect(BaseEventData eventData)
		{
			active = true;
			NotifyThemePropertyChanged();
		}

		/// <inheritdoc />
		public void OnDeselect(BaseEventData eventData)
		{
			active = false;
			NotifyThemePropertyChanged();
		}

		/// <inheritdoc />
		public void OnPointerDown(PointerEventData eventData)
		{
			pressed = true;
			NotifyThemePropertyChanged();
		}

		/// <inheritdoc />
		public void OnPointerUp(PointerEventData eventData)
		{
			pressed = false;
			NotifyThemePropertyChanged();
		}
	}
}