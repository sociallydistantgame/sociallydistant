using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace UI.Applications.Chat
{
	public class ServerMembersListView : OSA<BaseParamsWithPrefab, ServerMemberViewsHolder>
	{
		private SimpleDataHelper<ServerMember> members;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			members = new SimpleDataHelper<ServerMember>(this);
		}

		public void SetItems(IList<ServerMember> memberList)
		{
			if (!IsInitialized)
				Init();
			
			this.members.ResetItems(memberList);
		}

		/// <inheritdoc />
		protected override ServerMemberViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new ServerMemberViewsHolder();
			
			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(ServerMemberViewsHolder newOrRecycled)
		{
			ServerMember member = members[newOrRecycled.ItemIndex];

			newOrRecycled.UpdateMember(member);
			
			ScheduleComputeVisibilityTwinPass();
		}
	}
}