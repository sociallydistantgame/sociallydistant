using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;

namespace Com.TheFallenGames.OSA.Demos.SimplePullToRefresh
{
	/// <summary>
	/// This is very similar to <see cref="Simple.SimpleExample"/>, but it's a bit simpler and allows items to be 
	/// refreshed or (new ones) inserted when the user pulls the ScrollView's start or end edge
	/// </summary>
	public class SimplePullToRefreshExample : OSA<MyParams, MyViewsHolder>
	{
        /// <summary>Fired when the number of items changes or the items were refreshed</summary>
        public UnityEngine.Events.UnityEvent OnItemsUpdated;

		public SimpleDataHelper<MyModel> Data { get; private set; }


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<MyModel>(this);

			base.Start();
		}

		/// <inheritdoc/>
		protected override MyViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			MyModel model = Data[newOrRecycled.ItemIndex];

			newOrRecycled.backgroundImage.color = model.color;
			newOrRecycled.UpdateTitleByItemIndex(model);
		}

		/// <summary>
		/// <para>This is overidden only so that the items' title will be updated to reflect its new index in case of Insert/Remove, because the index is not stored in the model</para>
		/// <para>If you don't store/care about the index of each item, you can omit this</para>
		/// <para>For more info, see <see cref="OSA{TParams, TItemViewsHolder}.OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder, int, bool, int)"/> </para>
		/// </summary>
		protected override void OnItemIndexChangedDueInsertOrRemove(MyViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.UpdateTitleByItemIndex(Data[shiftedViewsHolder.ItemIndex]);
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


	public class MyModel
	{
		public string title;
		public readonly Color color = DemosUtil.GetRandomColor();
	}

	
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
	}

	
	public class MyViewsHolder : BaseItemViewsHolder
	{
		public Text titleText;
		public Image backgroundImage;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("TitlePanel/TitleText", out titleText);
			root.GetComponentAtPath("Background", out backgroundImage);
		}

		public void UpdateTitleByItemIndex(MyModel model) { titleText.text = model.title + " #" + ItemIndex; }
	}
}