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

namespace Com.TheFallenGames.OSA.Demos.Simple
{
	/// <summary>
	/// Old name "SimpleTutorial". Class (initially) implemented during this YouTube tutorial: https://youtu.be/aoqq_j-aV8I (which is now old, but still useful to learn the basic concepts). 
	/// It demonstrates a simple use case with items that expand/collapse on click.
	/// It uses the <see cref="SimpleDataHelper{T}"/> just so we won't need to spend time on lazily-initializing the models
	/// </summary>
	public class SimpleExample : OSA<MyParams, MyItemViewsHolder>
	{
        /// <summary>Fired when the number of items changes or refreshes</summary>
        public UnityEngine.Events.UnityEvent OnItemsUpdated;

		public SimpleDataHelper<ExampleItemModel> Data { get; private set; }


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<ExampleItemModel>(this);

			base.Start();
		}

		/// <inheritdoc/>
		protected override MyItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyItemViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyItemViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			ExampleItemModel model = Data[newOrRecycled.ItemIndex];

			newOrRecycled.backgroundImage.color = model.color;
			newOrRecycled.UpdateTitleByItemIndex(model);
			newOrRecycled.icon1Image.texture = _Params.availableIcons[model.icon1Index];
			newOrRecycled.icon2Image.texture = _Params.availableIcons[model.icon2Index];
		}

		/// <summary>
		/// <para>This is overidden only so that the items' title will be updated to reflect its new index in case of Insert/Remove, because the index is not stored in the model</para>
		/// <para>If you don't store/care about the index of each item, you can omit this</para>
		/// <para>For more info, see <see cref="OSA{TParams, TItemViewsHolder}.OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder, int, bool, int)"/> </para>
		/// </summary>
		protected override void OnItemIndexChangedDueInsertOrRemove(MyItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
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


	public class ExampleItemModel
	{
		public string title;
		public int icon1Index, icon2Index;
		public readonly Color color = DemosUtil.GetRandomColor();
	}

	
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public Texture2D[] availableIcons; // used to randomly generate models;
	}

	
	public class MyItemViewsHolder : BaseItemViewsHolder
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

		public void UpdateTitleByItemIndex(ExampleItemModel model) { titleText.text = model.title + " #" + ItemIndex; }
	}
}