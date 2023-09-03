using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.PageView
{
	/// <summary>
	/// </summary>
	public class PageViewSceneEntry : DemoSceneEntry<PageViewExample, MyParams, PageViewsHolder>
	{
		[SerializeField]
		Sprite[] _AvailableImages = null; // used to randomly generate models;


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
		protected override void OnItemCountChangeRequested(PageViewExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			// Alternatively, we could construct the list here as a local variable and call adapter.Data.ResetItems directly, passing the new list, 
			// but if we can choose, it's better to reuse the same list that's already there. This is optimal only if using Reset, not partial Inserts/Removes
			adapter.Data.List.Clear();
			for (int i = 0; i < count; ++i)
			{
				var m = CreateNewModel(i, DemosUtil.GetRandomTextBody(180), _AvailableImages[UnityEngine.Random.Range(0, _AvailableImages.Length)]);
				adapter.Data.List.Add(m);
			}
			adapter.Data.NotifyListChangedExternally();
		}
		#endregion

		PageModel CreateNewModel(int itemIdex, string body, Sprite icon)
		{
			return new PageModel()
			{
				title = "Page " + itemIdex,
				body = body,
				image = icon
			};
		}
	}
}
