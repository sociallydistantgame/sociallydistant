#nullable enable

using System;
using AcidicGui.Common;
using UnityEngine;

namespace AcidicGui.Transitions
{
	public abstract class TransitionController :
		MonoBehaviour,
		IShowOrHide
	{
		/// <inheritdoc />
		public abstract bool IsVisible { get; }

		/// <inheritdoc />
		public abstract void Show(Action? callback = null);

		/// <inheritdoc />
		public abstract void Hide(Action? callback = null);
	}
}