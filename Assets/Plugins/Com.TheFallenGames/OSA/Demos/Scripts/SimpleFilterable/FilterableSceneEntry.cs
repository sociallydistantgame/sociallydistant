using System;
using System.Collections;
using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.Demos.SimpleFilterable
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class FilterableSceneEntry : DemoSceneEntry<SimpleFilterExample, FilterableMyParams, FilterableViewHoler>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, true, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnAddItemRequested(SimpleFilterExample adapter, int index)
		{
			base.OnAddItemRequested(adapter, index);
			_Adapters[0].Data.InsertOne(_Adapters[0].Data.GetUnfilteredIdex(index), CreateRandomModel(index), _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnRemoveItemRequested(SimpleFilterExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			//if (_Adapters[0].Data.Count < index)
			//{
			//	Debug.LogWarning("Please make sure that the index is smaller than the filtered item count");
			//	return;
			//}

			_Adapters[0].Data.RemoveItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnItemCountChangeRequested(SimpleFilterExample adapter, int newCount)
		{
			base.OnItemCountChangeRequested(adapter, newCount);

			StartCoroutine(
				FetchItemModelsFromServer(newCount, OnReceivedNewModelsForReset)
			);
		}
		#endregion

		IEnumerator FetchItemModelsFromServer(int count, Action<IList<FilterableExampleItemModel>> onDone)
		{
			// Simulating server delay
			yield return new WaitForSeconds(.1f);

			// Create the requested number of random models
			var models = new List<FilterableExampleItemModel>(count);
			for (int i = 0; i < count; i++)
				models.Add(CreateRandomModel(i));

			onDone(models);
		}

		void OnReceivedNewModelsForReset(IList<FilterableExampleItemModel> newModels)
		{
			if (_Adapters.Length > 0 && _Adapters[0] != null)
				_Adapters[0].Data.ResetItems(newModels, _Drawer.freezeContentEndEdgeToggle.isOn);
		}

		FilterableExampleItemModel CreateRandomModel(int itemIdex)
		{
			var parameters = _Adapters[0].Parameters;
			int numIcons = parameters.availableIcons.Length;

			return new FilterableExampleItemModel()
			{
				title = "Item ",
				icon1Index = UnityEngine.Random.Range(0, numIcons),
				icon2Index = UnityEngine.Random.Range(0, numIcons)
			};
		}
	}
}
