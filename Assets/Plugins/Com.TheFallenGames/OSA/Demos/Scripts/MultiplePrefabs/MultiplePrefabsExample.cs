using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.ViewsHolders;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models;
using Com.TheFallenGames.OSA.Util.IO.Pools;
using Com.TheFallenGames.OSA.Util.Animations;

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs
{
	/// <summary>
	/// <para>Example implementation demonstrating the use of 2 different views holders, representing 2 different models into their own prefab, with a common Title property, displayed in a Text found on both prefabs. </para>
	/// <para>The only constrain is for the models to have a common ancestor class and for the views holders to also have a common ancestor class</para>
	/// <para>Also, the <see cref="BidirectionalModel"/> is used to demonstrate how the data can flow from the model to the views, but also from the views to the model (i.e. this model updates when the user changes the value of its corresponding slider)</para>
	/// <para>At the core, everything's the same as in other example implementations, so if something's not clear, check them (SimpleExample is a good start)</para>
	/// </summary>
	public class MultiplePrefabsExample : OSA<MyParams, BaseVH>
	{
		public SimpleDataHelper<BaseModel> Data { get; private set; }

		/// <summary> Used to only allow one item to be expanded at once </summary>
		int _IndexOfCurrentlyExpandedItem;
		ExpandCollapseAnimationState _ExpandCollapseAnimation;
		IPool _ImagesPool;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<BaseModel>(this);

			_ImagesPool = new FIFOCachingPool(50, ImageDestroyer);

			var cancel = _Params.Animation.Cancel;
			// Needed so that CancelUserAnimations() won't be called when sizes change (which happens during our animation itself)
			cancel.UserAnimations.OnCountChanges = false;
			// Needed so that CancelUserAnimations() won't be called on count changes - we're handling these manually by overriding ChangeItemsCount 
			cancel.UserAnimations.OnSizeChanges = false;

			base.Start();
		}

		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			if (_ExpandCollapseAnimation != null)
				AdvanceExpandCollapseAnimation();
		}

		/// <inheritdoc/>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			_IndexOfCurrentlyExpandedItem = -1; // at initialization, and each time the item count changes, this should be invalidated

			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}

		/// <summary>
		/// Creates either a <see cref="BidirectionalVH"/> or a <see cref="ExpandableVH"/>, depending on the type of the model at index <paramref name="itemIndex"/>. 
		/// Calls <see cref="AbstractViewsHolder.Init(RectTransform, int, bool, bool)"/> on it, which instantiates the prefab etc.
		/// </summary>
		/// <seealso cref="OSA{TParams, TItemViewsHolder}.CreateViewsHolder(int)"/>
		protected override BaseVH CreateViewsHolder(int itemIndex)
		{
			var modelType = Data[itemIndex].CachedType;// _ModelTypes[itemIndex];
			if (modelType == typeof(BidirectionalModel)) // very efficient type comparison, since typeof() is evaluated at compile-time
			{
				var vh = new BidirectionalVH();
				vh.Init(_Params.bidirectionalPrefab, _Params.Content, itemIndex);

				return vh;
			}
			else if (modelType == typeof(ExpandableModel))
			{
				var vh = new ExpandableVH();
				vh.Init(_Params.expandablePrefab, _Params.Content, itemIndex);
				vh.expandCollapseButton.onClick.AddListener(() => OnExpandCollapseButtonClicked(vh));
				vh.remoteImageBehaviour.InitializeWithPool(_ImagesPool);

				return vh;
			}

			throw new InvalidOperationException("Unrecognized model type: " + modelType.Name);
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(BaseVH newOrRecycled)
		{
			// Initialize/update the views from the associated model
			BaseModel model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}

		/// <summary>
		/// <para>This is overidden only so that the items' title will be updated to reflect its new index in case of Insert/Remove, because the index is not stored in the model</para>
		/// <para>If you don't store/care about the index of each item, you can omit this</para>
		/// <para>For more info, see <see cref="OSA{TParams, TItemViewsHolder}.OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder, int, bool, int)"/> </para>
		/// </summary>
		protected override void OnItemIndexChangedDueInsertOrRemove(BaseVH shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.UpdateTitleOnly(Data[shiftedViewsHolder.ItemIndex]);
		}

		/// <summary>Overriding the base implementation, which always returns true. In this case, a views holder is recyclable only if its <see cref="BaseVH.CanPresentModelType(Type)"/> returns true for the model at index <paramref name="indexOfItemThatWillBecomeVisible"/></summary>
		/// <seealso cref="OSA{TParams, TItemViewsHolder}.IsRecyclable(TItemViewsHolder, int, double)"/>
		protected override bool IsRecyclable(BaseVH potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
		{
			BaseModel model = Data[indexOfItemThatWillBecomeVisible];
			return potentiallyRecyclable.CanPresentModelType(model.CachedType);
		}

		/// <inheritdoc/>
		protected override void CancelUserAnimations()
		{
			// Correctly handling OSA's request to stop user's (our) animations
			_ExpandCollapseAnimation = null;

			base.CancelUserAnimations();
		}

		protected override void OnDestroy()
		{
			// Destroying all cached textures, since they're only used inside this ScrollView, which is now being destroyed.
			// Not doing this would cause memory leaks
			_ImagesPool.Clear();

			base.OnDestroy();
		}
		#endregion

		void OnExpandCollapseButtonClicked(ExpandableVH vh)
		{
			// Force finish previous animation
			if (_ExpandCollapseAnimation != null)
			{
				int oldItemIndex = _ExpandCollapseAnimation.itemIndex;
				var oldModel = Data[oldItemIndex] as ExpandableModel;
				_ExpandCollapseAnimation.ForceFinish();
				oldModel.ExpandedAmount = _ExpandCollapseAnimation.CurrentExpandedAmount;
				ResizeViewsHolderIfVisible(oldItemIndex, oldModel);
				ManageCollapseOfPreviouslyExpandedItem();
				_ExpandCollapseAnimation = null;
			}

			var model = Data[vh.ItemIndex] as ExpandableModel;
			var anim = new ExpandCollapseAnimationState(_Params.UseUnscaledTime);
			anim.initialExpandedAmount = model.ExpandedAmount;
			anim.duration = .2f;
			if (model.ExpandedAmount == 1f) // fully expanded
				anim.targetExpandedAmount = 0f;
			else
				anim.targetExpandedAmount = 1f;

			anim.itemIndex = vh.ItemIndex;

			_ExpandCollapseAnimation = anim;
		}

		float GetModelExpandedSize()
		{
			return _Params.expandableItemExpandFactor * _Params.DefaultItemSize;
		}

		float GetModelCurrentSize(ExpandableModel model)
		{
			float expandedSize = GetModelExpandedSize();

			return Mathf.Lerp(_Params.DefaultItemSize, expandedSize, model.ExpandedAmount);
		}

		void ResizeViewsHolderIfVisible(int itemIndex, ExpandableModel model)
		{
			float newSize = GetModelCurrentSize(model);

			// Set to true if positions aren't corrected; this happens if you don't position the pivot exactly at the stationary edge
			bool correctPositions = false;

			RequestChangeItemSizeAndUpdateLayout(itemIndex, newSize, _Params.freezeItemEndEdgeWhenResizing, true, correctPositions);
		}

		void ManageCollapseOfPreviouslyExpandedItem()
		{
			bool newItemIsExpanding = _ExpandCollapseAnimation.IsExpanding;
			if (_IndexOfCurrentlyExpandedItem != -1)
			{
				if (newItemIsExpanding)
				{
					// The previously expanded item grows inversely than the newly expanding one
					var modelOfExpandedItem = Data[_IndexOfCurrentlyExpandedItem] as ExpandableModel;
					modelOfExpandedItem.ExpandedAmount = _ExpandCollapseAnimation.CurrentExpandedAmountInverse;
					ResizeViewsHolderIfVisible(_IndexOfCurrentlyExpandedItem, modelOfExpandedItem);
				}
			}

			if (_ExpandCollapseAnimation.IsDone)
			{
				if (newItemIsExpanding)
					_IndexOfCurrentlyExpandedItem = _ExpandCollapseAnimation.itemIndex;
				else if (_ExpandCollapseAnimation.itemIndex == _IndexOfCurrentlyExpandedItem)
					// The currently expanded item was collapsed => invalidate _IndexOfCurrentlyExpandedItem 
					_IndexOfCurrentlyExpandedItem = -1;
			}
		}

		void AdvanceExpandCollapseAnimation()
		{
			int itemIndex = _ExpandCollapseAnimation.itemIndex;
			var model = Data[itemIndex] as ExpandableModel;
			model.ExpandedAmount = _ExpandCollapseAnimation.CurrentExpandedAmount;
			ResizeViewsHolderIfVisible(itemIndex, model);
			ManageCollapseOfPreviouslyExpandedItem();

			if (_ExpandCollapseAnimation != null && _ExpandCollapseAnimation.IsDone)
				_ExpandCollapseAnimation = null;
		}

		void ImageDestroyer(object urlKey, object texture)
		{
			var asUnityObject = texture as UnityEngine.Object;
			if (asUnityObject != null)
				Destroy(asUnityObject);
		}
	}


	/// <summary>Contains the 2 prefabs associated with the 2 views holders</summary>
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParams
	{
		public RectTransform bidirectionalPrefab, expandablePrefab;
		public float expandableItemExpandFactor = 2f;

		[NonSerialized]
		public bool freezeItemEndEdgeWhenResizing;


		public override void InitIfNeeded(IOSA iAdapter)
		{
			base.InitIfNeeded(iAdapter);

			AssertValidWidthHeight(bidirectionalPrefab);
			AssertValidWidthHeight(expandablePrefab);
		}
	}
}
