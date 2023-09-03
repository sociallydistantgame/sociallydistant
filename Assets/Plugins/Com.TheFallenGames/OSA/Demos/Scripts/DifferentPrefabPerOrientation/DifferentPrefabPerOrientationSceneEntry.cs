using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.Demos.DifferentPrefabPerOrientation.ViewsHolders;
using Com.TheFallenGames.OSA.Demos.DifferentPrefabPerOrientation.Models;

namespace Com.TheFallenGames.OSA.Demos.DifferentPrefabPerOrientation
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class DifferentPrefabPerOrientationSceneEntry : DemoSceneEntry<DifferentPrefabPerOrientationExample, MyParams, BaseVH>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(DifferentPrefabPerOrientationExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			// Generating some random models
			var newModels = new CommonModel[count];
			for (int i = 0; i < count; ++i)
			{
				newModels[i] = new CommonModel()
				{
					Title = "Item #" + i,
					Content = DemosUtil.GetRandomTextBody(200, 400)
				};
			}

			adapter.Data.ResetItems(newModels);
		}
		#endregion
	}
}
