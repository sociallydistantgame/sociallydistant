using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using GamePlatform;

namespace UI.Login
{
	public class UserListController : OSA<BaseParamsWithPrefab, UserViewsHolder>
	{
		private SimpleDataHelper<UserListItemModel> users;

		public event Action<IGameData?>? GameDataSelected; 
		
		/// <inheritdoc />
		protected override void Awake()
		{
			users = new SimpleDataHelper<UserListItemModel>(this);
			base.Awake();
		}

		public void SetItems(IList<UserListItemModel> userList)
		{
			if (!this.IsInitialized)
				this.Init();
			
			this.users.ResetItems(userList);
		}
        
		/// <inheritdoc />
		protected override UserViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new UserViewsHolder();
			
			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex, false);

			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(UserViewsHolder newOrRecycled)
		{
			UserListItemModel model = users[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateView(model);

			newOrRecycled.Callback = OnClickCallback;
			
			ScheduleComputeVisibilityTwinPass();
		}

		private void OnClickCallback(IGameData? gameData)
		{
			GameDataSelected?.Invoke(gameData);
		}
	}
}