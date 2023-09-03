using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.DifferentPrefabPerOrientation.Models;
using Com.TheFallenGames.OSA.Demos.DifferentPrefabPerOrientation.ViewsHolders;
using Com.TheFallenGames.OSA.CustomParams;

namespace Com.TheFallenGames.OSA.Demos.DifferentPrefabPerOrientation
{
	/// <summary>
	/// <para>Example implementation showing how can you have 2 (or more) different prefabs for each screen orientation. </para>
	/// <para>See <see cref="OnScrollViewSizeChanged"/> for more info. </para>
	/// <para>Note: the following settings are incompatible with this use-case, so don't enable them: <see cref="BaseParams.Optimization.KeepItemsPoolOnEmptyList"/>, <see cref="BaseParams.Optimization.KeepItemsPoolOnLayoutRebuild"/></para>
	/// </summary>
	public class DifferentPrefabPerOrientationExample : OSA<MyParams, BaseVH>
	{
		public SimpleDataHelper<CommonModel> Data { get; private set; }

		Type _CurrentVHType;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<CommonModel>(this);

			ChooseCurrentPrefabAndVHType();

			base.Start();
		}

		/// <seealso cref="OSA{TParams, TItemViewsHolder}.CreateViewsHolder(int)"/>
		protected override BaseVH CreateViewsHolder(int itemIndex)
		{
			// "Activator" is a C# utility that lets you create an instance of a class given its type. Since we can't use "new Something()" here, we need to rely on it
			var vh = Activator.CreateInstance(_CurrentVHType) as BaseVH;
			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return vh;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(BaseVH newOrRecycled)
		{
			var model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}

		/// <inheritdoc/>
		protected override void OnScrollViewSizeChanged()
		{
			base.OnScrollViewSizeChanged();

			// Here, we decide which prefab to use based on the viewport's width-to-height ratio, but you're free to use your own app logic.
			// For ex., you can decide based on the Scrolling orientation (i.e. using Params.IsHorizontal), or the Screen.orientation property

			var prevPrefab = _Params.ItemPrefab;
			ChooseCurrentPrefabAndVHType();
			if (prevPrefab != _Params.ItemPrefab)
			{
				// The prefab has changed => Re-create all items

				var prevPos = GetNormalizedPosition();
				var prevVelocity = Velocity;

				// Let params update the DefaultItemSize property from the new prefab
				_Params.PrepareForInit(false);
				_Params.InitIfNeeded(this);

				// This clears the VHs, including any cached ones. Our models in Data aren't changed, of course
				ResetItems(0);

				// We don't use Data.ResetItems, but this.ResetItems() because we don't want the list of Models to be cleared, only the list of 
				// VHs. So they will be re-created from a new prefab by having CreateViewsHolder(int) called again.
				ResetItems(Data.Count);

				// Go full responsive by preserving the previous scroll position and velocity. This is not needed, but nice to have	
				SetNormalizedPosition(prevPos);
				Velocity = prevVelocity;
			}
		}
		#endregion

		void ChooseCurrentPrefabAndVHType()
		{
			var vpRect = Viewport.rect;
			bool isPortrait = vpRect.height > vpRect.width;
			if (isPortrait)
			{
				_Params.ItemPrefab = _Params.portraitPrefab;
				_CurrentVHType = typeof(PortraitVH);
			}
			else
			{
				_Params.ItemPrefab = _Params.landscapePrefab;
				_CurrentVHType = typeof(LandscapeVH);
			}
		}
	}


	/// <summary>
	/// Contains the prefabs for the 2 orientation types. Note that <see cref="BaseParamsWithPrefab._ItemPrefab"/> shouldn't be set in inspector because that's decided at runtime, based on the orientation
	/// </summary>
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public RectTransform portraitPrefab, landscapePrefab;


		public override void InitIfNeeded(IOSA iAdapter)
		{
			base.InitIfNeeded(iAdapter);

			AssertValidWidthHeight(portraitPrefab);
			AssertValidWidthHeight(landscapePrefab);
		}
	}
}
