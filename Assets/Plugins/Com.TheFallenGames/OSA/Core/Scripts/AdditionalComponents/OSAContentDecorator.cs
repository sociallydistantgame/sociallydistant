using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using frame8.Logic.Misc.Other.Extensions;
using frame8.Logic.Misc.Visual.UI;

namespace Com.TheFallenGames.OSA.AdditionalComponents
{
	/// <summary>
	/// Very useful script when you want to attach arbitrary content anywhere in an OSA and have it scrollable as any other item.
	/// Needs to be attached to a child of OSA's Viewport.
	/// <para>Note: If you use Unity 2019.3.5f1 (probably there are other buggy versions as well), this won't work properly if you don't bring the anchors together in the scrolling direction. 
	/// It's a bug in Unity where a RectTransform's size isn't correctly reported in Awake(), and it affects all UI components, not only this one</para>
	/// </summary>
	public class OSAContentDecorator : MonoBehaviour
	{
		[SerializeField]
		InsetEdgeEnum _InsetEdge = InsetEdgeEnum.START;

		[SerializeField]
		[Tooltip("How far from the InsetEdge should this be positioned. Can be normalized ([0, 1]), if InsetIsNormalized=true, or raw. Default is 0")]
		float _Inset = 0f;

		[SerializeField]
		[Tooltip("If false, will interpret the Inset property as the raw distance from InsetEdge, rather than the normalized inset relative to the content's full size")]
		bool _InsetIsNormalized = true;

		[SerializeField]
		bool _DisableWhenNotVisible = true;

		[SerializeField]
		[Tooltip("If false, won't be dragged together with the OSA's content when it's pulled when already at the scrolling limit")]
		bool _AffectedByElasticity = false;

		[SerializeField]
		[Tooltip(
			"Sets when the OSA's padding from InsetEdge will be controlled to be the same as this object's size.\n" +
			"Once at initialization, or adapting continuously, or none (i.e. you'll probably set OSA's padding manually, in case the decorator shouldn't overlap with items).\n" +
			"Only works if Inset is 0")]
		ControlOSAPaddingMode _ControlOSAPaddingAtInsetEdge = ControlOSAPaddingMode.ONCE_AT_INIT;

		[SerializeField]
		[Tooltip("If null, will use the first an implementation of IOSA found in parents")]
		RectTransform _OSATransform = null;

		RectTransform _ParentRT;
		RectTransform _RT;
		IOSA _OSA;
		bool _Initialized;
		double _LastKnownInset;
		double _MyLastKnownSize;
		RectOffset _OSALastKnownPadding = new RectOffset();


		/// <summary>
		/// Only to be called if OSA is initialized manually via <see cref="OSA{TParams, TItemViewsHolder}.Init"/>. Call it before that.
		/// With the default setup, where OSA initializes itself in its Start(), you don't need to call this, as it's called from this.Awake()
		/// </summary>
		public void Init()
		{
			_RT = transform as RectTransform;
			_ParentRT = _RT.parent as RectTransform;
			if (_OSA == null)
			{
				_OSA = _ParentRT.GetComponentInParent(typeof(IOSA)) as IOSA;
				if (_OSA == null)
					throw new OSAException("Component implementing IOSA not found in parents");
			}
			else
			{
				_OSA = _OSATransform.GetComponent(typeof(IOSA)) as IOSA;
				if (_OSA == null)
					throw new OSAException("Component implementing IOSA not found on the specified object '" + _OSATransform.name + "'");
			}

			if (_OSA.BaseParameters.Viewport != _ParentRT)
				throw new OSAException(typeof(OSAContentDecorator).Name + " can only work when attached to a direct child of OSA's Viewport.");

			if (_ControlOSAPaddingAtInsetEdge != ControlOSAPaddingMode.DONT_CONTROL)
			{
				if (_OSA.IsInitialized)
				{
					Debug.Log(
						"OSA's content padding can't be set after OSA was initialized. " +
						"You're most probably calling OSA.Init manually(), in which case make sure to also manually call Init() on this decorator, before OSA.Init()"
					);
				}
				else
				{
					SetOSAPadding();
				}
			}

			_OSA.ScrollPositionChanged += OSAScrollPositionChanged;

			// Improvement 14.09.2020: this was limiting - each user should be able to set of their own anchors for maximum flexibility. OSA should only control the decorator's position
			//var aPos = _RT.localPosition;
			//_RT.anchorMin = _RT.anchorMax = new Vector2(0f, 1f); // top-right
			//_RT.localPosition = aPos;

			_Initialized = true;
		}


		void Awake()
		{
			if (!_Initialized)
				Init();

			gameObject.SetActive(false);
		}

		void Update()
		{
			if (_ControlOSAPaddingAtInsetEdge == ControlOSAPaddingMode.ADAPTIVE)
				AdaptToPadding();
		}

		public void SetInset(float newInset)
		{
			_Inset = newInset;

			if (_OSA != null && _Initialized)
				OSAScrollPositionChanged(0d);
		}

        private void OnRectTransformDimensionsChange()
        {
            
        }

