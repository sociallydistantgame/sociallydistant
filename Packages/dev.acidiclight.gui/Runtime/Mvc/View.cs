using System;
using AcidicGui.Common;
using AcidicGui.Transitions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace AcidicGui.Mvc
{
	[RequireComponent(typeof(TransitionController))]
	public abstract class View :
		UIBehaviour,
		IView
	{
		private TransitionController visibilityController = null!;

		/// <inheritdoc />
		public bool IsVisible => visibilityController;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.MustGetComponent(out visibilityController);
			this.Hide();
			base.Awake();
		}

		/// <inheritdoc />
		public void Show(Action? callback = null)
		{
			visibilityController.Show(() => OnShow(callback));
		}

		/// <inheritdoc />
		public void Hide(Action? callback = null)
		{
			visibilityController.Hide(() => OnHide(callback));
		}

		protected virtual void OnShow(Action? callback = null)
		{
			callback?.Invoke();
		}

		protected virtual void OnHide(Action? callback = null)
		{
			callback?.Invoke();
		}
	}
}