using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace UI.Login
{
	public class UserListController : OSA<BaseParamsWithPrefab, UserViewsHolder>
	{
		private SimpleDataHelper<UserListItemModel> users;
        
		/// <inheritdoc />
		protected override void Awake()
		{
			users = new SimpleDataHelper<UserListItemModel>(this);
			base.Awake();
		}

		public void SetItems(IList<UserListItemModel> userList)
		{
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
		}
	}
}