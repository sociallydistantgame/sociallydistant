using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;

namespace AcidicGui.Widgets
{
	public sealed class AnimatedHighlight : 
		MonoBehaviour,
		IPointerEnterHandler,
		IPointerExitHandler,
		IPointerDownHandler,
		IPointerUpHandler,
		ISelectHandler,
		IDeselectHandler
	{
		[SerializeField]
		private AnimatedHighlightDriver? driver = null!;

		[SerializeField]
		private float durationInSeconds = 0.1f;
		
		[SerializeField]
		private Color defaultColor;

		[SerializeField]
		private Color hoverColor;

		[SerializeField]
		private Color pressedColor;

		[SerializeField]
		private Color activeColor;

		private bool pressed;
		private bool hovered;
		private bool active;
		private bool selected;
		private LTDescr? animation;
		
		public Color NormalColor
		{
			get => defaultColor;
			set
			{
				defaultColor = value;
				UpdateAnimationState();
			}
		}
		
		public Color HoveredColor
		{
			get => hoverColor;
			set
			{
				hoverColor = value;
				UpdateAnimationState();
			}
		}
		
		public Color PressedColor
		{
			get => pressedColor;
			set
			{
				pressedColor = value;
				UpdateAnimationState();
			}
		}
		
		public Color ActiveColor
		{
			get => activeColor;
			set
			{
				activeColor = value;
				UpdateAnimationState();
			}
		}

		public bool IsActive
		{
			get => active;
			set
			{
				active = value;
				UpdateAnimationState();
			}
		}

		public AnimatedHighlightDriver? Driver
		{
			get => driver;
			set
			{
				if (driver == value)
					return;

				driver = value;
				UpdateAnimationState();
			}
		}
		
		private void Awake()
		{
			if (driver != null)
				driver.CurrentColor = defaultColor;
		}

		private void OnEnable()
		{
			UpdateAnimationState();
		}

		private void OnDisable()
		{
			hovered = false;
			pressed = false;
		}


		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{
			hovered = true;
			UpdateAnimationState();
		}

		/// <inheritdoc />
		public void OnPointerExit(PointerEventData eventData)
		{
			hovered = false;
			UpdateAnimationState();
		}

		/// <inheritdoc />
		public void OnPointerDown(PointerEventData eventData)
		{
			pressed = true;
			UpdateAnimationState();
		}

		/// <inheritdoc />
		public void OnPointerUp(PointerEventData eventData)
		{
			pressed = false;
			UpdateAnimationState();
		}

		/// <inheritdoc />
		public void OnSelect(BaseEventData eventData)
		{
			selected = true;
			UpdateAnimationState();
		}

		/// <inheritdoc />
		public void OnDeselect(BaseEventData eventData)
		{
			selected = false;
			UpdateAnimationState();
		}

		private void AfterAnimation()
		{
			this.animation = null;
		}

		private void UpdateColor(Color newColor)
		{
			if (driver != null)
				this.driver.CurrentColor = newColor;
		}
		
		private void UpdateAnimationState()
		{
			if (this.driver == null)
				return;
			
			float newAnimationDuration = this.durationInSeconds;
			
			if (animation != null)
			{
				newAnimationDuration = animation.time - animation.passed;
				LeanTween.cancel(animation.id);
				animation = null;
			}

			Color destinationColor = this.defaultColor;

			if (pressed)
				destinationColor = pressedColor;
			else if (active || selected)
				destinationColor = activeColor;
			else if (hovered)
				destinationColor = hoverColor;

			if (newAnimationDuration <= 0)
			{
				if (driver != null)
					driver.CurrentColor = destinationColor;
			}
			else
			{
				this.animation = LeanTween.value(this.gameObject, UpdateColor, driver.CurrentColor, destinationColor, newAnimationDuration)
					.setOnComplete(AfterAnimation);
			}
		}
	}
}