using UnityEngine;
using frame8.Logic.Misc.Visual.UI;
using frame8.Logic.Misc.Other.Extensions;
using System;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.Demos.Hierarchy;

namespace Com.TheFallenGames.OSA.Demos.HierarchyWithStickyHeaders
{
	/// <summary>Only works on vertical scroll views</summary>
	public class OSAHierarchyStickyHeader : MonoBehaviour
	{
		[SerializeField]
		RectTransform _HeaderPrefab = null;

		[SerializeField]
		[Tooltip("Set to false to have the header on the same depth as the directory it represents. Otherwise, it'll be displayed as a top-level directory for any sub-directory")]
		bool _HeaderFixedPadding = true;

		[SerializeField]
		[Tooltip("1 = the parent of the first item (or non-expanded directory) item will be shown.\n" +
			"2 = (not implemented yet) another header, behind the 1st one will be shown when applicable (when the directory represented by the header also has a parent).\n" +
			"This generalizes to the N headers case. \n" +
			"Important: This is a work in progress, and only 1 header can be used at once for now")]
		int _MaxNestedHeaders = 1;

		const int MAX_SUPPORTED_HEADERS = 10;

		IHierarchyOSA _HierarchyOSA;
		IHierarchyParams _Params;
		Type _VHType;

		// First one (index 0) is the deepest, next one is its parent etc.
		IHierarchyNodeViewsHolder[] _Headers;
		IHierarchyNodeModel _CurrentHeaderDirModel;

		bool _AtLeastOneActivated;


		#region Unity
		void Update()
		{
			if (_HierarchyOSA == null) // not ready yet
				return;

			if (_Headers.Length == 0)
				return;

			ManageHeaders();
		}
		#endregion

		/// <summary>If the header should have a different model-to-view binding logic, use this</summary>
		public void InitWithCustomItemViewsHolder<TCustomItemViewsHolder>(IHierarchyOSA hierarchyOSA)
			where TCustomItemViewsHolder : AbstractViewsHolder, IHierarchyNodeViewsHolder
		{
			Init(hierarchyOSA, typeof(TCustomItemViewsHolder));
		}

		public void InitWithSameItemViewsHolder(IHierarchyOSA hierarchyOSA)
		{
			Init(hierarchyOSA, hierarchyOSA.GetViewsHolderType());
		}

		void Init(IHierarchyOSA hierarchyOSA, Type itemVHType)
		{
			_HierarchyOSA = hierarchyOSA;
			_VHType = itemVHType;
			_Params = _HierarchyOSA.BaseParameters as IHierarchyParams;

			_HierarchyOSA.ScrollPositionChanged += OnScrollPositionChanged;

			if (_MaxNestedHeaders > MAX_SUPPORTED_HEADERS)
			{
				Debug.Log(GetType().Name + " has MaxNestedHeaders set to " + _MaxNestedHeaders + ", but maximum allowed is " + MAX_SUPPORTED_HEADERS + ". Clamping...");
				_MaxNestedHeaders = MAX_SUPPORTED_HEADERS;
			}
			else if (_MaxNestedHeaders < 0)
			{
				Debug.Log(GetType().Name + " has MaxNestedHeaders set to " + _MaxNestedHeaders + ", which is negative. Setting to 0...");
				_MaxNestedHeaders = 0;
			}

			_Headers = new IHierarchyNodeViewsHolder[_MaxNestedHeaders];
			for (int i = 0; i < _MaxNestedHeaders; i++)
			{
				_Headers[i] = CreateAndStoreHeader(i);
			}
		}

