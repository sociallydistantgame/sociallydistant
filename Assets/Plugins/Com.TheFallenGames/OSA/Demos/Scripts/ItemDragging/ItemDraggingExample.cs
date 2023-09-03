using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using frame8.Logic.Misc.Visual.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.Util.ItemDragging;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Com.TheFallenGames.OSA.Demos.ItemDragging
{
	/// <summary>
	/// </summary>
	public class ItemDraggingExample : OSA<MyParams, MyViewsHolder>, DraggableItem.IDragDropListener, ICancelHandler
	{
		public SimpleDataHelper<MyModel> Data { get; private set; }

		DragStateManager _DragManager = new DragStateManager();
		Canvas _Canvas;
		RectTransform _CanvasRT;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<MyModel>(this);

			base.Start();

			_Canvas = GetComponentInParent<Canvas>();
			_CanvasRT = _Canvas.transform as RectTransform;
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			if (_DragManager.State == DragState.DRAGGING)
				DoAutoScrolling();
		}

		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);

			if (changeMode != ItemCountChangeMode.RESET)
				return;

			if (count == 0)
				return;

			// Randomize sizes
			int indexOfFirstItemThatWillChangeSize = 0;
			int end = indexOfFirstItemThatWillChangeSize + count;

			itemsDesc.BeginChangingItemsSizes(indexOfFirstItemThatWillChangeSize);
			for (int i = indexOfFirstItemThatWillChangeSize; i < end; ++i)
				itemsDesc[i] = UnityEngine.Random.Range(_Params.DefaultItemSize / 3, _Params.DefaultItemSize * 3);
			itemsDesc.EndChangingItemsSizes();
		}

		/// <inheritdoc/>
		protected override MyViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			instance.draggableComponent.dragDropListener = this;

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			MyModel model = Data[newOrRecycled.ItemIndex];

			bool isPlaceholder = _DragManager.State != DragState.NONE && model == _DragManager.PlaceholderModel;
			newOrRecycled.scalableViews.gameObject.SetActive(!isPlaceholder);
			if (!isPlaceholder)
			{
				newOrRecycled.titleText.text = "[id:" + model.id + "] " + model.title;
				newOrRecycled.background.color = model.color;
			}
		}
		#endregion

		#region DraggableItem.IDragDropListener
		bool DraggableItem.IDragDropListener.OnPrepareToDragItem(DraggableItem item)
		{
			var dragged = GetItemViewsHolderIfVisible(item.RT);
			if (dragged == null)
				return false;

			int index = dragged.ItemIndex;
			var modelOfDragged = Data[index];
			Debug.Log("Dragging with id " + modelOfDragged.id + ", ItemIndex " + index);

			// Modifying the list manually, because RemoveItemWithViewsHolder will do a Remove itself
			Data.List.RemoveAt(index);

			var normPosBefore = GetNormalizedPosition();

			RemoveItemWithViewsHolder(dragged, true, false);
			UpdateScaleOfVisibleItems(dragged);

			// Insert back an empty slot in its place, with the same size
			var emptyModel = new EmptyModel();
			_DragManager.EnterState_PreparingForDrag(dragged, modelOfDragged, emptyModel);

			InsertPlaceholderAtNewIndex(index);

			// Using the normPos before the item replacement so the content doesn't unexpectedly jump,
			// which was happening when the content was scrolled near end
			SetNormalizedPosition(normPosBefore);

			return true;
		}

		void DraggableItem.IDragDropListener.OnBeginDragItem(PointerEventData eventData)
		{
			_DragManager.EnterState_Dragging(eventData);
		}
		
		void DraggableItem.IDragDropListener.OnDraggedItem(PointerEventData eventData)
		{
			bool isInDragSpace;
			var closestVH = GetClosestVHAtScreenPoint(eventData, out isInDragSpace);
			if (closestVH == null)
			{
				// TODO if needed
			}
			
			_DragManager.Dragged.background.color = isInDragSpace ? _DragManager.ModelOfDragged.color : _DragManager.ModelOfDragged.color * _Params.outsideColorTint;

			UpdateScaleOfVisibleItems(closestVH);

			if (closestVH == null)
				return;

			int closestVHIndex = closestVH.ItemIndex;
			int placeholderIndex = _DragManager.PlaceholderModel.placeholderForIndex;
			if (!_DragManager.TryPrepareSwap(closestVHIndex, eventData.delta))
				return;

			var normPosBefore = GetNormalizedPosition();

			Data.RemoveOne(placeholderIndex);
			InsertPlaceholderAtNewIndex(closestVHIndex);
			_DragManager.RegisterSwap(placeholderIndex, eventData.delta);

			// Using the normPos before the item replacement so the content doesn't unexpectedly jump,
			// which was happening when the content was scrolled near end
			SetNormalizedPosition(normPosBefore);
		}

		DraggableItem.OrphanedItemBundle DraggableItem.IDragDropListener.OnDroppedItem(PointerEventData eventData)
		{
			var orphaned = DropDraggedVHAndEnterNoneState(eventData);
			return orphaned;
		}

		bool DraggableItem.IDragDropListener.OnDroppedExternalItem(PointerEventData eventData, DraggableItem orphanedItemWithBundle)
		{
			bool grabbed = TryGrabOrphanedItemVH(eventData, orphanedItemWithBundle);
			return grabbed;
		}
		#endregion

		void ICancelHandler.OnCancel(BaseEventData eventData)
		{
			if (_DragManager.State == DragState.NONE)
				return;

			_DragManager.Dragged.draggableComponent.CancelDragSilently();

			DropDraggedVHAndEnterNoneState(null);
		}

		void DoAutoScrolling()
		{
			// Can't scroll when the content is too small
			if (this.GetContentSizeToViewportRatio() < 1f)
				return;

			Vector2 localPointInDragSpace;
			if (!GetLocalPointInDragSpaceIfWithinBounds(
					_DragManager.Dragged.draggableComponent.CurrentOnDragEventWorldPosition,
					_DragManager.Dragged.draggableComponent.CurrentPressEventCamera,
					out localPointInDragSpace
				)
			)
				return;

			// 0=start(top, in our case), 1=end(bottom)
			float abstrPoint01 = ConvertLocalPointToLongitudinalPointStart0End1(_Params.DragSpaceRectTransform, localPointInDragSpace);

			//Debug.Log(localPointInScrollView + ", " + abstrPoint01.ToString("####.####"));

			abstrPoint01 = Mathf.Clamp01(abstrPoint01);
			float scrollAbstrDeltaInCTSpace = _Params.maxScrollSpeedOnBoundary * DeltaTime;
			float startEdgeLimit01 = _Params.minDistFromEdgeToBeginScroll01;
			float endEdgeLimit01 = 1f - _Params.minDistFromEdgeToBeginScroll01;

			if (abstrPoint01 < startEdgeLimit01)
				ScrollByAbstractDelta(scrollAbstrDeltaInCTSpace * (startEdgeLimit01 - abstrPoint01) / _Params.minDistFromEdgeToBeginScroll01); // scroll towards start
			else if (abstrPoint01 > endEdgeLimit01)
				ScrollByAbstractDelta(-scrollAbstrDeltaInCTSpace * (abstrPoint01 - endEdgeLimit01) / _Params.minDistFromEdgeToBeginScroll01); // towards end
		}

		void InsertPlaceholderAtNewIndex(int index)
		{
			_DragManager.PlaceholderModel.placeholderForIndex = index;
			Data.InsertOne(index, _DragManager.PlaceholderModel);
			RequestChangeItemSizeAndUpdateLayout(index, _DragManager.Dragged.root.rect.height, false, true, true);
		}

		void UpdateScaleOfVisibleItems(MyViewsHolder vhToRotate)
		{
			for (int i = 0; i < VisibleItemsCount; i++)
			{
				var vh = GetItemViewsHolder(i);
				vh.scalableViews.localScale = Vector3.one * (vh == vhToRotate ? .98f : 1f);
			}
		}

		bool TryGrabOrphanedItemVH(PointerEventData eventData, DraggableItem orphanedItemWithBundle)
		{
			bool isPointInDragSpace;
			var closestVH = GetClosestVHAtScreenPoint(eventData, out isPointInDragSpace);
			if (!isPointInDragSpace)
				return false;

			orphanedItemWithBundle.dragDropListener = this;

			int atIndex;
			if (closestVH == null) // no items present, but the point is in drag space => add it to the list
				atIndex = 0;
			else
				atIndex = closestVH.ItemIndex;

			var vh = orphanedItemWithBundle.OrphanedBundle.views as MyViewsHolder;
			var model = orphanedItemWithBundle.OrphanedBundle.model as MyModel;
			Debug.Log("Dropped with id " + model.id + ", ItemIndex " + vh.ItemIndex + ", droppedAtItemIndex " + atIndex);

			// Modifying the list manually, because InsertItemWithViewsHolder will do an Insert itself
			Data.List.Insert(atIndex, model);

			float itemSizeToUse = vh.root.rect.height; // for vertical ScrollViews, "size" = height
			InsertItemWithViewsHolder(vh, atIndex, false);

			// The adapter will use _Params.DefaultItemSize to set the item's size, but we want it to keep the same size.
			// The alternative way of doing it (a bit faster, but negligeable) is to store <itemSizeToUse> somewhere and 
			// pass it in CollectItemsSizes for <atIndex>. CollectItemsSizes() is always called during count changes, including during InsertItemWithViewsHolder()
			RequestChangeItemSizeAndUpdateLayout(atIndex, itemSizeToUse, false, true);

			//// Restore the scale of all items;
			//UpdateScaleOfVisibleItems(null);

			return true;
		}

		DraggableItem.OrphanedItemBundle DropDraggedVHAndEnterNoneState(PointerEventData eventData)
		{
			var dragged = _DragManager.Dragged;
			var modelOfDragged = _DragManager.ModelOfDragged;
			dragged.background.color = modelOfDragged.color;

			int atIndex;
			int placeholderIndexToBeRemovedFrom = _DragManager.PlaceholderModel.placeholderForIndex;;
			if (eventData == null)
			{
				// No event means something was canceled => put it back at the same index;
				atIndex = dragged.ItemIndex;

				// Removing the placeholder item before
				Data.RemoveOne(placeholderIndexToBeRemovedFrom);
				placeholderIndexToBeRemovedFrom = -1;
			}
			else
			{
				bool isPointInDragSpace;
				GetClosestVHAtScreenPoint(eventData, out isPointInDragSpace);

				if (!isPointInDragSpace)
				{
					Debug.Log("Orphaned item (dropped outside) with id " + modelOfDragged.id + ", ItemIndex " + dragged.ItemIndex);
					var orphaned = new DraggableItem.OrphanedItemBundle
					{
						model = modelOfDragged,
						views = dragged,
						previousOwner = this
					};
					dragged.draggableComponent.dragDropListener = null;

					// Removing the placeholder item
					Data.RemoveOne(placeholderIndexToBeRemovedFrom);

					_DragManager.EnterState_None();

					// Restore the scale of all items;
					UpdateScaleOfVisibleItems(null);

					return orphaned;
				}

				// The placeholder item  is shifted down as a result of the prior Insert operation below
				atIndex = placeholderIndexToBeRemovedFrom;
				placeholderIndexToBeRemovedFrom += 1;
			}

			Debug.Log("Dropped with id " + modelOfDragged.id + ", ItemIndex " + dragged.ItemIndex + ", droppedAtItemIndex " + atIndex);

			// Modifying the list manually, because InsertItemWithViewsHolder will do an Insert itself
			Data.List.Insert(atIndex, modelOfDragged);

			// Before important changes in the view state, enter a clean state to prepare for other drags
			// and store the dragged in a local var (since it'll be removed from the manager)
			_DragManager.EnterState_None();

			float itemSizeToUse = dragged.root.rect.height; // for vertical ScrollViews, "size" = height
			InsertItemWithViewsHolder(dragged, atIndex, false);

			// The adapter will use _Params.DefaultItemSize to set the item's size, but we want it to keep the same size.
			// The alternative way of doing it (a bit faster, but negligeable) is to store <itemSizeToUse> somewhere and 
			// pass it in CollectItemsSizes for <atIndex>. CollectItemsSizes() is always called during count changes, including during InsertItemWithViewsHolder()
			RequestChangeItemSizeAndUpdateLayout(atIndex, itemSizeToUse, false, true);

			// Restore the scale of all items;
			UpdateScaleOfVisibleItems(null);

			if (placeholderIndexToBeRemovedFrom != -1)
			{
				// Removing the placeholder item after insert
				Data.RemoveOne(placeholderIndexToBeRemovedFrom);
			}

			return null;
		}

		MyViewsHolder GetClosestVHAtScreenPoint(PointerEventData eventData, out bool isPointInDragSpace)
		{
			Vector2 localPoint;
			isPointInDragSpace = false;
			if (!GetLocalPointInDragSpaceIfWithinBounds(eventData.position, eventData.pressEventCamera, out localPoint))
				return null;

			isPointInDragSpace = true;

			// Passing .5f so that this uses the item's center to calculate which one is the closest
			float _;
			var vh = GetViewsHolderClosestToViewportPoint(_Canvas, _CanvasRT, localPoint, .5f, out _);
			if (vh == null)
				return null;

			return vh;
		}

		bool GetLocalPointInDragSpace(Vector2 screenPoint, Camera camera, out Vector2 localPoint)
		{ return RectTransformUtility.ScreenPointToLocalPointInRectangle(_Params.DragSpaceRectTransform, screenPoint, camera, out localPoint); }

		bool GetLocalPointInDragSpaceIfWithinBounds(Vector2 screenPoint, Camera camera, out Vector2 localPoint)
		{ return GetLocalPointInDragSpace(screenPoint, camera, out localPoint) && _Params.DragSpaceRectTransform.IsLocalPointInRect(localPoint); }
	}


	[Serializable]
	public class MyModel
	{
		public int id;
		public string title;
		public Color color;
	}


	class EmptyModel : MyModel
	{
		public int placeholderForIndex;
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		[Range(0f, 1f)]
		[Tooltip("This is normalized (0-1) relative to the dragSpace's size")]
		public float minDistFromEdgeToBeginScroll01 = .2f;
		public float maxScrollSpeedOnBoundary = 3000f;
		public Color outsideColorTint = new Color(1f, 1f, 1f, .8f);
		[Tooltip("Use the SCROLL_VIEW option in case the Viewport is smaller than the ScrollView and auto-scrolling should work on its entire area")]
		public DragSpace dragSpace = DragSpace.VIEWPORT;

		public RectTransform DragSpaceRectTransform { get { return dragSpace == DragSpace.SCROLL_VIEW ? ScrollViewRT : Viewport; } }


		public override void InitIfNeeded(IOSA iAdapter)
		{
			base.InitIfNeeded(iAdapter);

			var graphic = DragSpaceRectTransform.GetComponent<Graphic>();
			if (!graphic || !graphic.raycastTarget)
			{
				throw new OSAException("DragSpaceRectTransform should have a Graphic component (Image, RawImage etc.) with raycastTarget=true");
			}
		}
	}


	public class MyViewsHolder : BaseItemViewsHolder
	{
		public Text titleText;

		// Using a child that helps us scale the item's views, because the scale of the item itself is managed exclusively by the adapter
		public DraggableItem draggableComponent;
		public RectTransform scalableViews;
		public Image background;


		public override void CollectViews()
		{
			base.CollectViews();

			draggableComponent = root.GetComponent<DraggableItem>();
			root.GetComponentAtPath("ScalableViews", out scalableViews);
			scalableViews.GetComponentAtPath("TitlePanel/TitleText", out titleText);
			scalableViews.GetComponentAtPath("Background", out background);
		}
	}


	public enum DragSpace
	{
		VIEWPORT,
		SCROLL_VIEW
	}


	class DragStateManager
	{
		public MyViewsHolder Dragged { get; private set; }
		public MyModel ModelOfDragged { get; private set; }
		public DragState State { get; private set; }
		public EmptyModel PlaceholderModel { get; private set; }

		int LastSwappedItemAtIndex { get; set; }
		Vector2 LastSwapDragEventDelta { get; set; }
		bool PreventSwapWithLastSwappedItem { get; set; }


		public void EnterState_None()
		{
			Dragged = null;
			ModelOfDragged = null;
			PlaceholderModel = null;
			LastSwappedItemAtIndex = -1;
			PreventSwapWithLastSwappedItem = false;
			LastSwapDragEventDelta = Vector2.zero;
			State = DragState.NONE;
		}

		public void EnterState_PreparingForDrag(MyViewsHolder dragged, MyModel modelOfDragged, EmptyModel placeholderModel)
		{
			Dragged = dragged;
			ModelOfDragged = modelOfDragged;
			PlaceholderModel = placeholderModel;

			State = DragState.PREPARING_FOR_DRAG;
		}

		public void EnterState_Dragging(PointerEventData eventData)
		{
			State = DragState.DRAGGING;
		}

		public void RegisterSwap(int placeholderIndexBeforeSwap, Vector2 dragEventDelta)
		{
			LastSwappedItemAtIndex = placeholderIndexBeforeSwap;
			LastSwapDragEventDelta = dragEventDelta;
			PreventSwapWithLastSwappedItem = true;
		}

		/// <summary>Returns whether the swap is possible/allowed or not</summary>
		public bool TryPrepareSwap(int closestVHIndex, Vector2 dragEventDelta)
		{
			if (closestVHIndex == PlaceholderModel.placeholderForIndex)
				return false;

			// When changing the direction of drag, allow the last swapped item to be repositioned
			if (Vector2.Dot(LastSwapDragEventDelta, dragEventDelta) < 0)
			{
				PreventSwapWithLastSwappedItem = false;
				return true;
			}

			if (PreventSwapWithLastSwappedItem)
			{
				// Prevent glitch of infinite swapping of items by making sure closestVH wasn't the last one swapped
				if (closestVHIndex == LastSwappedItemAtIndex)
					return false;
			}

			return true;
		}
	}


	enum DragState
	{
		NONE,
		PREPARING_FOR_DRAG,
		DRAGGING,
	}
}