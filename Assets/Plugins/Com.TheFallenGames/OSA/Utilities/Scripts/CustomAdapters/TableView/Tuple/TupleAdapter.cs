using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using frame8.Logic.Misc.Other.Extensions;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple
{
	public abstract class TupleAdapter<TParams, TTupleValueViewsHolder> : OSA<TParams, TTupleValueViewsHolder>, ITupleAdapter
		where TParams : TupleParams, new()
		where TTupleValueViewsHolder : TupleValueViewsHolder, new()
	{
		public event Action<TupleValueViewsHolder> ValueClicked;
		public event Action<TupleValueViewsHolder, object> ValueChangedFromInput;

		public TupleParams TupleParameters { get { return _Params; } }
		public ITupleAdapterSizeHandler SizeHandler { get; set; }

		public RectTransform RTransform
		{
			get
			{
				if (_RectTransform == null)
					_RectTransform = transform as RectTransform;

				return _RectTransform;
			}
		}

		// Can be null (for example, when data is not available and will be provided later)
		protected ITuple _CurrentTuple;
		protected ITableColumns _ColumnsProvider;
		RectTransform _RectTransform;
		float _MyPrevKnownTransvSize;


		public void ResetWithTuple(ITuple tuple, ITableColumns columnsProvider)
		{
			if (!IsInitialized)
				Init();

			_CurrentTuple = tuple;
			_ColumnsProvider = columnsProvider;
			int columnsCount = _ColumnsProvider.ColumnsCount;

			if (GetItemsCount() == columnsCount)
			{
				// Save massive amounts of performance by just updating the existing views holders rather than resetting the view.
				// Same count means existing items don't need to be enabled/disabled/destroyed, because their position won't change
				var thisAsITupleAdapter = this as ITupleAdapter;
				for (int i = 0; i < VisibleItemsCount; i++)
				{
					var vh = GetItemViewsHolder(i);
					thisAsITupleAdapter.ForceUpdateValueViewsHolder(vh);
				}

				// Force a ComputeVisibility pass, if needed
				//SetNormalizedPosition(GetNormalizedPosition());
			}
			else
				ResetItems(columnsCount);
		}

		/// <summary>
		/// Start was overridden so that Init is not called automatically (see base.Start()), because this is done manually in the first call of ResetWithTuple().
		/// <para>See <see cref="OSA{TParams, TItemViewsHolder}.Start"/></para>
		/// </summary>
		protected sealed override void Start()
		{}

		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			CheckResizing();
		}

		protected override void OnInitialized()
		{
			_MyPrevKnownTransvSize = GetMyCurrentTransversalSize();
			base.OnInitialized();
		}

		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);

			if (changeMode == ItemCountChangeMode.REMOVE || count == 0)
				return;

			int indexOfFirstItemThatWillChangeSize;
			if (changeMode == ItemCountChangeMode.RESET)
				indexOfFirstItemThatWillChangeSize = 0;
			else
				indexOfFirstItemThatWillChangeSize = indexIfInsertingOrRemoving;

			int end = indexOfFirstItemThatWillChangeSize + count;
			itemsDesc.BeginChangingItemsSizes(indexOfFirstItemThatWillChangeSize);
			for (int i = indexOfFirstItemThatWillChangeSize; i < end; i++)
			{
				var size = _ColumnsProvider.GetColumnState(i).CurrentSize;
				bool useDefault = size == -1;
				if (useDefault)
					size = Parameters.DefaultItemSize;

				itemsDesc[i] = size;
			}
			itemsDesc.EndChangingItemsSizes();
		}

		protected override TTupleValueViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new TTupleValueViewsHolder();
			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			vh.SetClickListener(() => OnValueClicked(vh));
			vh.SetValueChangedFromInputListener(value => OnValueChangedFromInput(vh, value));
			
			// Fixing text that disappears because all layout elements and groups are disabled on the prefab when resize mode is none, for optimization purposes
			if (_Params.ResizingMode == TableResizingMode.NONE)
			{
				vh.TextComponent.RT.MatchParentSize(true);
			}

			return vh;
		}

		protected override void UpdateViewsHolder(TTupleValueViewsHolder newOrRecycled)
		{
			object value;
			if (_CurrentTuple == null) // data pending
				value = null;
			else
				value = _CurrentTuple.GetValue(newOrRecycled.ItemIndex);
			newOrRecycled.UpdateViews(value, _ColumnsProvider);
		}

		protected override void OnBeforeDestroyViewsHolder(TTupleValueViewsHolder vh, bool isActive)
		{
			vh.SetValueChangedFromInputListener(null);

			base.OnBeforeDestroyViewsHolder(vh, isActive);
		}

		protected override void OnBeforeRecycleOrDisableViewsHolder(TTupleValueViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);

			if (_Params.ResizingMode == TableResizingMode.AUTO_FIT_TUPLE_CONTENT)
			{
				// Make sure items that will just become visible will be rebuilt shortly
				if (newItemIndex >= 0)
					inRecycleBinOrVisible.HasPendingTransversalSizeChanges = true;
				else
					inRecycleBinOrVisible.HasPendingTransversalSizeChanges = false;
			}
		}

		protected virtual void OnValueClicked(TTupleValueViewsHolder vh)
		{
			if (ValueClicked != null)
				ValueClicked(vh);
		}

		protected virtual void OnValueChangedFromInput(TTupleValueViewsHolder vh, object newValue)
		{
			if (ValueChangedFromInput != null)
				ValueChangedFromInput(vh, newValue);
		}

		void ITupleAdapter.ForceUpdateValueViewsHolderIfVisible(int withItemIndex)
		{
			var vh = GetItemViewsHolderIfVisible(withItemIndex);
			if (vh != null)
				UpdateViewsHolder(vh);
		}

		void ITupleAdapter.ForceUpdateValueViewsHolder(TupleValueViewsHolder vh)
		{
			UpdateViewsHolder(vh as TTupleValueViewsHolder);
		}

		void ITupleAdapter.OnWillBeRecycled(float newSize)
		{
			float transvPad = (float)_InternalState.layoutInfo.transversalPaddingStartPlusEnd;
			bool autoFitEnabled = _Params.ResizingMode == TableResizingMode.AUTO_FIT_TUPLE_CONTENT;
			var axis = (RectTransform.Axis)(1 - _InternalState.hor0_vert1);
			// When this entire tuple will be recycled, reset every vh, visible or in recycle cache
			for (int i = 0; i < VisibleItemsCount; i++)
			{
				var vh = GetItemViewsHolder(i);
				OnBeforeRecycleOrDisableViewsHolder(vh, -1);
				if (autoFitEnabled)
				{
					float valueItemSize = newSize - transvPad;
					vh.root.SetSizeFromParentEdgeWithCurrentAnchors(_Params.Content, _InternalState.transvStartEdge, valueItemSize);
				}
			}
			if (autoFitEnabled)
			{
				for (int i = 0; i < RecyclableItemsCount; i++)
				{
					var vh = _RecyclableItems[i];
					if (autoFitEnabled)
					{
						float valueItemSize = newSize - transvPad;
						// SetSizeWithCurrentAnchors is more efficient when positioning is not important
						vh.root.SetSizeWithCurrentAnchors(axis, valueItemSize);
					}
				}
				for (int i = 0; i < BufferedRecyclableItemsCount; i++)
				{
					var vh = _BufferredRecyclableItems[i];
					if (autoFitEnabled)
					{
						float valueItemSize = newSize - transvPad;
						// SetSizeWithCurrentAnchors is more efficient when positioning is not important
						vh.root.SetSizeWithCurrentAnchors(axis, valueItemSize);
					}
				}
			}
		}

		float GetMyCurrentTransversalSize()
		{
			return _Params.ScrollViewRT.rect.size[1 - _InternalState.hor0_vert1];
		}

		// When a children's size exceeds this adapter's size or all 
		// children become smaller than this adapter (meaning the adapter should be shrunk by the parent)
		void CheckResizing()
		{
			float myTransvSize = GetMyCurrentTransversalSize();
			float biggestSize = myTransvSize;
			float biggestItemTransvSizePlusTransvPadding = 0f;
			bool resizeNeeded = false;

			if (myTransvSize != _MyPrevKnownTransvSize)
			{
				_MyPrevKnownTransvSize = myTransvSize;
				resizeNeeded = true;
			}

			// Check if any item has an even bigger size
			int indexOfBiggestItem = -1;
			float transvPaddingStart = (float)_InternalState.layoutInfo.transversalPaddingContentStart;
			float transvPaddingStartPlusEnd = (float)_InternalState.layoutInfo.transversalPaddingStartPlusEnd;
			bool foundItemBiggerThanMe = false;
			for (int i = 0; i < VisibleItemsCount; i++)
			{
				var vh = GetItemViewsHolder(i);
				RebuildVHIfNeeded(vh);

				float transvSize = vh.root.rect.size[1 - _InternalState.hor0_vert1];
				float transvSizePlusTransvPadding = transvSize + transvPaddingStartPlusEnd;
				if (transvSizePlusTransvPadding > biggestSize)
				{
					biggestSize = transvSizePlusTransvPadding;
					foundItemBiggerThanMe = true;
					indexOfBiggestItem = i;
				}
				if (transvSizePlusTransvPadding > biggestItemTransvSizePlusTransvPadding)
					biggestItemTransvSizePlusTransvPadding = transvSizePlusTransvPadding;
			}

			var vhsSmallerThanBiggestSizeMinusPadding = new List<TTupleValueViewsHolder>();
			for (int i = 0; i < VisibleItemsCount; i++)
			{
				var vh = GetItemViewsHolder(i);
				float transvSize = vh.root.rect.size[1 - _InternalState.hor0_vert1];
				float transvSizePlusTransvPadding = transvSize + transvPaddingStartPlusEnd;

				if (transvSizePlusTransvPadding < biggestSize)
				{
					//Debug.Log(i + ", " + (biggestSize - transvSizePlusTransvPadding) + ", " + transvPaddingStartPlusEnd);
					vhsSmallerThanBiggestSizeMinusPadding.Add(vh);
					//vhsSmallerThanMeSizes.Add(transvSize);
				}
			}

			if (foundItemBiggerThanMe)
			{
				resizeNeeded = true;
			}

			float sizeToSet = biggestSize;
			if (!resizeNeeded)
			{
				// All items are smaller and also this adapter's size didn't change => consider shrinking

				if (biggestItemTransvSizePlusTransvPadding > 0f)
				{
					// Only if it's a significant drop in size
					if (myTransvSize - biggestItemTransvSizePlusTransvPadding > 1f)
					{
						resizeNeeded = true;
						sizeToSet = biggestItemTransvSizePlusTransvPadding;
					}
				}
			}

			if (_Params.ResizingMode == TableResizingMode.AUTO_FIT_TUPLE_CONTENT)
			{
				float itemsSizeToSet = sizeToSet - transvPaddingStartPlusEnd;
				// Resize smaller items to fill the empty space
				for (int i = 0; i < vhsSmallerThanBiggestSizeMinusPadding.Count; i++)
				{
					// The biggest item is already sized correctly, is indexOfBiggestItem is not -1
					if (i == indexOfBiggestItem)
						continue;

					var vh = vhsSmallerThanBiggestSizeMinusPadding[i];
					//Debug.Log(i + ": " + indexOfBiggestItem + ", " + itemsSizeToSet + ", vh " + vh.root.rect.height);
					//_Params.SetPaddingTransvEndToAchieveTansvSizeFor(vh.root, vh.LayoutGroup, itemsSizeToSet);
					vh.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(_InternalState.layoutInfo.transvStartEdge, transvPaddingStart, itemsSizeToSet);
				}
			}

			if (resizeNeeded)
			{
				//Debug.Log(resizeNeeded);
				//if (indexOfBiggestItem != -1)
				//	Debug.Log(", 1 " + biggestItemTransvSizePlusTransvPadding + ", b " + myTransvSize + ", c " + biggestSize + ", d " + indexOfBiggestItem, gameObject);
				if (SizeHandler != null)
					SizeHandler.RequestChangeTransversalSize(this, sizeToSet);
			}
		}

		void RebuildVHIfNeeded(TTupleValueViewsHolder vh)
		{
			if (vh.HasPendingTransversalSizeChanges)
			{
				if (_Params.ResizingMode == TableResizingMode.AUTO_FIT_TUPLE_CONTENT)
				{
					// Only rebuild strings
					if (_ColumnsProvider.GetColumnState(vh.ItemIndex).Info.ValueType == TableValueType.STRING)
					{
						if (vh.CSF)
							ForceRebuildViewsHolder(vh);
					}
				}
				vh.HasPendingTransversalSizeChanges = false;
			}
		}
	}

	public interface ITupleAdapterSizeHandler
	{
		void RequestChangeTransversalSize(ITupleAdapter adapter, double size);
	}
}
