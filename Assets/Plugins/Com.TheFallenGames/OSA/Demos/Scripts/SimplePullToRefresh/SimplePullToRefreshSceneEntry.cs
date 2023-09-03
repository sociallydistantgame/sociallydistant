using System;
using System.Collections;
using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using System.Collections.Generic;
using frame8.Logic.Misc.Visual.UI;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;

namespace Com.TheFallenGames.OSA.Demos.SimplePullToRefresh
{
	/// <summary>
	/// Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code.
	/// See <see cref="SimplePullToRefreshExample"/> for additional info
	/// </summary>
	public class SimplePullToRefreshSceneEntry : DemoSceneEntry<SimplePullToRefreshExample, MyParams, MyViewsHolder>
	{
		const int FEW_ITEMS_FOR_INSERT = 3;
		const int MANY_ITEMS_FOR_INSERT = 500;

		LabelWithToggles _OnPullActionTogglesPanel;
		OnPullAction _OnPullAction;


		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false);
			_Drawer.serverDelaySetting.inputField.text = 1f + "";
			_Drawer.galleryEffectSetting.slider.value = 0f;

			_OnPullActionTogglesPanel = _Drawer.AddLabelWithTogglesPanel(
				"On release pull: ", 
				"Refresh", 
				"+" + FEW_ITEMS_FOR_INSERT + "", 
				"+" + MANY_ITEMS_FOR_INSERT + ""
			);
			_OnPullActionTogglesPanel.transform.SetAsFirstSibling();

			_OnPullActionTogglesPanel.ToggleChanged += (index, isOn) =>
			{
				if (!isOn)
					return;

				_OnPullAction = (OnPullAction)index;

				// The content's fixed edge can only be chosen by the user on refresh, because on insert it is chosen automatically, based on the direction the refresh is done
				_Drawer.freezeContentEndEdgeToggle.transform.parent.gameObject.SetActive(_OnPullAction == OnPullAction.REFRESH);
			};

			var toggleToActivateFirst = _OnPullActionTogglesPanel.subItems[(int)OnPullAction.INSERT_FEW_AT_EDGE].toggle;
			toggleToActivateFirst.isOn = true;
			toggleToActivateFirst.onValueChanged.Invoke(true);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();
			
			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnAddItemRequested(SimplePullToRefreshExample adapter, int index)
		{
			base.OnAddItemRequested(adapter, index);

			_Adapters[0].Data.InsertOne(index, CreateRandomModel(index), _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnRemoveItemRequested(SimplePullToRefreshExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			if (_Adapters[0].Data.Count == 0)
				return;

			_Adapters[0].Data.RemoveItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnItemCountChangeRequested(SimplePullToRefreshExample adapter, int newCount)
		{
			base.OnItemCountChangeRequested(adapter, newCount);

			var models = CreateRandomModels(newCount);
			_Adapters[0].Data.ResetItems(models, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		#endregion

		/// <summary>Exposed for the <see cref="Util.PullToRefresh.PullToRefreshBehaviour"/> script</summary>
		public void OnPullReleased(float sign)
		{
			var adapter = _Adapters[0];
			int itemsForInsert = -1;
			switch (_OnPullAction)
			{
				case OnPullAction.REFRESH:
					StartCoroutine(
						FetchItemModelsFromServer(adapter.Data.Count, OnReceivedNewModelsForReset)
					);
					return;

				case OnPullAction.INSERT_FEW_AT_EDGE:
					itemsForInsert = FEW_ITEMS_FOR_INSERT;
					break;

				case OnPullAction.INSERT_MANY_AT_EDGE:
					itemsForInsert = MANY_ITEMS_FOR_INSERT;
					break;
			}

			// Inserting at start or at end, depending on where the pull occurred.
			bool atEnd = sign < 0;
			StartCoroutine(
				FetchItemModelsFromServer(itemsForInsert, models => OnReceivedNewModelsForInsert(models, atEnd))
			);
		}

		IEnumerator FetchItemModelsFromServer(int count, Action<IList<MyModel>> onDone)
		{
			// Simulating server delay
			yield return new WaitForSeconds(_Drawer.serverDelaySetting.InputFieldValueAsInt);

			var models = CreateRandomModels(count);

			onDone(models);
		}

		void OnReceivedNewModelsForReset(IList<MyModel> newModels)
		{
			if (_Adapters.Length > 0 && _Adapters[0] != null)
				_Adapters[0].Data.ResetItems(newModels, _Drawer.freezeContentEndEdgeToggle.isOn);
		}

		void OnReceivedNewModelsForInsert(IList<MyModel> newModels, bool atEnd)
		{
			if (_Adapters.Length > 0 && _Adapters[0] != null)
			{
				var adapter = _Adapters[0];
				int insertIndex = atEnd ? adapter.GetItemsCount() : 0;

				// When pulling from start (and thus inserting at start), we freeze the content's end edge, so 
				// we'll see the new items appear before the current ones. Same thing for pulling from end.
				// This is not important, but it looks good.
				bool freezeContentEndEdge = !atEnd;

				adapter.Data.InsertItems(insertIndex, newModels, freezeContentEndEdge);
			}
		}

		IList<MyModel> CreateRandomModels(int count)
		{
			// Create the requested number of random models
			var models = new List<MyModel>(count);
			for (int i = 0; i < count; i++)
				models.Add(CreateRandomModel(i));

			return models;
		}

		MyModel CreateRandomModel(int itemIdex)
		{
			return new MyModel()
			{
				title = "Item ",
			};
		}


		enum OnPullAction
		{
			REFRESH,
			INSERT_FEW_AT_EDGE,
			INSERT_MANY_AT_EDGE,
		}
	}
}
