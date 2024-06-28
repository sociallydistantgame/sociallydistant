#nullable enable
using System;
using AcidicGui.Transitions;
using UnityEngine;
using UnityExtensions;

namespace AcidicGui.Common
{
	/// <summary>
	///		A simple component for controlling the visibility of a UI element that integrates well with the animation and MVC systems.
	/// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	public class VisibilityController : TransitionController
	{
		
		private bool isVisible = true;

		private CanvasGroup canvasGroup;
		
		/// <inheritdoc />
		public override bool IsVisible => isVisible;
		
		private void Awake()
		{
			this.MustGetComponent(out canvasGroup);
		}

		/// <inheritdoc />
		public override void Show(Action? callback = null)
		{
			canvasGroup.alpha = 1;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			callback?.Invoke();
		}

		/// <inheritdoc />
		public override void Hide(Action? callback = null)
		{
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			callback?.Invoke();
		}
		
		#if UNITY_EDITOR

		private void OnValidate()
		{
			if (!this.TryGetComponent(out canvasGroup))
				return;

			if (isVisible)
				Show();
			else
				Hide();
		}

#endif
	}
}