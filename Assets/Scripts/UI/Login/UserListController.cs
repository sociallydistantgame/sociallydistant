using System;
using System.Collections.Generic;
using GamePlatform;
using UI.ScrollViews;

namespace UI.Login
{
	public class UserListController : ScrollViewController<UserViewsHolder>
	{
		private ScrollViewItemList<UserListItemModel> users;

		public event Action<IGameData?>? GameDataSelected; 
		
		/// <inheritdoc />
		protected override void Awake()
		{
			users = new ScrollViewItemList<UserListItemModel>(this);
			base.Awake();
		}

		public void SetItems(IList<UserListItemModel> userList)
		{
			this.users.SetItems(userList);
		}
        
		/// <inheritdoc />
		protected override UserViewsHolder CreateModel(int itemIndex)
		{
			var vh = new UserViewsHolder();
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(UserViewsHolder newOrRecycled)
		{
			UserListItemModel model = users[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateView(model);

			newOrRecycled.Callback = OnClickCallback;
		}

		private void OnClickCallback(IGameData? gameData)
		{
			GameDataSelected?.Invoke(gameData);
		}
	}
}