		IHierarchyNodeViewsHolder CreateAndStoreHeader(int i)
		{
			var header = Activator.CreateInstance(_VHType) as IHierarchyNodeViewsHolder;
			var headerAsAbstractVH = header as AbstractViewsHolder;
			// Index is never stored. it's retrieved only on expand/collapse, since it's currently expensive to get it
			headerAsAbstractVH.Init(_HeaderPrefab, _HierarchyOSA.BaseParameters.Viewport, -1);
			headerAsAbstractVH.root.SetAsLastSibling();
			headerAsAbstractVH.root.name = "Header" + i;
			// Positioning a views holder. Taken from OSA.AddViewsHolderAndMakeVisible() method in OSAInternal.cs (15-Feb-19, 16:33)
			var layoutInfo = _HierarchyOSA.GetLayoutInfoReadonly();
			headerAsAbstractVH.root.anchorMin = headerAsAbstractVH.root.anchorMax = layoutInfo.constantAnchorPosForAllItems;
			header.SetOnToggleFoldoutListener(OnHeaderFoldOutClicked);
			SetHeaderInset(header, 0f);
			DeactivateVH(header);

			return header;
		}

		void OnScrollPositionChanged(double position)
		{
			// TODO header positioning can be done here instead of in Update, but it requires a lot of testing priorly
		}

		void OnHeaderFoldOutClicked(IHierarchyNodeViewsHolder headerVH)
		{
			if (_CurrentHeaderDirModel == null)
				throw new OSAException("Model null");

			var list = _Params.FlattenedVisibleHierarchy;
			int index = list.IndexOf(_CurrentHeaderDirModel);

			if (index == -1)
				throw new OSAException("Model not found: " + _CurrentHeaderDirModel.Depth + ", " + (_CurrentHeaderDirModel.Children == null ? "null children" : _CurrentHeaderDirModel.Children.Length + " children"));

			_HierarchyOSA.ToggleDirectoryFoldout(index);
			UpdateHeaderViews(headerVH, _CurrentHeaderDirModel);
		}

		void ActivateVH(IHierarchyNodeViewsHolder header)
		{
			(header as AbstractViewsHolder).root.gameObject.SetActive(true);
			_AtLeastOneActivated = true;
		}

		void DeactivateVH(IHierarchyNodeViewsHolder header)
		{
			(header as AbstractViewsHolder).root.gameObject.SetActive(false);
		}

		void UpdateHeaderViews(IHierarchyNodeViewsHolder header, IHierarchyNodeModel model)
		{
			int prevDepth = model.Depth;
			if (_HeaderFixedPadding)
				model.Depth = 1;

			header.UpdateViews(model);

			if (_HeaderFixedPadding)
				model.Depth = prevDepth;
		}

		void SetHeaderInset(IHierarchyNodeViewsHolder header, float inset)
		{
			var layoutInfo = _HierarchyOSA.GetLayoutInfoReadonly();

			// Positioning a views holder. Taken from OSA.AddViewsHolderAndMakeVisible() method in OSAInternal.cs (15-Feb-19, 16:33)
			var asAbstractVH = (header as AbstractViewsHolder);
			asAbstractVH.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(layoutInfo.startEdge, inset, _HierarchyOSA.BaseParameters.DefaultItemSize);

			if (layoutInfo.transversalPaddingContentStart == -1d)
				throw new OSAException("transversalPaddingContentStart is not allowed to be -1 when using " + typeof(OSAHierarchyStickyHeader).Name);

			asAbstractVH.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(layoutInfo.transvStartEdge, (float)layoutInfo.transversalPaddingContentStart, (float)layoutInfo.itemsConstantTransversalSize);
		}

		void DeactivateAll()
		{
			for (int i = 0; i < _Headers.Length; i++)
			{
				var vh = _Headers[i];
				DeactivateVH(vh);
			}

			_AtLeastOneActivated = false;
		}

		void AactivateAll()
		{
			for (int i = 0; i < _Headers.Length; i++)
			{
				var vh = _Headers[i];
				ActivateVH(vh);
			}
		}

