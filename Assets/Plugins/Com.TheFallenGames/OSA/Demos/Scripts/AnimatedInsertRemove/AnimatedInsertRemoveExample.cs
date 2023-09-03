using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Util.Animations;

namespace Com.TheFallenGames.OSA.Demos.AnimatedInsertRemove
{
	/// <summary>
	/// <para>Simple list demonstrating adding and removing items with an animation. Items "pop up" when are inserted and shrink before being removed.</para>
	/// <para>It's assumed that adding/inserting items is not done throgh the <see cref="OSA{TParams, TItemViewsHolder}.InsertItems(int, int, bool, bool)"/> or <see cref="OSA{TParams, TItemViewsHolder}.RemoveItems(int, int, bool, bool)"/>,
	/// but only through <see cref="AnimatedInsert(int, MyModel)"/> and <see cref="AnimatedRemove(int)"/>, respectively</para>
	/// </summary>
	public class AnimatedInsertRemoveExample : OSA<MyParams, MyVH>
	{
		// Initialized outside
		public LazyDataHelper<MyModel> LazyData { get; set; }

		const float NON_EXPANDED_SIZE = .1f;

		InsertDeleteAnimationState _InsertDeleteAnimation;
		bool _AlternatingEndEdgeStationary;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			var cancel = _Params.Animation.Cancel;
			// Needed so that CancelUserAnimations() won't be called when sizes change (which happens during our animation itself)
			cancel.UserAnimations.OnCountChanges = false;
			// Needed so that CancelUserAnimations() won't be called on count changes - we're handling these manually by overriding ChangeItemsCount 
			cancel.UserAnimations.OnSizeChanges = false;

