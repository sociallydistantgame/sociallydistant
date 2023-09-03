using System;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.Chat
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class ChatSceneEntry : DemoSceneEntry<ChatExample, MyParams, ChatMessageViewsHolder>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, false, false, true, false);
			_Drawer.galleryEffectSetting.slider.value = .04f;

			// No adding/removing at the head of the list
			_Drawer.addRemoveOnePanel.button2.gameObject.SetActive(false);
			_Drawer.addRemoveOnePanel.button4.gameObject.SetActive(false);

			// No removing whatsoever. Only adding
			_Drawer.addRemoveOnePanel.button3.gameObject.SetActive(false);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			OnItemCountChangeRequested(_Adapters[0], 3);
		}

		#region events from DrawerCommandPanel
		protected override void OnAddItemRequested(ChatExample adapter, int index)
		{
			base.OnAddItemRequested(adapter, index);

			adapter.Data.InsertOne(index, CreateRandomModel(index), true);
		}
		protected override void OnItemCountChangeRequested(ChatExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			// Generating some random models
			var newModels = new ChatMessageModel[count];
			for (int i = 0; i < count; ++i)
				newModels[i] = CreateRandomModel(i, i != 1); // the second model will always have an image, for demo purposes

			adapter.Data.ResetItems(newModels, true);
		}
		#endregion

		ChatMessageModel CreateRandomModel(int itemIdex, bool addImageOnlyRandomly = true)
		{
			return new ChatMessageModel()
			{
				timestampSec = (Int32)(DateTime.UtcNow.Subtract(ChatMessageModel.EPOCH_START_TIME)).TotalSeconds,
				Text = GetRandomContent(),
				IsMine = UnityEngine.Random.Range(0, 2) == 0,
				ImageIndex = addImageOnlyRandomly ?
								// Twice as many messages without photo as with photo
								UnityEngine.Random.Range(
									-2 * _Adapters[0].Parameters.availableChatImages.Length,
									_Adapters[0].Parameters.availableChatImages.Length
								)
								: 0
			};
		}

		string GetRandomContent() { return DemosUtil.GetRandomTextBody(0, UnityEngine.Random.Range(DemosUtil.LOREM_IPSUM.Length / 50 + 1, DemosUtil.LOREM_IPSUM.Length / 2)); }
	}
}
