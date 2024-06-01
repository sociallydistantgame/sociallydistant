#nullable enable
using System;
using UnityEngine;
using UnityExtensions;

namespace UI.Shell.Dock
{
	public sealed class NotificationIndicator : MonoBehaviour
	{
		[SerializeField]
		private bool isBeingShown = false;
		
		private CanvasGroup canvasGroup;
		private LTDescr? animation;
		
		public bool IsVisible
		{
			get => isBeingShown;
			set
			{
				if (isBeingShown == value)
					return;

				isBeingShown = value;
				UpdateAnimation();
			}
		}
		
		private void Awake()
		{
			this.MustGetComponent(out canvasGroup);
			canvasGroup.alpha = isBeingShown ? 1 : 0;
		}

		private void UpdateAnimation()
		{
			// TODO: Fade animation
			canvasGroup.alpha = isBeingShown ? 1 : 0;
		}
	}
}