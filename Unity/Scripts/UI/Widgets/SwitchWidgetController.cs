#nullable enable

using ThisOtherThing.UI.Shapes;
using AcidicGui.Widgets;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using ThisOtherThing.UI.Shapes;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Core;
using TrixelCreative.TrixelAudio.Data;
using UnityExtensions;

namespace UI.Widgets
{
	public class SwitchWidgetController : 
		WidgetController,
		IPointerEnterHandler,
		IPointerExitHandler,
		IPointerDownHandler,
		IPointerUpHandler,
		ISelectHandler,
		IDeselectHandler
	{
		[Header("Sound")]
		
		private SoundEffectAsset switchSound = null!;

		
		private SoundEffectAsset switchOffSound = null!;
		
		[Header("UI")]
		
		private Toggle toggle = null!;

		
		private Rectangle background = null!;

		
		private RectTransform nub = null!;

		[Header("Background - When Off")]
		
		private ColorBlock backgroundWhenOff;
		
		[Header("Background - When On")]
		
		private ColorBlock backgroundWhenOn;
		
		[Header("Border - When Off")]
		
		private ColorBlock borderWhenOff;
		
		[Header("Border - When On")]
		
		private ColorBlock borderWhenOn;

		private LTDescr? borderAnimation;
		private LTDescr? bgAnimation;
		private LTDescr? nubAnimation = null;

		private bool hovered;
		private bool pressed;
		private bool selected;
		
		public bool IsActive { get; set; }
		
		public Action<bool>? Callback { get; set; }

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SwitchWidgetController));
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			this.toggle.SetIsOnWithoutNotify(IsActive);
			this.toggle.onValueChanged.AddListener(OnToggleSwitched);
			
			UpdateVisuals();
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			hovered = false;
			pressed = false;
			selected = false;
			
			this.toggle.onValueChanged.RemoveListener(OnToggleSwitched);
			Callback = null;
		}

		private void OnToggleSwitched(bool value)
		{
			AudioManager.PlaySound(value ? switchSound : switchOffSound);
            
			UpdateVisuals(true);
			Callback?.Invoke(value);
		}

		private void UpdateNubPosition(Vector3 position)
		{
			this.nub.anchoredPosition3D = position;
		}

		private void OnNubAnimationCompleted()
		{
			nubAnimation = null;
		}

		private Color GetBorderColor(bool isOn)
		{
			ColorBlock block = isOn ? borderWhenOn : borderWhenOff;
			return GetColorState(block);
		}

		private Color GetBackgroundColor(bool isOn)
		{
			ColorBlock block = isOn ? backgroundWhenOn : backgroundWhenOff;
			return GetColorState(block);
		}

		private Color GetColorState(ColorBlock block)
		{
			if (!enabled)
				return block.disabledColor;

			if (selected)
				return block.selectedColor;

			if (pressed)
				return block.pressedColor;

			if (hovered)
				return block.highlightedColor;
			
			return block.normalColor;
		}

		private void OnBackgroundAnimationCompleted()
		{
			bgAnimation = null;
		}

		private void OnBorderAnimationCompleted()
		{
			borderAnimation = null;
		}

		private void UpdateBorder(Color color)
		{
			background.ShapeProperties.OutlineColor = color;
			background.ForceMeshUpdate();
		}

		private void UpdateBackground(Color color)
		{
			background.ShapeProperties.FillColor = color;
			background.ForceMeshUpdate();
		}
		
		private void UpdateVisuals(bool animate = false)
		{
			bool toggleValue = toggle.isOn;

			var nubDestination = new Vector3(
				(toggleValue) ? 8 : -8,
				0,
				0
			);

			
			
			Color bgDestination = GetBackgroundColor(toggleValue);
			Color borderDestination = GetBorderColor(toggleValue);
			
			if (animate)
			{
				float nubAnimationTime = 0.2f;
				float colorDuration = 0.2f;

				if (bgAnimation != null)
				{
					colorDuration = bgAnimation.time - bgAnimation.passed;
					LeanTween.cancel(bgAnimation.id);
					bgAnimation = null;
				}

				if (borderAnimation != null)
				{
					LeanTween.cancel(borderAnimation.id);
					borderAnimation = null;
				}
				
				if (nubAnimation != null)
				{
					nubAnimationTime = nubAnimation.passed;
					LeanTween.cancel(nubAnimation.id);
					nubAnimation = null;
				}
				
				nubAnimation = LeanTween.value(nub.gameObject, UpdateNubPosition, nub.anchoredPosition3D, nubDestination, nubAnimationTime)
					.setEase(LeanTweenType.easeOutCubic)
					.setOnComplete(OnNubAnimationCompleted);
				
				bgAnimation = LeanTween.value(this.background.gameObject, UpdateBackground, background.ShapeProperties.FillColor, bgDestination, colorDuration)
					.setEase(LeanTweenType.easeOutCubic)
					.setOnComplete(OnBackgroundAnimationCompleted);
				
				borderAnimation = LeanTween.value(this.background.gameObject, UpdateBorder, background.ShapeProperties.OutlineColor, borderDestination, colorDuration)
					.setEase(LeanTweenType.easeOutCubic)
					.setOnComplete(OnBorderAnimationCompleted);
				
				
			}
			else
			{
				this.background.ShapeProperties.FillColor = bgDestination;
				this.background.ShapeProperties.OutlineColor = borderDestination;
				nub.anchoredPosition3D = nubDestination;

				this.background.ForceMeshUpdate();
			}
		}

		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{
			hovered = true;
			UpdateVisuals(true);
		}

		/// <inheritdoc />
		public void OnPointerExit(PointerEventData eventData)
		{
			hovered = false;
			UpdateVisuals(true);
		}

		/// <inheritdoc />
		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			pressed = true;
			UpdateVisuals();
		}

		/// <inheritdoc />
		public void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			pressed = false;
			UpdateVisuals();
		}

		/// <inheritdoc />
		public void OnSelect(BaseEventData eventData)
		{
			selected = true;
			UpdateVisuals();
		}

		/// <inheritdoc />
		public void OnDeselect(BaseEventData eventData)
		{
			selected = false;
			UpdateVisuals(true);
		}
	}
}