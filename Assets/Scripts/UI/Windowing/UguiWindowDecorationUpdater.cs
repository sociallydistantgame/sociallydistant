#nullable enable
using System;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Windowing
{
	public class UguiWindowDecorationUpdater : MonoBehaviour
	{
		private Image decoratorImage = null!;
		private UguiWindow window = null!;
		private bool isActive;
		
		[Header("dependencies")]
		[SerializeField]
		private WindowFocusService focusService = null!;

		[Header("Configuration")]
		[SerializeField]
		private Sprite unskinnedActiveDecoration = null!;
		
		[SerializeField]
		private Sprite unskinnedInactiveDecoration = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(UguiWindowDecorationUpdater));
			
			this.MustGetComponent(out decoratorImage);
			this.MustGetComponentInParent(out window);
		}

		private void Update()
		{
			bool wasActive = isActive;
			bool isActiveNow = ReferenceEquals(focusService.FocusedWindow, window);
			if (wasActive != isActiveNow)
			{
				isActive = isActiveNow;
				UpdateDecorations();
			}
		}

		private void UpdateDecorations()
		{
			this.decoratorImage.sprite = isActive
				? unskinnedActiveDecoration
				: unskinnedInactiveDecoration;
		}
	}
}