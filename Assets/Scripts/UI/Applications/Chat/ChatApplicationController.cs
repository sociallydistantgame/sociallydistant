#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Core.WorldData.Data;
using GameplaySystems.Networld;
using GameplaySystems.Social;
using Social;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UniRx;

namespace UI.Applications.Chat
{
	public class ChatApplicationController : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private SocialServiceHolder socialService = null!;
		
		[Header("UI")]
		[SerializeField]
		private ChatHeader chatHeader = null!;

		[SerializeField]
		private ServerMembersController serverMembers = null!;

		[SerializeField]
		private ChatConversationController conversationController = null!;

		[SerializeField]
		private GuildListView guildList = null!;

		[SerializeField]
		private Toggle homeButton = null!;

		private IGuildList? playerGuilds;
		private IDisposable? guildJoinObserver;
		private IDisposable? guildLeaveObserver;
		private IGuild? currentGuild;
		
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatApplicationController));
		}

		private void Start()
		{
			if (socialService.Value == null)
				return;
			
			playerGuilds = socialService.Value.GetGuilds().ThatHaveMember(socialService.Value.PlayerProfile);
			guildJoinObserver = playerGuilds.ObserveGuildAdded().Subscribe(OnGuildJoin);
			guildLeaveObserver = playerGuilds.ObserveGuildRemoved().Subscribe(OnGuildLeave);
			
			
			homeButton.onValueChanged.AddListener(OnHomeButtonToggle);
			guildList.GuildSelected += ShowGuild;

			RefreshGuildList();
			ShowDirectMessagesList();
		}

		private void OnDestroy()
		{
			playerGuilds?.Dispose();
		}

		public void ShowGuild(IGuild guild)
		{
			this.currentGuild = guild;

			this.serverMembers.IsVisible = true;
			this.serverMembers.UpdateMembers(currentGuild.Members.Select(x => new ServerMember
			{
				Username = x.Profile.ChatUsername,
				DisplayName = x.Profile.ChatName
			}).ToList());
		}

		public void ShowDirectMessagesList()
		{
			this.currentGuild = null;
			homeButton.SetIsOnWithoutNotify(true);

			this.serverMembers.IsVisible = false;
		}
		
		private void OnHomeButtonToggle(bool isOn)
		{
			if (isOn)
				ShowDirectMessagesList();
		}

		private void RefreshGuildList()
		{
			this.guildList.SetItems(playerGuilds.Select(x => new GuildItemModel()
			{
				Guild = x,
				ToggleGroup = this.homeButton.group,
				IsSelected = x == currentGuild
			}).ToList());
		}
		
		private void OnGuildLeave(IGuild guild)
		{
			if (guild == currentGuild)
				ShowDirectMessagesList();
			
			RefreshGuildList();
		}
        
		private void OnGuildJoin(IGuild guild)
		{
			ShowGuild(guild);
			RefreshGuildList();
		}
	}
}