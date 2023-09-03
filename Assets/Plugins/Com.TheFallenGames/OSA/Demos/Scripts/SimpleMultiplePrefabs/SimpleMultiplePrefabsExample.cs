using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.ViewsHolders;
using Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.Models;

namespace Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs
{
	/// <summary>
	/// <para>Example implementation (simpler than <see cref="MultiplePrefabs.MultiplePrefabsExample"/>) demonstrating the use of 3 different views holders, presenting 3 different models: </para>
	/// <para>- Green model contains text content </para>
	/// <para>- Orange model contains a float number presented by a slider</para>
	/// <para>- Ad model shows a clickable image of variable size</para>
	/// <para>The only constrain is for the models to have a common ancestor class and for the views holders to also have a common ancestor class</para>
	/// <para>At the core, everything's the same as in other example implementations, so if something's not clear, check them (SimpleExample is a good start)</para>
	/// </summary>
	public class SimpleMultiplePrefabsExample : OSA<MyParams, SimpleBaseVH>
	{
		public SimpleDataHelper<SimpleBaseModel> Data { get; private set; }


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<SimpleBaseModel>(this);

			base.Start();
		}

		/// <summary>Creates a viewsholder, depending on the type of the model at index <paramref name="itemIndex"/></summary>
		/// <seealso cref="OSA{TParams, TItemViewsHolder}.CreateViewsHolder(int)"/>
		protected override SimpleBaseVH CreateViewsHolder(int itemIndex)
		{
			var modelType = Data[itemIndex].CachedType;
			if (modelType == typeof(GreenModel))
			{
				var vh = new GreenVH();
				vh.Init(_Params.greenPrefab, _Params.Content, itemIndex);

				return vh;
			}

			if (modelType == typeof(OrangeModel))
			{
				var vh = new OrangeVH();
				vh.Init(_Params.orangePrefab, _Params.Content, itemIndex);

				return vh;
			}

			if (modelType == typeof(AdModel))
			{
				var vh = new AdVH();
				vh.Init(_Params.adPrefab, _Params.Content, itemIndex);
				vh.Clicked += OnAdClicked;
				return vh;
			}

			// If you want to avoid ifs, you could use a dictionary with model type as key and a Func<int, SimpleBaseHV> as value
			// which would point to a separate method for each model type. Then here simply return _Map[modelType](itemIndex)

			throw new InvalidOperationException("Unrecognized model type: " + modelType.Name);
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(SimpleBaseVH newOrRecycled)
		{
			// Initialize/update the views from the associated model.
			// If the VH needs additional information for updating the view (for example, something from the _Params),
			// you can either add new parameters to the SimpleBaseVH.UpdateViews() method or pass those 
			// parameters at the moment the VH is created so it'll always have access to them.
			SimpleBaseModel model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);

			// This allows items to have different sizes by calling UpdateItemSizeOnTwinPass() for each of them after the current ComputeVisibility pass.
			// We're always calling it just for simplicity, but usually you'd only call it if you detect the item's size has changed (in our case
			// this can only happen with the Ad items, whose sizes depend on their image)
			ScheduleComputeVisibilityTwinPass();
		}

		/// <inheritdoc/>
		protected override float UpdateItemSizeOnTwinPass(SimpleBaseVH viewsHolder)
		{
			viewsHolder.UpdateSize();
			return viewsHolder.root.rect.height;
		}

		/// <summary>
		/// Overriding the base implementation, which always returns true. 
		/// In this case, a views holder is recyclable only if its <see cref="SimpleBaseVH.CanPresentModelType(Type)"/> returns 
		/// true for the model at index <paramref name="indexOfItemThatWillBecomeVisible"/></summary>
		/// <seealso cref="OSA{TParams, TItemViewsHolder}.IsRecyclable(TItemViewsHolder, int, double)"/>
		protected override bool IsRecyclable(SimpleBaseVH potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
		{
			SimpleBaseModel model = Data[indexOfItemThatWillBecomeVisible];
			return potentiallyRecyclable.CanPresentModelType(model.CachedType);
		}
		#endregion

		void OnAdClicked(AdVH vh)
		{
			var adModel = Data[vh.ItemIndex] as AdModel;
			Debug.Log("Ad clicked: item #" + vh.ItemIndex + ", ID = " + adModel.adID);
		}
	}


	/// <summary>
	/// Contains the prefabs associated with the 3 views holder types
	/// </summary>
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParams
	{
		public RectTransform orangePrefab, greenPrefab, adPrefab;


		public override void InitIfNeeded(IOSA iAdapter)
		{
			base.InitIfNeeded(iAdapter);

			AssertValidWidthHeight(orangePrefab);
			AssertValidWidthHeight(greenPrefab);
			AssertValidWidthHeight(adPrefab);
		}
	}
}