		void ManageHeader(IHierarchyNodeViewsHolder header, IHierarchyNodeViewsHolder firstVH, IHierarchyNodeViewsHolder secondVH)
		{
			var firstAsAbstractVH = firstVH as AbstractViewsHolder;
			int itemIndex = firstVH.ItemIndex;
			var firstVisibleItemModel = _Params.FlattenedVisibleHierarchy[itemIndex];
			var parentDirOfFirstVisibleItem = firstVisibleItemModel.Parent;

			if (firstVisibleItemModel.IsDirectory() && firstVisibleItemModel.Expanded)
			{
				if (_CurrentHeaderDirModel == null || _CurrentHeaderDirModel != firstVisibleItemModel)
				{
					_CurrentHeaderDirModel = firstVisibleItemModel;
					UpdateHeaderViews(header, _CurrentHeaderDirModel);
				}

				float amountBeforeViewport = -_HierarchyOSA.GetItemRealInsetFromParentStart(firstAsAbstractVH.root);
				//Debug.Log(amountBeforeViewport);
				bool beforeViewport = amountBeforeViewport > 0;
				if (beforeViewport)
				{
					ActivateVH(header);
					UpdateHeaderViews(header, _CurrentHeaderDirModel);
					SetHeaderInset(header, 0f);
				}
				else
					DeactivateVH(header);
			}
			else
			{
				//Debug.Log(firstVisibleItemModel.title + ", " + firstVisibleItemModel.IsDirectory + ", " + firstVisibleItemModel.depth + ", parent= " + (parentDirOfFirstVisibleItem == null ? "null" : parentDirOfFirstVisibleItem.title + ", d " + parentDirOfFirstVisibleItem.depth));
				if (parentDirOfFirstVisibleItem == null || parentDirOfFirstVisibleItem.Depth == 0)
				{
					DeactivateVH(header);
					return;
				}

				ActivateVH(header);

				if (_CurrentHeaderDirModel != parentDirOfFirstVisibleItem)
				{
					_CurrentHeaderDirModel = parentDirOfFirstVisibleItem;
					UpdateHeaderViews(header, parentDirOfFirstVisibleItem);
				}

				SetHeaderInset(header, 0f);
			}

			var secondAsAbstractVH = secondVH as AbstractViewsHolder;
			int secondItemIndex = secondVH.ItemIndex;
			var secondVisibleItemModel = _Params.FlattenedVisibleHierarchy[secondItemIndex];

			if (secondVisibleItemModel.IsDirectory())
			{
				bool secondIsSameDepth = secondVisibleItemModel.Depth == _CurrentHeaderDirModel.Depth;

				if (!secondIsSameDepth)
				{
					if (!secondVisibleItemModel.Expanded)
						return;

					if (secondVisibleItemModel.Parent != _CurrentHeaderDirModel)
					{
						bool secondIsSmallerDepth = secondVisibleItemModel.Depth < _CurrentHeaderDirModel.Depth;
						if (!secondIsSmallerDepth)
							return;
					}
				}

				float secondItemInset = _HierarchyOSA.GetItemRealInsetFromParentStart(secondAsAbstractVH.root);
				if (secondItemInset < 0) // wait for positions to be fixed
					return;

				var layoutInfo = _HierarchyOSA.GetLayoutInfoReadonly();
				var headerAsAbstractVH = header as AbstractViewsHolder;
				var size = headerAsAbstractVH.root.rect.size[layoutInfo.hor0_vert1];
				var insetMax0 = Mathf.Min(0f, (float)(secondItemInset /*- layoutInfo.spacing*/ - size));
				SetHeaderInset(header, insetMax0);
			}
		}

		void ManageHeaders()
		{
			// Nothing to show
			var firstVH = _HierarchyOSA.GetBaseItemViewsHolder(0) as IHierarchyNodeViewsHolder;
			if (firstVH == null)
			{
				if (_AtLeastOneActivated)
					DeactivateAll();
				return;
			}

			// Sticky headers are not needed when you can't scroll
			if (_HierarchyOSA.GetContentSizeToViewportRatio() <= 1f)
			{
				if (_AtLeastOneActivated)
					DeactivateAll();
				return;
			}

			// Second vh not present => TODO, if needed
			var secondVH = _HierarchyOSA.GetBaseItemViewsHolder(1) as IHierarchyNodeViewsHolder;
			if (secondVH == null)
			{
				if (_AtLeastOneActivated)
					DeactivateAll();
				return;
			}

			var deepestHeader = _Headers[0];
			ManageHeader(deepestHeader, firstVH, secondVH);

			for (int i = 1; i < _Headers.Length; i++)
			{
				var vh = _Headers[i];
				DeactivateVH(vh);
			}
		}
	}
}
