using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.NestedScrollViews
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class NestedScrollViewsSceneEntry : DemoSceneEntry<NestedScrollViewsExample, MyParams, ChildAdapterViewsHolder>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 1f;
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(NestedScrollViewsExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			var availableImages = adapter.Parameters.availableIntroImages;
			adapter.Data.List.Clear();
			for (int i = 0; i < count; i++)
				adapter.Data.List.Add(CreateNewModel(availableImages[UnityEngine.Random.Range(0, availableImages.Length)], i));
			adapter.Data.NotifyListChangedExternally();
		}
		#endregion


		ChildAdapterModel CreateNewModel(Sprite introImage, int itemIdex)
		{
			return new ChildAdapterModel()
			{
				title = "Section " + itemIdex,
				introImage = introImage
			};
		}
	}
}
