using System;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;

namespace Com.TheFallenGames.OSA.Demos.LoopingSpinners
{
	/// <summary>
	/// Very basic example with a spinner that loops its items, similarly to a time picker in an alarm app.
	/// Minimal implementation of the adapter that initializes and updates the viewsholders. The size of each item is fixed in this case and it's the same as the prefab's
	/// </summary>
	public class LoopingSpinnerExample : OSA<MyParams, MyItemViewsHolder>
	{
		#region OSA implementation
		/// <inheritdoc/>
		protected override void Update()
        {
			base.Update();

			if (!IsInitialized)
				return;

			_Params.currentSelectedIndicatorText.text = "Selected: ";
            if (VisibleItemsCount == 0)
                return;

			// Update 27.12.2020: more accurate detection of middle vh, based on snapper
			int middleVHItemIndex = -1;
			if (_Params.Snapper)
			{
				float _;
				var middleVH = _Params.Snapper.GetMiddleVH(out _) as MyItemViewsHolder;
				if (middleVH != null)
				{
					middleVHItemIndex = middleVH.ItemIndex;
					_Params.currentSelectedIndicatorText.text += _Params.GetItemValueAtIndex(middleVH.ItemIndex);
					middleVH.background.CrossFadeColor(_Params.selectedColor, .1f, false, false);
				}
			}

            for (int i = 0; i < VisibleItemsCount; ++i)
            {
				var vh = GetItemViewsHolder(i);
				if (vh.ItemIndex != middleVHItemIndex)
					vh.background.CrossFadeColor(_Params.nonSelectedColor, .1f, false, false);
            }
		}

		/// <inheritdoc/>
		protected override MyItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyItemViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyItemViewsHolder newOrRecycled) { newOrRecycled.titleText.text = _Params.GetItemValueAtIndex(newOrRecycled.ItemIndex) + ""; }
		#endregion

		/// <summary>If the viewport's size changes, looping capability may also change</summary>
		protected override void PostRebuildLayoutDueToScrollViewSizeChange()
		{
			base.PostRebuildLayoutDueToScrollViewSizeChange();
			ManageLoopingOnItemCountOrLayoutChanges(GetItemsCount());
		}

		/// <summary>If the items count changes, looping capability may also change</summary>
		public void ChangeItemsCountWithChecks(int newCount)
		{
			ResetItems(newCount);
			ManageLoopingOnItemCountOrLayoutChanges(newCount);
		}

		void ManageLoopingOnItemCountOrLayoutChanges(int newItemsCount)
		{
			bool wasLooping = _Params.effects.LoopItems;
			bool canLoop = CanLoop(newItemsCount);
			if (wasLooping == canLoop)
				return;

			_Params.effects.LoopItems = canLoop;
			double padStartEnd;
			if (canLoop)
				padStartEnd = _Params.ContentSpacing;
			else
				// Explanation: Having half the viewport as padding allows you to move the item at extremity 
				// so that its EDGE will be exactly at the viewport's center. After that, we subtract half of the 
				// item's size from the padding, so that the item's CENTER will be in the viewport's center.
				padStartEnd = GetViewportSize() / 2 - _Params.DefaultItemSize / 2;

			_Params.ContentPadding.top = _Params.ContentPadding.bottom = (int)padStartEnd;
			ScheduleForceRebuildLayout();
		}

		bool CanLoop(int newItemsCount)
		{
			float itemSizeAndSpacing = _Params.DefaultItemSize + _Params.ContentSpacing;
			int minCount = (int)(GetViewportSize() / itemSizeAndSpacing + 2);
			return newItemsCount >= minCount;
		}
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public int startItemNumber = 0;
		public int increment = 1;
		public Color selectedColor, nonSelectedColor;
		public Text currentSelectedIndicatorText;

		/// <summary>The value of each item is calculated dynamically using its <paramref name="index"/>, <see cref="startItemNumber"/> and the <see cref="increment"/></summary>
		/// <returns>The item's value (the displayed number)</returns>
		public int GetItemValueAtIndex(int index) { return startItemNumber + increment * index; }
	}


	public class MyItemViewsHolder : BaseItemViewsHolder
	{
		public Image background;
		public Text titleText;

		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			background = root.GetComponent<Image>();
			titleText = root.GetComponentInChildren<Text>();
		}
	}
}