        public void AdaptToPadding()
		{
			if (_ControlOSAPaddingAtInsetEdge != ControlOSAPaddingMode.ADAPTIVE)
				return;

			if (_OSA == null || !_OSA.IsInitialized) // make sure adapter wasn't disposed
				return;

			var rect = _RT.rect;
			var li = _OSA.GetLayoutInfoReadonly();
			double mySize = rect.size[li.hor0_vert1];

			// Update OSA's padding when either the decorator's size changes or when OSA's own padding is externally changed
			if (_LastKnownInset != _Inset || _MyLastKnownSize != mySize || !IsSamePadding(_OSALastKnownPadding, _OSA.BaseParameters.ContentPadding))
			{
				SetPadding(mySize);
				// Commented: updating sooner is better than later
				//_OSA.ScheduleForceRebuildLayout();
				_OSA.ForceRebuildLayoutNow();
			}
		}

		void SetOSAPadding()
		{
			var rect = _RT.rect;
			// Commented: layout info may not be available
			//var li = _OSA.GetLayoutInfoReadonly();

			double mySize = rect.size[_OSA.IsHorizontal ? 0 : 1];
			SetPadding(mySize);
		}

		void SetPadding(double myNewSize)
		{
			var p = _OSA.BaseParameters;
			var paddingToSet = myNewSize + _Inset;
			int paddingToSetCeiling = (int)(paddingToSet + .6f);
			var pad = p.ContentPadding;
			if (_InsetEdge == InsetEdgeEnum.START)
			{
				if (p.IsHorizontal)
					pad.left = paddingToSetCeiling;
				else
					pad.top = paddingToSetCeiling;
			}
			else
			{
				if (p.IsHorizontal)
					pad.right = paddingToSetCeiling;
				else
					pad.bottom = paddingToSetCeiling;
			}
			_LastKnownInset = _Inset;
			_MyLastKnownSize = myNewSize;
			_OSALastKnownPadding = new RectOffset(pad.left, pad.right, pad.top, pad.bottom);
		}

		bool IsSamePadding(RectOffset a, RectOffset b) 
		{ 
			return
				a.left == b.left &&
				a.right == b.right &&
				a.top == b.top &&
				a.bottom == b.bottom; 
		}

		void OSAScrollPositionChanged(double scrollPos)
		{
			// The terms 'before' and 'after' mean what they should, if _InsetEdge is START,
			// but their meaning is swapped when _InsetEdge is END.

			var li = _OSA.GetLayoutInfoReadonly();
			double osaInsetFromEdge;
			RectTransform.Edge edgeToInsetFrom;
			if (_InsetEdge == InsetEdgeEnum.START)
			{
				osaInsetFromEdge = _OSA.ContentVirtualInsetFromViewportStart;
				edgeToInsetFrom = li.startEdge;
			}
			else
			{
				osaInsetFromEdge = _OSA.ContentVirtualInsetFromViewportEnd;
				edgeToInsetFrom = li.endEdge;
			}

			double myExpectedInsetFromVirtualContent = _Inset;
			var rect = _RT.rect;
			double mySize = rect.size[li.hor0_vert1];
			double osaViewportSize = li.vpSize;
			if (_InsetIsNormalized)
			{
				myExpectedInsetFromVirtualContent *= _OSA.GetContentSize() - mySize;
			}

			double myExpectedInsetFromViewport = osaInsetFromEdge + myExpectedInsetFromVirtualContent;
			bool visible = true;
			if (myExpectedInsetFromViewport < 0d)
			{
				if (myExpectedInsetFromViewport <= -mySize) // completely 'before' the viewport
				{
					myExpectedInsetFromViewport = -mySize; // don't position it too far away
					visible = false;
				}
			}
			else
			{
				if (myExpectedInsetFromViewport >= osaViewportSize) // completely 'after' the viewport
				{
					myExpectedInsetFromViewport = osaViewportSize; // don't position it too far away
					visible = false;
				}
			}

			bool disable = false;
			if (!visible)
				disable = _DisableWhenNotVisible;

			if (gameObject.activeSelf == disable)
				gameObject.SetActive(!disable);

			if (disable)
				// No need to position it, since it's disabled now
				return;

			if (!_AffectedByElasticity)
			{
				// If OSA's Content is pulled outside bounds (elasticity)
				if (osaInsetFromEdge > .1d)
				{
					if (myExpectedInsetFromViewport > 0d)
					{
						// Update: actually, it looks better to just keep it at the edge, no matter what
						// // only if the content is bigger than viewport, otherwise the decorator is forced to stay with the content
						//if (_OSA.GetContentSizeToViewportRatio() > 1d) 
						//{

						//}

						// Bugfix 30.09.2020: Actually, the decorator should take _Inset into account, no matter what
						//myExpectedInsetFromViewport = 0d;
						myExpectedInsetFromViewport = _Inset;
					}
				}
			}

			_RT.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(edgeToInsetFrom, (float)myExpectedInsetFromViewport, (float)mySize);
		}


		public enum InsetEdgeEnum
		{
			START,
			END
		}


		public enum ControlOSAPaddingMode
		{
			DONT_CONTROL,
			ONCE_AT_INIT,
			ADAPTIVE
		}
	}
}
