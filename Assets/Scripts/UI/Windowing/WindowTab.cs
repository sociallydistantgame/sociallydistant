#nullable enable
using System;
using Codice.Client.GameUI.Update;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Windowing
{
	public class WindowTab : 
		UIBehaviour,
		IPointerDownHandler,
		IPointerUpHandler,
		IPointerEnterHandler,
		IPointerExitHandler,
		ISelectHandler,
		IDeselectHandler
	{
		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI label = null!;

		private readonly Color normalBackgroundColor = GuiHelpers.GetPackedColor(0x444444ff);
		private readonly Color normalHoveredBackgroundColor = GuiHelpers.GetPackedColor(0x0F73A9ff);
		private readonly Color normalPressedBackgroundColor = GuiHelpers.GetPackedColor(0x08537Bff);
		private readonly Color normalFocusedBackgroundColor = GuiHelpers.GetPackedColor(0x1693D6ff);
		private readonly Color activeBackgroundColor = GuiHelpers.GetPackedColor(0x1693D6ff);
		private readonly Color activeHoveredBackgroundColor = GuiHelpers.GetPackedColor(0x0F73A9ff);
		private readonly Color activePressedBackgroundColor = GuiHelpers.GetPackedColor(0x444444ff);
		private readonly Color activeFocusedBackgroundColor = GuiHelpers.GetPackedColor(0x08537Bff);
		private readonly Color normalPanicBackgroundColor = GuiHelpers.GetPackedColor(0x870909ff);
		private readonly Color normalPanicHoveredBackgroundColor = GuiHelpers.GetPackedColor(0x444444ff);
		private readonly Color normalPanicPressedBackgroundColor = GuiHelpers.GetPackedColor(0x444444ff);
		private readonly Color normalPanicFocusedBackgroundColor = GuiHelpers.GetPackedColor(0xFA5353ff);
		private readonly Color activePanicBackgroundColor = GuiHelpers.GetPackedColor(0xFA5353ff);
		private readonly Color activePanicHoveredBackgroundColor = GuiHelpers.GetPackedColor(0x444444ff);
		private readonly Color activePanicPressedBackgroundColor = GuiHelpers.GetPackedColor(0x444444ff);
		private readonly Color activePanicFocusedBackgroundColor = GuiHelpers.GetPackedColor(0xFA5353ff);
		
		private Rectangle rectangle;
		private Button button;
		private bool isActiveTab;
		private bool hovered;
		private bool pressed;
		private bool selected;
		
		public string Title
		{
			get => label.text;
			set => label.SetText(value);
		}

		public bool IsActiveTab
		{
			get => isActiveTab;
			set
			{
				if (isActiveTab == value)
					return;

				isActiveTab = value;
				UpdateRectangle();
			}
		}
		public int TabIndex { get; set; }

		public Action<int>? Clicked { get; set; } 
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(WindowTab));
			
			this.MustGetComponent(out rectangle);
			this.MustGetComponent(out button);
			
			button.onClick.AddListener(OnClick);
		}

		/// <inheritdoc />
		protected override void OnEnable()
		{
			UpdateRectangle();
			base.OnEnable();
		}

		/// <inheritdoc />
		protected override void OnDisable()
		{
			UpdateRectangle();
			base.OnDisable();
		}

		private void OnClick()
		{
			Clicked?.Invoke(this.TabIndex);
		}

		private void UpdateRectangle()
		{
			Color backgroundColor = IsActiveTab ? activeBackgroundColor : normalBackgroundColor;
			Color textColor = enabled ? Color.white : Color.gray;
			Color borderColor = backgroundColor;

			if (!enabled)
			{
				backgroundColor *= 0.5f;
				borderColor = backgroundColor;
			}
			else  if (IsActiveTab)
			{
				if (pressed)
				{
					backgroundColor = activePressedBackgroundColor;
					borderColor = activeFocusedBackgroundColor;
				}
				else if (hovered)
				{
					backgroundColor = activeHoveredBackgroundColor;
					borderColor = activeFocusedBackgroundColor;
				}
				else if (selected)
				{
					borderColor = activeFocusedBackgroundColor;
				}
			}
			else
			{
				if (pressed)
				{
					backgroundColor = normalPressedBackgroundColor;
					borderColor = normalFocusedBackgroundColor;
				}
				else if (hovered)
				{
					backgroundColor = normalHoveredBackgroundColor;
					borderColor = normalFocusedBackgroundColor;
				}
				else if (selected)
				{
					borderColor = normalFocusedBackgroundColor;
				}
			}

			this.label.color = textColor;

			this.rectangle.ShapeProperties.FillColor = backgroundColor;
			this.rectangle.ShapeProperties.OutlineColor = borderColor;
			this.rectangle.ForceMeshUpdate();

		}

		/// <inheritdoc />
		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			pressed = true;
			UpdateRectangle();
		}

		/// <inheritdoc />
		public void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			pressed = false;
			UpdateRectangle();
		}

		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{
			hovered = true;
			UpdateRectangle();
		}

		/// <inheritdoc />
		public void OnPointerExit(PointerEventData eventData)
		{
			hovered = false;
			UpdateRectangle();
		}

		/// <inheritdoc />
		public void OnSelect(BaseEventData eventData)
		{
			selected = true;
			UpdateRectangle();
		}

		/// <inheritdoc />
		public void OnDeselect(BaseEventData eventData)
		{
			selected = false;
			UpdateRectangle();
		}
	}
}