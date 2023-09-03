using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;

namespace Com.TheFallenGames.OSA.Demos.SimpleFilterable
{
	/// <summary>
	/// Example of using <see cref="FilterableDataHelper{T}"/> 
	/// </summary>
	public class SimpleFilterExample : OSA<FilterableMyParams, FilterableViewHoler>
	{
        /// <summary>Fired when the number of items changes or refreshes</summary>
        public UnityEngine.Events.UnityEvent OnItemsUpdated;

		public FilterableDataHelper<FilterableExampleItemModel> Data { get; private set; }


		private System.Collections.Generic.List<int> filterableItems = new System.Collections.Generic.List<int>();

		public void addFilterIndex(int index)
        {
			filterableItems.Add(index);

			rebuildPredicate();	
        }


		public void removeFilterIndex(int index)
        {
			filterableItems.Remove(index);

			rebuildPredicate();
		}

		private void rebuildPredicate()
		{


			Predicate<FilterableExampleItemModel> target;

			if (filterableItems.Count > 2)
			{
				target = (t) => false;
			}
			else if(filterableItems.Count == 2)
			{
				target = (t) => (filterableItems[0] == t.icon1Index || filterableItems[1] == t.icon1Index) &&
								(filterableItems[0] == t.icon2Index || filterableItems[1] == t.icon2Index);
            }
            else if(filterableItems.Count == 1)
            {
				target = (t) => t.icon1Index == filterableItems[0] || t.icon2Index == filterableItems[0];
            }
            else
            {
				target = (t) => true;
            }

			Data.FilteringCriteria = target;
		}


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new FilterableDataHelper<FilterableExampleItemModel>(this);

			base.Start();
		}

		/// <inheritdoc/>
		protected override FilterableViewHoler CreateViewsHolder(int itemIndex)
		{
			var instance = new FilterableViewHoler();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(FilterableViewHoler newOrRecycled)
		{
			// Initialize the views from the associated model
			FilterableExampleItemModel model = Data[newOrRecycled.ItemIndex];

			newOrRecycled.backgroundImage.color = model.color;
			newOrRecycled.UpdateTitleByItemIndex(model, Data.GetUnfilteredIdex(newOrRecycled.ItemIndex));
			newOrRecycled.icon1Image.texture = _Params.availableIcons[model.icon1Index];
			newOrRecycled.icon2Image.texture = _Params.availableIcons[model.icon2Index];
		}

		/// <summary>
		/// <para>This is overidden only so that the items' title will be updated to reflect its new index in case of Insert/Remove, because the index is not stored in the model</para>
		/// <para>If you don't store/care about the index of each item, you can omit this</para>
		/// <para>For more info, see <see cref="OSA{TParams, TItemViewsHolder}.OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder, int, bool, int)"/> </para>
		/// </summary>
		protected override void OnItemIndexChangedDueInsertOrRemove(FilterableViewHoler shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.UpdateTitleByItemIndex(Data[shiftedViewsHolder.ItemIndex], Data.GetUnfilteredIdex(shiftedViewsHolder.ItemIndex));
		}

		/// <inheritdoc/>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);

			if (OnItemsUpdated != null)
				OnItemsUpdated.Invoke();
		}
		#endregion
	}


	public class FilterableExampleItemModel
	{
		public string title;
		public int icon1Index, icon2Index;
		public readonly Color color = DemosUtil.GetRandomColor();
	}

	
	[Serializable] // serializable, so it can be shown in inspector
	public class FilterableMyParams : BaseParamsWithPrefab
	{
		public Texture2D[] availableIcons; // used to randomly generate models;
	}

	
	public class FilterableViewHoler : BaseItemViewsHolder
	{
		public Text titleText;
		public Image backgroundImage;
		public RawImage icon1Image, icon2Image;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("TitlePanel/TitleText", out titleText);
			root.GetComponentAtPath("Background", out backgroundImage);
			root.GetComponentAtPath("Icon1Image", out icon1Image);
			root.GetComponentAtPath("Icon2Image", out icon2Image);
		}

		public void UpdateTitleByItemIndex(FilterableExampleItemModel model, int unfilteredIndex) { titleText.text = "#" + ItemIndex + "("+ unfilteredIndex + ")" + "\n" + model.title; }
	}
}