			// Prevent initialization. It'll be done from the outside
			//base.Start();
		}

		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			if (_InsertDeleteAnimation != null)
				AdvanceExpandCollapseAnimation(GetShouldKeepEndEdgeStationary());
		}

		/// <inheritdoc/>
		protected override MyVH CreateViewsHolder(int itemIndex)
		{
			var instance = new MyVH();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyVH newOrRecycled)
		{
			// Update the views from the associated model
			MyModel model = LazyData.GetOrCreate(newOrRecycled.ItemIndex);
			newOrRecycled.UpdateFromModel(model);
		}

		/// <inheritdoc/>
		protected override void OnItemIndexChangedDueInsertOrRemove(MyVH shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			// Update the title only, since it's the only view that's dependent on the index and not on the actual data
			shiftedViewsHolder.UpdateTitleByItemIndex();
		}
		
		/// <inheritdoc/>
		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);

			// CollectItemsSizes is called whenever the items count changes, but before the view is actually updated.
			// The reason we need it here is that when inserting an item, it's initial size will be _Params.DefaultItemSize 
			// (which is taken from the prefab's actual size), but we want it to be NON_EXPANDED_SIZE (which is close to zero), 
			// so we can expand it after. 
			// It's not something very important, but it looks a bit odd without it

			if (changeMode != ItemCountChangeMode.INSERT)
				return;

			if (count > 1)
				// Not an animated insertion, since the count would've been 1 in that case
				return;

			if (_InsertDeleteAnimation == null)
				// No animation running => the item will directly appear expanded (with _Params.DefaultItemSize)
				return;

			if (_InsertDeleteAnimation.itemIndex == indexIfInsertingOrRemoving)
			{
				itemsDesc.BeginChangingItemsSizes(indexIfInsertingOrRemoving);
				itemsDesc[indexIfInsertingOrRemoving] = NON_EXPANDED_SIZE;
				itemsDesc.EndChangingItemsSizes();
			}
		}

		/// <inheritdoc/>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			// No animation should be preserved between count changes. 
			// In case of insertion and removals, we don't touch it, since we expect those to be handled by the AnimatedInsert and AnimatedRemove, respectively
			if (changeMode == ItemCountChangeMode.RESET)
				_InsertDeleteAnimation = null;

			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}

		/// <inheritdoc/>
		protected override void CancelUserAnimations()
		{
			// Correctly handling OSA's request to stop user's (our) animations
			_InsertDeleteAnimation = null;

			base.CancelUserAnimations();
		}
		#endregion

		/// <summary>Inserting now, then animating</summary>
		public void AnimatedInsert(int index, MyModel model)
		{
			// Force finish previous animation
			if (_InsertDeleteAnimation != null)
			{
				ForceFinishCurrentAnimation();

				if (index > LazyData.Count)
					// The previous animation was a removal and this index is not valid anymore
					return;
			}

			_InsertDeleteAnimation = new InsertDeleteAnimationState(_Params.UseUnscaledTime, index, 0f, 1f);
			LazyData.InsertOneManuallyCreated(index, model, false);
		}

		/// <summary>Animating removal, then remove</summary>
		public void AnimatedRemove(int index)
		{
			// Force finish previous animation
			if (_InsertDeleteAnimation != null)
			{
				ForceFinishCurrentAnimation();

				if (index >= LazyData.Count)
					// The previous animation was a removal and this index is not valid anymore
					return;
			}

			var model = LazyData.GetOrCreate(index);
			_InsertDeleteAnimation = new InsertDeleteAnimationState(_Params.UseUnscaledTime, index, model.expandedAmount, 0f);
		}

		void ForceFinishCurrentAnimation()
		{
			_InsertDeleteAnimation.ForceFinish();
			AdvanceExpandCollapseAnimation(false);
		}

		float GetModelCurrentSize(MyModel model)
		{
			float nonExpandedSize = NON_EXPANDED_SIZE;
			float expandedSize = _Params.DefaultItemSize;

			return Mathf.Lerp(nonExpandedSize, expandedSize, model.expandedAmount);
		}

		void ResizeViewsHolderIfVisible(int itemIndex, MyModel model, bool endEdgeStationary)
		{
			float newSize = GetModelCurrentSize(model);

			// Set to true if positions aren't corrected; this happens if you don't position the pivot exactly at the stationary edge
			bool correctPositions = false;

			RequestChangeItemSizeAndUpdateLayout(itemIndex, newSize, endEdgeStationary, true, correctPositions);
		}

		void AdvanceExpandCollapseAnimation(bool itemEndEdgeStationary)
		{
			int itemIndex = _InsertDeleteAnimation.itemIndex;
			var model = LazyData.GetOrCreate(itemIndex);
			model.expandedAmount = _InsertDeleteAnimation.CurrentExpandedAmount;

			ResizeViewsHolderIfVisible(itemIndex, model, itemEndEdgeStationary);

			if (_InsertDeleteAnimation != null && _InsertDeleteAnimation.IsDone)
				OnCurrentInsertDeleteAnimationFinished();
		}

		void OnCurrentInsertDeleteAnimationFinished()
		{
			int itemIndex = _InsertDeleteAnimation.itemIndex;
			if (!_InsertDeleteAnimation.IsInsert)
			{
				// The animation was a remove animation => The item needs to be removed at the end of it
				LazyData.RemoveItems(itemIndex, 1, false);
			}

			_InsertDeleteAnimation = null;
		}

		/// <summary>
		/// If the item needs to expand both upwards and downwards, do it alternatively between the frames.
		/// Otherwise, fix its top edge.
		/// </summary>
		bool GetShouldKeepEndEdgeStationary()
		{
			if (_Params.itemAnimationPivotMiddle)
			{
				_AlternatingEndEdgeStationary = !_AlternatingEndEdgeStationary;
				return _AlternatingEndEdgeStationary;
			}
			return false;
		}
	}

	/// <summary>The data associated with an item</summary>
	public class MyModel
	{
		public Color color;
		public float expandedAmount = 1f;
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public bool itemAnimationPivotMiddle;
	}


	/// <summary>The Views holder. It displays your data and it's constantly recycled</summary>
	public class MyVH : BaseItemViewsHolder
	{
		public Text titleText;
		public Image background;


		public override void CollectViews()
		{
			base.CollectViews();

			background = root.GetComponent<Image>();
			root.GetComponentAtPath("TitlePanel/TitleText", out titleText);
		}

		/// <summary>Utility getting rid of the need of manually writing assignments</summary>
		public void UpdateFromModel(MyModel model)
		{
			background.color = model.color;
			UpdateTitleByItemIndex();
		}

		public void UpdateTitleByItemIndex()
		{
			string title = "[#" + ItemIndex + "] Item";
			if (titleText.text != title)
				titleText.text = title;
		}
	}


	/// <summary>Class to also store whether the animation a delete or insert</summary>
	class InsertDeleteAnimationState : ExpandCollapseAnimationState
	{
		public bool IsInsert { get { return targetExpandedAmount == 1f; } }

		const float ANIMATION_DURATION = .5f;


		public InsertDeleteAnimationState(bool useUnscaledTime, int itemIndex, float initialExpandedAmount, float targetExpandedAmount)
			:base(useUnscaledTime)
		{
			this.itemIndex = itemIndex;
			this.initialExpandedAmount = initialExpandedAmount;
			this.targetExpandedAmount = targetExpandedAmount;
			duration = ANIMATION_DURATION;
		}
	}
}
