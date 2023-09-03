using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.ViewsHolders;
using Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.Models;
using System;

namespace Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class SimpleMultiplePrefabsSceneEntry : DemoSceneEntry<SimpleMultiplePrefabsExample, MyParams, SimpleBaseVH>
	{
		/// <summary>List from which ad textures are randomly sampled</summary>
		[SerializeField]
		Texture2D[] _AdTextures = null;

		// For every 20 items, have an ad at that position
		const int ADS_INTERVAL = 20; 


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
		protected override void OnItemCountChangeRequested(SimpleMultiplePrefabsExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			// Generating some random models
			var newModels = new SimpleBaseModel[count];
			for (int i = 0; i < count; ++i)
			{
				int modelType;
				if ((i+1) % ADS_INTERVAL == 0)
					modelType = 2; // ad
				else
					modelType = UnityEngine.Random.Range(0, 2); // random between orange and green
				newModels[i] = CreateRandomModel(i, modelType);
			}

			adapter.Data.ResetItems(newModels);
		}
		#endregion

		/// <summary>Creates a random model of the specified type: 0 = Green, 1 = Orange, 2 = Ad</summary>
		SimpleBaseModel CreateRandomModel(int index, int type)
		{
			SimpleBaseModel model;
			if (type == 0)
				model = new GreenModel() { textContent = DemosUtil.GetRandomTextBody(10, 70) };
			else if (type == 1)
				model = new OrangeModel() { value = UnityEngine.Random.Range(0f, 1f) };
			else
			{
				model = new AdModel()
				{
					adID = Guid.NewGuid().ToString(),
					adTexture = _AdTextures[UnityEngine.Random.Range(0, _AdTextures.Length)]
				};
			}

			return model;
		}
	}
}
