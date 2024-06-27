using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Classic
{
    /// <summary>Class (initially) implemented during this YouTube tutorial: https://youtu.be/aoqq_j-aV8I (which is now too old to relate). It demonstrates a simple use case with items that expand/collapse on click</summary>
    public abstract class ClassicSRIA<TViewsHolder> : MonoBehaviour where TViewsHolder : CAbstractViewsHolder
	{
		#region Config
		public RectTransform viewport;
		#endregion

		public List<TViewsHolder> viewsHolders = new List<TViewsHolder>();
		public ScrollRect ScrollRectComponent { get; private set; }
		public LayoutGroup ContentLayoutGroup { get; private set; }
		public RectTransform ScrollRectRT { get; private set; }
		public bool IsHorizontal { get { return ScrollRectComponent.horizontal; } }
		/// <summary> 1f = start; 0f = end </summary>
		public float AbstractNormalizedPosition
		{
			get { return IsHorizontal ? 1f - ScrollRectComponent.horizontalNormalizedPosition : ScrollRectComponent.verticalNormalizedPosition; }
			set { if (IsHorizontal) ScrollRectComponent.horizontalNormalizedPosition = 1f - value; else ScrollRectComponent.verticalNormalizedPosition = value; }
		}
		public float ContentSize { get { return ScrollRectComponent.content.rect.size[_OneIfVertical_ZeroIfHorizontal]; } }
		public float ViewportSize { get { return viewport.rect.size[_OneIfVertical_ZeroIfHorizontal]; } }
		public virtual RectOffset Padding { get { return ContentLayoutGroup == null ? _ZeroRectOffset : ContentLayoutGroup.padding; } }

		RectOffset _ZeroRectOffset = new RectOffset();
		int _OneIfHorizontal_ZeroIfVertical, _OneIfVertical_ZeroIfHorizontal;
		Coroutine _SmoothScrollToCoroutine;


		protected virtual void Awake() { }

		protected virtual void Start()
		{
			ScrollRectComponent = GetComponent<ScrollRect>();
			ContentLayoutGroup = ScrollRectComponent.content.GetComponent<LayoutGroup>();
			ScrollRectRT = transform as RectTransform;
			if (!viewport)
				viewport = ScrollRectRT;

			_OneIfHorizontal_ZeroIfVertical = ScrollRectComponent.horizontal ? 1 : 0;
			_OneIfVertical_ZeroIfHorizontal = 1 - _OneIfHorizontal_ZeroIfVertical;
		}

		protected virtual void OnBeforeDestroyViewsHolder(TViewsHolder vh)
		{ }
		
		protected virtual void Update()
		{

		}
		protected virtual void OnDestroy()
		{
			if (_SmoothScrollToCoroutine != null)
			{
				StopCoroutine(_SmoothScrollToCoroutine);
				_SmoothScrollToCoroutine = null;
			}
		}

		public virtual void Refresh() { ChangeItemsCount(0, viewsHolders.Count); }

		/// <summary>It clears any previously cached views holders</summary>
		public virtual void ResetItems(int itemsCount, bool contentPanelEndEdgeStationary = false)
		{ ChangeItemsCount(0, itemsCount, -1, contentPanelEndEdgeStationary); }

		/// <summary>It preserves previously cached views holders</summary>
		public virtual void InsertItems(int index, int itemsCount, bool contentPanelEndEdgeStationary = false)
		{ ChangeItemsCount(1, itemsCount, index, contentPanelEndEdgeStationary); }

		/// <summary>It preserves previously cached views holders</summary>
		public virtual void RemoveItems(int index, int itemsCount, bool contentPanelEndEdgeStationary = false)
		{ ChangeItemsCount(2, itemsCount, index, contentPanelEndEdgeStationary); }

		/// <summary> Utility to smooth scroll. The scroll is animated (scroll is done gradually, throughout multiple frames) </summary>
		/// <returns>If content size smaller or equal to viweport size, returns false. Else, returns if no smooth scroll animation was already playing or <paramref name="overrideCurrentScrollingAnimation"/> is true</returns>
		public virtual bool SmoothScrollTo(int itemIndex, float duration = .75f, float normalizedOffsetFromViewportStart = 0f, float normalizedPositionOfItemPivotToUse = 0f, Action onDone = null, bool overrideCurrentScrollingAnimation = false)
		{
			if (ContentSize <= ViewportSize)
				return false;

			if (_SmoothScrollToCoroutine != null)
			{
				if (!overrideCurrentScrollingAnimation)
					return false;

				StopCoroutine(_SmoothScrollToCoroutine);
				_SmoothScrollToCoroutine = null;
			}

			duration = Mathf.Clamp(duration, 0.001f, 100f);

			var vh = viewsHolders[itemIndex];
			float itemSize = vh.root.rect.size[_OneIfVertical_ZeroIfHorizontal];
			RectTransform.Edge edge;
			if (ScrollRectComponent.horizontal)
				edge = RectTransform.Edge.Left;
			else
				edge = RectTransform.Edge.Top;
			var contentRT = ScrollRectComponent.content;
			float insetFromParentStart = vh.root.GetInsetFromParentEdge(contentRT, edge);
			float initialContentInsetFromViewportStart = contentRT.GetInsetFromParentEdge(viewport, edge);
			float targetContentInsetFromViewportStart = -insetFromParentStart;
			targetContentInsetFromViewportStart += ViewportSize * normalizedOffsetFromViewportStart;
			targetContentInsetFromViewportStart -= itemSize * normalizedPositionOfItemPivotToUse;
			float scrollableArea = ContentSize - ViewportSize;
			targetContentInsetFromViewportStart = Mathf.Clamp(targetContentInsetFromViewportStart, -scrollableArea, 0f);

			_SmoothScrollToCoroutine = StartCoroutine(SetContentInsetFromViewportStart(edge, initialContentInsetFromViewportStart, targetContentInsetFromViewportStart, duration, onDone));
			return true;
		}

		IEnumerator SetContentInsetFromViewportStart(RectTransform.Edge edge, float fromInset, float toInset, float duration, Action onDone)
		{
			float startTime = Time.time;
			float curElapsed, t01;
			bool inProgress = true;
			do
			{
				yield return null;
				if (_SmoothScrollToCoroutine == null)
					yield break;

				curElapsed = Time.time - startTime;
				t01 = curElapsed / duration;
				if (t01 >= 1f)
				{
					t01 = 1f;

					inProgress = false;
				}
				else
					t01 = Mathf.Sin(t01 * Mathf.PI / 2); // normal in, sin slow out
				float curInset = Mathf.Lerp(fromInset, toInset, t01);
				ScrollRectComponent.content.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(edge, curInset, ContentSize);
			}
			while (inProgress);
			_SmoothScrollToCoroutine = null;

			if (onDone != null)
				onDone();
		}

		/// <param name="changeMode">0=remove, 1=insert, 2=reset</param>
		void ChangeItemsCount(int changeMode, int itemsCount, int indexIfAppendingOrRemoving = -1, bool contentPanelEndEdgeStationary = false)
		{
			float ctInsetBefore;
			RectTransform.Edge startEdge, endEdge;
			if (IsHorizontal)
			{
				startEdge = RectTransform.Edge.Left;
				endEdge = RectTransform.Edge.Right;
			}
			else
			{
				startEdge = RectTransform.Edge.Top;
				endEdge = RectTransform.Edge.Bottom;
			}

			RectTransform.Edge edgeToInsetFrom;
			ctInsetBefore = ScrollRectComponent.content.GetInsetFromParentEdge(viewport, edgeToInsetFrom = (contentPanelEndEdgeStationary ? endEdge : startEdge));

			switch (changeMode)
			{
				case 0: // reset
					RemoveItemsAndUpdateIndices(0, viewsHolders.Count);
					AddItemsAndUpdateIndices(0, itemsCount);
					break;
				case 1: // insert
					AddItemsAndUpdateIndices(indexIfAppendingOrRemoving, itemsCount);
					break;
				case 2: // remove
					RemoveItemsAndUpdateIndices(indexIfAppendingOrRemoving, itemsCount);
					break;
			}

			Canvas.ForceUpdateCanvases();
			ScrollRectComponent.content.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(edgeToInsetFrom, ctInsetBefore, ContentSize);
		}

		void RemoveItemsAndUpdateIndices(int index, int count)
		{
			int rem = count;
			while (rem-- > 0)
			{
				// SOCIALLY DISTANT: Needed for WidgetList recycling to work
				OnBeforeDestroyViewsHolder(viewsHolders[index]);
				
				Destroy(viewsHolders[index].root.gameObject);
				viewsHolders.RemoveAt(index);
			}

			for (int i = index; i < viewsHolders.Count; ++i)
			{
				var vh = viewsHolders[i];
				vh.ItemIndex -= count;
				UpdateViewsHolder(vh);
			}
		}

		void AddItemsAndUpdateIndices(int index, int count)
		{
			int lastIndexExcl = index + count;
			for (int i = index; i < lastIndexExcl; ++i)
			{
				var vh = CreateViewsHolder(i);
				viewsHolders.Insert(i, vh);
				vh.root.SetParent(ScrollRectComponent.content, false);
				vh.root.SetSiblingIndex(i);
				UpdateViewsHolder(vh);
			}

			// Increase the index of next items
			for (int i = lastIndexExcl; i < viewsHolders.Count; ++i)
			{
				var vh = viewsHolders[i];
				vh.ItemIndex += count;
				UpdateViewsHolder(vh);
			}
		}



		protected abstract TViewsHolder CreateViewsHolder(int itemIndex);

		protected abstract void UpdateViewsHolder(TViewsHolder vh);
	}


	static class RectTransformExtensions
	{
		static Dictionary<RectTransform.Edge, Func<RectTransform, RectTransform, float>> _GetInsetFromParentEdge_MappedActions =
			new Dictionary<RectTransform.Edge, Func<RectTransform, RectTransform, float>>()
		{
			{ RectTransform.Edge.Bottom, GetInsetFromParentBottomEdge },
			{ RectTransform.Edge.Top, GetInsetFromParentTopEdge },
			{ RectTransform.Edge.Left, GetInsetFromParentLeftEdge },
			{ RectTransform.Edge.Right, GetInsetFromParentRightEdge }
		};

		static Dictionary<RectTransform.Edge, Action<RectTransform, RectTransform, float, float>> _SetInsetAndSizeFromParentEdgeWithCurrentAnchors_MappedActions =
			new Dictionary<RectTransform.Edge, Action<RectTransform, RectTransform, float, float>>()
		{
			{
				RectTransform.Edge.Bottom,
				(child, parentHint, newInset, newSize) => {
					var offsetChange = newInset - child.GetInsetFromParentBottomEdge(parentHint);
					var offsetMin = new Vector2(child.offsetMin.x, child.offsetMin.y + offsetChange); // need to store it before modifying anything, because the offsetmax will change the offsetmin and vice-versa
					child.offsetMax = new Vector2(child.offsetMax.x, child.offsetMax.y + (newSize - child.rect.height + offsetChange));
					child.offsetMin = offsetMin;
				}
			},
			{
				RectTransform.Edge.Top,
				(child, parentHint, newInset, newSize) => {
					var offsetChange = newInset - child.GetInsetFromParentTopEdge(parentHint);
					var offsetMax = new Vector2(child.offsetMax.x, child.offsetMax.y - offsetChange);
					child.offsetMin = new Vector2(child.offsetMin.x, child.offsetMin.y - (newSize - child.rect.height + offsetChange));
					child.offsetMax = offsetMax;
				}
			},
			{
				RectTransform.Edge.Left,
				(child, parentHint, newInset, newSize) => {
					var offsetChange = newInset - child.GetInsetFromParentLeftEdge(parentHint);
					var offsetMin = new Vector2(child.offsetMin.x + offsetChange, child.offsetMin.y);
					child.offsetMax = new Vector2(child.offsetMax.x + (newSize - child.rect.width + offsetChange), child.offsetMax.y);
					child.offsetMin = offsetMin;
				}
			},
			{
				RectTransform.Edge.Right,
				(child, parentHint, newInset, newSize) => {
					var offsetChange = newInset - child.GetInsetFromParentRightEdge(parentHint);
					var offsetMax = new Vector2(child.offsetMax.x - offsetChange, child.offsetMax.y);
					child.offsetMin = new Vector2(child.offsetMin.x - (newSize - child.rect.width + offsetChange), child.offsetMin.y);
					child.offsetMax = offsetMax;
				}
			}
		};

		/// <summary>
		/// It assumes the transform has a parent
		/// </summary>
		/// <param name="child"></param>
		/// <param name="parentHint"> the parent of child. used in order to prevent casting, in case the caller already has the parent stored in a variable</param>
		/// <returns></returns>
		public static float GetInsetFromParentTopEdge(this RectTransform child, RectTransform parentHint)
		{
			float parentPivotYDistToParentTop = (1f - parentHint.pivot.y) * parentHint.rect.height;
			float childLocPosY = child.localPosition.y;

			return parentPivotYDistToParentTop - child.rect.yMax - childLocPosY;
		}

		public static float GetInsetFromParentBottomEdge(this RectTransform child, RectTransform parentHint)
		{
			float parentPivotYDistToParentBottom = parentHint.pivot.y * parentHint.rect.height;
			float childLocPosY = child.localPosition.y;

			return parentPivotYDistToParentBottom + child.rect.yMin + childLocPosY;
		}

		public static float GetInsetFromParentLeftEdge(this RectTransform child, RectTransform parentHint)
		{
			float parentPivotXDistToParentLeft = parentHint.pivot.x * parentHint.rect.width;
			float childLocPosX = child.localPosition.x;

			return parentPivotXDistToParentLeft + child.rect.xMin + childLocPosX;
		}

		public static float GetInsetFromParentRightEdge(this RectTransform child, RectTransform parentHint)
		{
			float parentPivotXDistToParentRight = (1f - parentHint.pivot.x) * parentHint.rect.width;
			float childLocPosX = child.localPosition.x;

			return parentPivotXDistToParentRight - child.rect.xMax - childLocPosX;
		}

		public static float GetInsetFromParentEdge(this RectTransform child, RectTransform parentHint, RectTransform.Edge parentEdge)
		{ return _GetInsetFromParentEdge_MappedActions[parentEdge](child, parentHint); }

		/// <summary> NOTE: Use the optimized version if parent is known </summary>
		public static void SetSizeFromParentEdgeWithCurrentAnchors(this RectTransform child, RectTransform.Edge fixedEdge, float newSize)
		{
			var par = child.parent as RectTransform;
			child.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(par, fixedEdge, child.GetInsetFromParentEdge(par, fixedEdge), newSize);
		}

		/// <summary> Optimized version of SetSizeFromParentEdgeWithCurrentAnchors(RectTransform.Edge fixedEdge, float newSize) when parent is known </summary>
		/// <param name="parentHint"></param>
		/// <param name="fixedEdge"></param>
		/// <param name="newSize"></param>
		public static void SetSizeFromParentEdgeWithCurrentAnchors(this RectTransform child, RectTransform parentHint, RectTransform.Edge fixedEdge, float newSize)
		{
			child.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(parentHint, fixedEdge, child.GetInsetFromParentEdge(parentHint, fixedEdge), newSize);
		}

		/// <summary> NOTE: Use the optimized version if parent is known </summary>
		/// <param name="fixedEdge"></param>
		/// <param name="newInset"></param>
		/// <param name="newSize"></param>
		public static void SetInsetAndSizeFromParentEdgeWithCurrentAnchors(this RectTransform child, RectTransform.Edge fixedEdge, float newInset, float newSize)
		{
			child.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(child.parent as RectTransform, fixedEdge, newInset, newSize);
		}

		/// <summary> Optimized version of SetInsetAndSizeFromParentEdgeWithCurrentAnchors(RectTransform.Edge fixedEdge, float newInset, float newSize) when parent is known </summary>
		/// <param name="parentHint"></param>
		/// <param name="fixedEdge"></param>
		/// <param name="newInset"></param>
		/// <param name="newSize"></param>
		public static void SetInsetAndSizeFromParentEdgeWithCurrentAnchors(this RectTransform child, RectTransform parentHint, RectTransform.Edge fixedEdge, float newInset, float newSize)
		{ _SetInsetAndSizeFromParentEdgeWithCurrentAnchors_MappedActions[fixedEdge](child, parentHint, newInset, newSize); }
	}
}
