using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.CustomParams;

namespace Com.TheFallenGames.OSA.Demos.NestedScrollViewsSameDirection
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class NestedScrollViewsSameDirectionSceneEntry : DemoSceneEntry<NestedScrollViewsSameDirectionExample, BaseParamsWithPrefab, ChildAdapterViewsHolder>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(NestedScrollViewsSameDirectionExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			adapter.Data.List.Clear();
			for (int i = 0; i < count; i++)
				adapter.Data.List.Add(CreateNewModel(i));
			adapter.Data.NotifyListChangedExternally();
		}
		#endregion


		ChildAdapterModel CreateNewModel(int itemIdex)
		{
			return new ChildAdapterModel()
			{
				title = "Section " + itemIdex
			};
		}
	}
}
