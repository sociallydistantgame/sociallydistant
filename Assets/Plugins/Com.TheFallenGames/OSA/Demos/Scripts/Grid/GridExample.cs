using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Util.IO;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Util.IO.Pools;

namespace Com.TheFallenGames.OSA.Demos.Grid
{
    /// <summary>
    /// Implementation demonstrating the usage of a <see cref="GridAdapter{TParams, TCellVH}"/> for a simple gallery of remote images downloaded with a <see cref="SimpleImageDownloader"/>.
    /// </summary>
    public class GridExample : GridAdapter<GridParams, MyCellViewsHolder>//, ILazyListSimpleDataManager<BasicModel>
	{
		public LazyDataHelper<BasicModel> LazyData { get; private set; }

		public bool freezeContentEndEdgeOnCountChange;

		IPool _ImagesPool;


		#region GridAdapter implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			LazyData = new LazyDataHelper<BasicModel>(this, CreateRandomModel);

			_ImagesPool = new FIFOCachingPool(50, ImageDestroyer);

			base.Start();
		}

		/// <param name="contentPanelEndEdgeStationary">ignored because we override this via <see cref="freezeContentEndEdgeOnCountChange"/></param>
		/// <seealso cref="GridAdapter{TParams, TCellVH}.Refresh(bool, bool)"/>
		public override void Refresh(bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			_CellsCount = LazyData.Count;
			base.Refresh(freezeContentEndEdgeOnCountChange, keepVelocity);
		}

		///// </inheritdoc>
		//protected override void OnCellViewsHolderCreated(MyCellViewsHolder cellVH, CellGroupViewsHolder<MyCellViewsHolder> cellGroup)
		//{
		//	base.OnCellViewsHolderCreated(cellVH, cellGroup);

		//	cellVH.flexibleHeightToggle.onValueChanged.AddListener(_ => OnFlexibleHeightToggledOnCell(cellVH));
		//}

		/// </inheritdoc>
		protected override void OnCellViewsHolderCreated(MyCellViewsHolder cellVH, CellGroupViewsHolder<MyCellViewsHolder> cellGroup)
		{
			base.OnCellViewsHolderCreated(cellVH, cellGroup);

			cellVH.iconRemoteImageBehaviour.InitializeWithPool(_ImagesPool);
		}

		/// <summary> Called when a cell becomes visible </summary>
		/// <param name="viewsHolder"> use viewsHolder.ItemIndexto find your corresponding model and feed data into its views</param>
		protected override void UpdateCellViewsHolder(MyCellViewsHolder viewsHolder)
		{
			var model = LazyData.GetOrCreate(viewsHolder.ItemIndex);

			viewsHolder.title.text = "Loading";
			viewsHolder.overlayImage.color = Color.white;
			int itemIndexAtRequest = viewsHolder.ItemIndex;
			var imageURLAtRequest = model.imageURL;
			viewsHolder.iconRemoteImageBehaviour.Load(imageURLAtRequest, true, (fromCache, success) => {
				if (!IsRequestStillValid(viewsHolder.ItemIndex, itemIndexAtRequest, imageURLAtRequest))
					return;

				viewsHolder.overlayImage.CrossFadeAlpha(0f, .5f, false);
				viewsHolder.title.text = model.title;
			});
			//viewsHolder.flexibleHeightToggle.isOn = model.flexibleHeight;
			//viewsHolder.UpdateFlexibleHeightFromToggleState();
		}

		protected override void OnDestroy()
		{
			// Destroying all cached textures, since they're only used inside this ScrollView, which is now being destroyed.
			// Not doing this would cause memory leaks
			_ImagesPool.Clear();

			base.OnDestroy();
		}
		#endregion

		//void OnFlexibleHeightToggledOnCell(MyCellViewsHolder cellVH)
		//{
		//	// Update the model this cell is representing
		//	var model = _Data[cellVH.ItemIndex];
		//	model.flexibleHeight = cellVH.flexibleHeightToggle.isOn;
		//	cellVH.UpdateFlexibleHeightFromToggleState();
		//}

		bool IsRequestStillValid(int itemIndex, int itemIdexAtRequest, string imageURLAtRequest)
		{
			return
				_CellsCount > itemIndex // be sure the index still points to a valid model
				&& itemIdexAtRequest == itemIndex // be sure the view's associated model index is the same (i.e. the viewsHolder wasn't re-used)
				&& imageURLAtRequest == LazyData.GetOrCreate(itemIndex).imageURL; // be sure the model at that index is the same (could have changed if ChangeItemCountTo would've been called meanwhile)
		}

		BasicModel CreateRandomModel(int index)
		{
			return new BasicModel()
			{
				title = index + "",
				imageURL = DemosUtil.GetRandomSmallImageURL()
			};
		}

		void ImageDestroyer(object urlKey, object texture)
		{
			var asUnityObject = texture as UnityEngine.Object;
			if (asUnityObject != null)
				Destroy(asUnityObject);
		}
	}


	public class BasicModel
	{
		public string title;
		//public bool flexibleHeight;
		public string imageURL;
	}


	/// <summary>All views holders used with GridAdapter should inherit from <see cref="CellViewsHolder"/></summary>
	public class MyCellViewsHolder : CellViewsHolder
	{
		public RemoteImageBehaviour iconRemoteImageBehaviour;
		public Image loadingProgress, overlayImage;
		public Text title;
		//public Toggle flexibleHeightToggle;


		public override void CollectViews()
		{
			base.CollectViews();

			views.GetComponentAtPath("IconRawImage", out iconRemoteImageBehaviour);
			views.GetComponentAtPath("OverlayImage", out overlayImage);
			views.GetComponentAtPath("LoadingProgressImage", out loadingProgress);
			views.GetComponentAtPath("TitleText", out title);
			//views.GetComponentAtPath("FlexibleHeightToggle", out flexibleHeightToggle);
		}

		//public void UpdateFlexibleHeightFromToggleState()
		//{
		//	rootLayoutElement.flexibleHeight = flexibleHeightToggle.isOn ? 1f : -1f;
		//}
	}
}
