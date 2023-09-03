using System.Collections.Generic;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.CustomParams;

namespace Com.TheFallenGames.OSA.Demos.SimpleNestedScrollViews
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class SimpleNestedScrollViewsEntry : DemoSceneEntry<SimpleNestedScrollViewsExample, BaseParamsWithPrefab, SimplePageVH>
	{

		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, false, false, false, false);
			_Drawer.forceLowFPSSetting.gameObject.SetActive(false);
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(SimpleNestedScrollViewsExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			adapter.CreatePageModelAtIndexFunc = CreateNewModel;

			// We tell the LazyDataHelper how many items we have, and it in turn calls CreatePageModelAtIndexFunc whenever it needs
			// a specific page's model.
			adapter.Data.ResetItems(count);
		}
		#endregion

		/// <summary>Creates a PageModel containing a title and a randomly generated list of sub-items for the child List</summary>
		PageModel CreateNewModel(int itemIdex)
		{
			// Not choosing too many items as that might freeze the UI. Note that you can also use a LazyDataHelper for the
			// child list's items themselves, but that's outside our scope
			int numChildItemsToAdd = UnityEngine.Random.Range(1, 10000);
			var childListItems = new List<ChildListItemModel>(numChildItemsToAdd);
			while (numChildItemsToAdd-- > 0)
				childListItems.Add(new ChildListItemModel { Text = DemosUtil.GetRandomTextBody(1, 30) });

			return new PageModel()
			{
				Title = "Page " + itemIdex,
				Items = childListItems
			};
		}
	}
}
