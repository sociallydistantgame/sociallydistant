using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Com.TheFallenGames.OSA.Core.Data.Animations
{
	[Serializable]
	public abstract class AnimationCancellingEvents
	{
		[SerializeField]
		[Tooltip("Whether to cancel on any count change event: Insert/Remove/Reset")]
		bool _OnCountChanges = true;
		/// <summary>Whether to cancel on any count change event: Insert/Remove/Reset</summary>
		public bool OnCountChanges { get { return _OnCountChanges; } set { _OnCountChanges = value; } }

		[SerializeField]
		[Tooltip("Whether to cancel on any event that changes sizes of the items or content")]
		bool _OnSizeChanges = true;
		/// <summary>Whether to cancel on any event that changes sizes of the items or content</summary>
		public bool OnSizeChanges { get { return _OnSizeChanges; } set { _OnSizeChanges = value; } }

		[SerializeField]
		[Tooltip("Whether to cancel on OSA.ScrollTo")]
		bool _OnScrollTo = true;
		/// <summary>Whether to cancel on <see cref="OSA{TParams, TItemViewsHolder}.ScrollTo(int, float, float)"/></summary>
		public bool OnScrollTo { get { return _OnScrollTo; } set { _OnScrollTo = value; } }
	}
}
