using System;
using System.Collections.Generic;
using UI.ScrollViews;

namespace UI.Applications.Chat
{
	public class ServerMembersListView : ScrollViewController<ServerMemberViewsHolder>
	{
		private ScrollViewItemList<ServerMember> members;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			members = new ScrollViewItemList<ServerMember>(this);
		}

		public void SetItems(IList<ServerMember> memberList)
		{
			this.members.SetItems(memberList);
		}

		/// <inheritdoc />
		protected override ServerMemberViewsHolder CreateModel(int itemIndex)
		{
			var vh = new ServerMemberViewsHolder(itemIndex);
			
			//vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(ServerMemberViewsHolder newOrRecycled)
		{
			ServerMember member = members[newOrRecycled.ItemIndex];

			newOrRecycled.UpdateMember(member);
		}
	}
}