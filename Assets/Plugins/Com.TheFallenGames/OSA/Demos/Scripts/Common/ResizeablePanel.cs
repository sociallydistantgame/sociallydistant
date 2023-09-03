using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using frame8.Logic.Misc.Other.Extensions;
using UnityEngine.Events;

namespace Com.TheFallenGames.OSA.Demos.Common
{
	public class ResizeablePanel : MonoBehaviour
	{
		[SerializeField]
		bool _Expanded = false;

		[Tooltip("Only needed to be set if starting with _Expanded=false")]
		[SerializeField]
		float _ExpandedSize = 0f;

		[Tooltip("Only needed to be set if starting with _Expanded=true")]
		[SerializeField]
		float _NonExpandedSize = 0f;

		[SerializeField]
		float _AnimTime = 1f;

		[SerializeField]
		Direction _Direction = Direction.HORIZONTAL;

		[SerializeField]
		bool _RebuildNearestScrollRectParentDuringAnimation = false;

		[SerializeField]
		UnityEventBool onExpandedStateChanged = null;


		float PreferredSize
		{
			get { return _Direction == Direction.HORIZONTAL ? _LayoutElement.preferredWidth : _LayoutElement.preferredHeight; }
			set { if (_Direction == Direction.HORIZONTAL) _LayoutElement.preferredWidth = value; else _LayoutElement.preferredHeight = value; }
		}


		LayoutElement _LayoutElement;
		ScrollRect _NearestScrollRectInParents;
		//bool _Animating;

		void Start()
		{
			Canvas.ForceUpdateCanvases();
			_LayoutElement = GetComponent<LayoutElement>();

			if (_Expanded)
			{
				if (_ExpandedSize == -1f)
					_ExpandedSize = PreferredSize;
			}
			else
			{
				if (_NonExpandedSize == -1f)
					_NonExpandedSize = PreferredSize;
			}

			var p = transform;
			while ((p = p.parent) && !_NearestScrollRectInParents)
				_NearestScrollRectInParents = p.GetComponent<ScrollRect>();
		}


		public void ToggleExpandedState()
		{
			bool expandedToSet = !_Expanded;
			float from = PreferredSize, to;
			if (expandedToSet)
			{
				to = _ExpandedSize;
			}
			else
			{
				to = _NonExpandedSize;
			}
			StartCoroutine(StartAnimating(from, to, () => { _Expanded = expandedToSet; if (onExpandedStateChanged != null) onExpandedStateChanged.Invoke(_Expanded); }));
		}

		IEnumerator StartAnimating(float from, float to, Action onDone)
		{
			float startTime = Time.unscaledTime;
			float elapsed;
			float t01;
			do
			{
				yield return null; // one frame

				elapsed = Time.unscaledTime - startTime;
				t01 = elapsed / _AnimTime;
				if (t01 > 1f)
					t01 = 1f;
				else
					t01 = Mathf.Sqrt(t01); // slightly fast-in, slow-out effect

				PreferredSize = from * (1f - t01) + to * t01;
				if (_RebuildNearestScrollRectParentDuringAnimation && _NearestScrollRectInParents)
					_NearestScrollRectInParents.OnScroll(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current));
			}
			while (t01 < 1f);

			if (onDone != null)
				onDone();
		}


		public enum Direction { HORIZONTAL, VERTICAL }


		[Serializable]
		public class UnityEventBool : UnityEvent<bool> { }
	}
}
