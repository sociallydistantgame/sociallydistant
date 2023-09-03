using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Util.Animations;

namespace Com.TheFallenGames.OSA.Demos.ExpandToVariableSize
{
	/// <summary>A list of items similar to the <see cref="ContentSizeFitter.ContentSizeFitterExample"/>, but items can be expanded/collapsed</summary>
	public class ExpandItemToVariableSizeExample : OSA<MyParams, MyVH>
	{
		// Initialized outside
		public LazyDataHelper<MyModel> LazyData { get; set; }

		FitterVH _Fitter;
		ExpandCollapseAnimationState _ExpandCollapseAnimation;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			// Needed so that CancelUserAnimations() won't be called when sizes change (which happens during our animation itself)
			_Params.Animation.Cancel.UserAnimations.OnSizeChanges = false;

			// Prevent initialization. It'll be done from the outside
			//base.Start();
		}

		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			if (_ExpandCollapseAnimation != null)
				AdvanceExpandCollapseAnimation();
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();

			// Parent the fitter to the viewport, to distinguish it from other items.
			_Fitter = new FitterVH(_Params);
			_Fitter.Init(_Params.ItemPrefab, _Params.Viewport, -1);
		}

		/// <inheritdoc/>
		protected override MyVH CreateViewsHolder(int itemIndex)
		{
			var instance = new MyVH();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			instance.expandCollapseButton.onClick.AddListener(() => OnExpandCollapseButtonClicked(instance));

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyVH newOrRecycled)
		{
			// Update the views from the associated model
			MyModel model = LazyData.GetOrCreate(newOrRecycled.ItemIndex);

			newOrRecycled.UpdateFromModel(model);

			if (model.HasPendingExpandedSizeChange && model.ExpandedAmount > 0f)
			{
				// An expanded item was just made visible, and its ExpandedSize is unknown or not updated => Schedule a twin pass to update it now
				ScheduleComputeVisibilityTwinPass();
			}
		}

		/// <inheritdoc/>
		protected override float UpdateItemSizeOnTwinPass(MyVH viewsHolder)
		{
			MyModel model = LazyData.GetOrCreate(viewsHolder.ItemIndex);
			if (model.HasPendingExpandedSizeChange)
			{
				// This is one of the items for which ScheduleComputeVisibilityTwinPass() was called => update its size now
				float newSize = UpdateAndGetModelCurrentSize(model);
				viewsHolder.root.SetSizeFromParentEdgeWithCurrentAnchors(_Params.Content, RectTransform.Edge.Top, newSize);
			}

			return base.UpdateItemSizeOnTwinPass(viewsHolder);
		}

		/// <inheritdoc/>
		protected override void RebuildLayoutDueToScrollViewSizeChange()
		{
			// Invalidate the last expanded sizes so that they'll be re-calculated
			SetAllModelsHavePendingSizeChange();

			base.RebuildLayoutDueToScrollViewSizeChange();
		}

		/// <summary>
		/// When the user resets the count or refreshes, the OSA's cached sizes are cleared so we can recalculate them. 
		/// This is provided here for new users that just want to call Refresh() and have everything updated instead of telling OSA exactly what has updated.
		/// But, in most cases you shouldn't need to ResetItems() or Refresh() because of performace reasons:
		/// - If you add/remove items, InsertItems()/RemoveItems() is preferred if you know exactly which items will be added/removed;
		/// - When just one item has changed externally and you need to force-update its size, you'd call ForceRebuildViewsHolderAndUpdateSize() on it;
		/// - When the layout is rebuilt (when you change the size of the viewport or call ScheduleForceRebuildLayout()), that's already handled
		/// So the only case when you'll need to call Refresh() (and override ChangeItemsCount()) is if your models can be changed externally and you'll only know that they've changed, but won't know which ones exactly.
		/// </summary>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			if (changeMode == ItemCountChangeMode.RESET)
				SetAllModelsHavePendingSizeChange();

			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}

		/// <inheritdoc/>
		protected override void CancelUserAnimations()
		{
			// Correctly handling OSA's request to stop user's (our) animations
			_ExpandCollapseAnimation = null;

			base.CancelUserAnimations();
		}
		#endregion

		void OnExpandCollapseButtonClicked(MyVH vh)
		{
			// Force finish previous animation
			if (_ExpandCollapseAnimation != null)
			{
				int oldItemIndex = _ExpandCollapseAnimation.itemIndex;
				var oldModel = LazyData.GetOrCreate(oldItemIndex);
				_ExpandCollapseAnimation.ForceFinish();
				oldModel.ExpandedAmount = _ExpandCollapseAnimation.CurrentExpandedAmount;
				UpdateModelAndResizeViewsHolderIfVisible(oldItemIndex, oldModel);
				_ExpandCollapseAnimation = null;
			}

			var model = LazyData.GetOrCreate(vh.ItemIndex);

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

		float UpdateAndGetModelExpandedSize(MyModel model)
		{
			float expandedSize;
			if (model.HasPendingExpandedSizeChange)
			{
				_Fitter.UpdateFromModel(model);
				expandedSize = ForceRebuildViewsHolder(_Fitter);
				model.ExpandedSize = expandedSize;
				model.HasPendingExpandedSizeChange = false;
			}
			else
				expandedSize = model.ExpandedSize;

			return expandedSize;
		}

		float UpdateAndGetModelCurrentSize(MyModel model)
		{
			float expandedSize = UpdateAndGetModelExpandedSize(model);

			return Mathf.Lerp(_Params.DefaultItemSize, expandedSize, model.ExpandedAmount);
		}

		void UpdateModelAndResizeViewsHolderIfVisible(int itemIndex, MyModel model)
		{
			float newSize = UpdateAndGetModelCurrentSize(model);

			// Set to true if positions aren't corrected; this happens if you don't position the pivot exactly at the stationary edge
			bool correctPositions = false;

			RequestChangeItemSizeAndUpdateLayout(itemIndex, newSize, false, true, correctPositions);

			var vh = GetItemViewsHolderIfVisible(itemIndex);
			if (vh != null)
			{
				vh.UpdateExpandCollapseArrowRotation(model);

				// If you encounter problems with RectMask2D not working properly, uncomment this. The VH class also needs to be changed to retrieve the RectMask2D component

				//// Fixing Unity bug: https://issuetracker.unity3d.com/issues/rectmask2d-doesnt-update-when-the-parent-size-is-changed
				//// Changing the transform's scale and restoring it back. This trigggers the update of the RectMask2D. 
				//// Tried RectMask2D.PerformClipping(), tried setting m_ForceClipping and other params through reflection with no success.
				//// This workaround remains the only one that works.
				//// This is not needed in case galleryEffect is bigger than 0, since that already changes the items' scale periodically, but we included it to cover all cases
				//var localScale = vh.rectMask2DRectTransform.localScale;
				//vh.rectMask2DRectTransform.localScale = localScale * .99f;
				//vh.rectMask2DRectTransform.localScale = localScale;
			}
		}

		void AdvanceExpandCollapseAnimation()
		{
			int itemIndex = _ExpandCollapseAnimation.itemIndex;
			var model = LazyData.GetOrCreate(itemIndex);
			model.ExpandedAmount = _ExpandCollapseAnimation.CurrentExpandedAmount;
			UpdateModelAndResizeViewsHolderIfVisible(itemIndex, model);

			if (_ExpandCollapseAnimation != null && _ExpandCollapseAnimation.IsDone)
				_ExpandCollapseAnimation = null;
		}
		
		void SetAllModelsHavePendingSizeChange()
		{
			foreach (var model in LazyData.GetEnumerableForExistingItems())
				model.HasPendingExpandedSizeChange = true;
		}
	}

	/// <summary>The data associated with an item</summary>
	public class MyModel
	{
		/// <summary><see cref="HasPendingExpandedSizeChange"/> is set to true each time this property changes</summary>
		public string Title
		{
			get { return _Title; }
			set
			{
				if (_Title != value)
				{
					_Title = value;
					HasPendingExpandedSizeChange = true;
				}
			}
		}

		/// <summary>This will be true when the item size may have changed and the ContentSizeFitter component needs to be updated</summary>
		public bool HasPendingExpandedSizeChange
		{
			get { return _HasPendingSizeChange; }
			set
			{
				_HasPendingSizeChange = value;

				if (_HasPendingSizeChange)
					ExpandedSize = -1f;
			}
		}

		public float ExpandedSize { get; set; }
		public float ExpandedAmount { get; set; }

		string _Title;
		bool _HasPendingSizeChange;


		public MyModel()
		{
			// By default, the model's expanded size is unknown, so mark it for size re-calculation next time it's requested
			HasPendingExpandedSizeChange = true;
		}
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
	}


	/// <summary>The Views holder, as explained in other examples. It displays your data and it's constantly recycled</summary>
	public class MyVH : BaseItemViewsHolder
	{
		public RectTransform barImage;
		public Text titleText;
		public Button expandCollapseButton;
		public RectTransform expandCollapseArrow;


		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("BarImage", out barImage);
			root.GetComponentAtPath("TitlePanel/TitleText", out titleText);
			root.GetComponentAtPath("ExpandCollapseButton", out expandCollapseButton);
			expandCollapseArrow = expandCollapseButton.transform.GetChild(0).transform as RectTransform;
		}

		/// <summary>Utility getting rid of the need of manually writing assignments</summary>
		public void UpdateFromModel(MyModel model)
		{
			UpdateExpandCollapseArrowRotation(model);
			UpdateTitleByItemIndex(model);
		}

		public void UpdateTitleByItemIndex(MyModel model)
		{
			string title = "[#" + ItemIndex + "] " + model.Title;
			if (titleText.text != title)
			{
				titleText.text = title;
				UpdateBarVisual();
			}
		}

		/// <summary>Just an aesthetic thing. Tha bar is thicker the more text there is</summary>
		void UpdateBarVisual()
		{
			var scale = barImage.localScale;
			scale.x = 1f + 10f * ((float)titleText.text.Length / 500);
			scale.x = Mathf.Min(10f, scale.x);
			barImage.localScale = scale;
		}

		public void UpdateExpandCollapseArrowRotation(MyModel model)
		{
			var rot = expandCollapseArrow.localEulerAngles;
			rot.z = Mathf.Lerp(270f, 90f, model.ExpandedAmount);
			expandCollapseArrow.localEulerAngles = rot;
		}
	}


	/// <summary>
	/// This is the same as our views holder, but it also needs a content size fitter, and it's used exclusively by us to calculate sizes of the regular views holders. 
	/// It's not passet to the adapter.
	/// </summary>
	public class FitterVH : MyVH
	{
		UnityEngine.UI.ContentSizeFitter CSF { get; set; }
		MyParams _Params;


		public FitterVH(MyParams myParams)
		{
			_Params = myParams;
		}


		public override void CollectViews()
		{
			base.CollectViews();

			root.name = "SharedFitter";

			CSF = root.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
			CSF.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

			// Scale with the viewport's width. Height unscaled
			root.anchorMin = Vector2.zero;
			root.anchorMax = new Vector2(1f, 0f);

			// Middle-Center pivot to grow downwards on resizing
			root.pivot = new Vector2(.5f, 1f);

			// Place it way below the viewport, so it won't be visible. The ScrollView's mask should not allow it to be visible anyway.
			// Height doesn't matter, because it'll constantly change
			float height = 1234f; 
			root.SetInsetAndSizeFromParentBottomEdgeWithCurrentAnchors(-30000f, height);

			// Correctly set its left and right padding according to the parameters, just like the regular items will be placed.
			// Since this VH is parented to the Viewport, you may wonder why we can use the paddings used for the Content: 
			// In an OSA, the Content and the Viewport always have the same size and position, 
			// so their children will be identically placed if the same insets and sizes are used
			var padding = _Params.ContentPadding;
			root.SetInsetAndSizeFromParentLeftEdgeWithCurrentAnchors(padding.left, _Params.Content.rect.width - padding.right);
		}
	}
}
