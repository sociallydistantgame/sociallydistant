#nullable enable
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace UI.Popovers
{
	public class Popover : 
		MonoBehaviour,
		IPointerEnterHandler,
		IPointerExitHandler,
		IPointerMoveHandler
	{
		private RectTransform myTransform;
		private bool isDisplayingPopOver = false;
		private float timeUntilDisplay = 0;
		private bool isCountingDown = false;
		private PopoverLayer? popoverLayer;
		
		[SerializeField]
		private string text = string.Empty;

		[SerializeField]
		private float delaySeconds = 0.1f;
		
		public string Text
		{
			get => text;
			set => text = value;
		}

		private void Awake()
		{
			this.MustGetComponent(out myTransform);
		}

		private void OnDisable()
		{
			if (isDisplayingPopOver && popoverLayer != null)
				popoverLayer.HidePopover();

			isCountingDown = false;
			isDisplayingPopOver = false;
			popoverLayer = null;
		}

		private void Update()
		{
			if (popoverLayer == null)
				return;
			
			if (isDisplayingPopOver)
				return;

			if (!isCountingDown)
				return;

			if (timeUntilDisplay > 0)
			{
				timeUntilDisplay -= Time.deltaTime;
				return;
			}

			isCountingDown = false;
			isDisplayingPopOver = true;
			popoverLayer.ShowPopover(myTransform, text);
		}

		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{
			PopoverLayer? layer = FindObjectOfType<PopoverLayer>();

			if (layer == null)
				return;

			this.popoverLayer = layer;
			this.isCountingDown = true;
			this.timeUntilDisplay = delaySeconds;
		}

		/// <inheritdoc />
		public void OnPointerExit(PointerEventData eventData)
		{
			if (isDisplayingPopOver && popoverLayer != null)
				popoverLayer.HidePopover();

			isCountingDown = false;
			isDisplayingPopOver = false;
			popoverLayer = null;
		}

		/// <inheritdoc />
		public void OnPointerMove(PointerEventData eventData)
		{
			if (isDisplayingPopOver)
				return;

			if (!isCountingDown)
				return;

			timeUntilDisplay = delaySeconds;
		}
	}
}