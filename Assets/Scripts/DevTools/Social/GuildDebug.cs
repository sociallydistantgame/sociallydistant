using System.Collections.Generic;
using Core;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools.Social
{
	public class GuildDebug : IDevMenu
	{
		private readonly WorldManager world;
			
		public GuildDebug(WorldManager world)
		{
			this.world = world;
		}

		/// <inheritdoc />
		public string Name => "Chat Guilds";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (GUILayout.Button("Create New Guild"))
				devMenu.PushMenu(new CreateGuildMenu(world));
			
			GUILayout.Label("Existing Guilds");

			foreach (WorldGuildData data in world.World.Guilds)
			{
				if (GUILayout.Button($"Guild {data.InstanceId}: {data.Name}"))
					devMenu.PushMenu(new GuildInfoMenu(world, data.InstanceId));
			}
		}
	}

	public class GuildInfoMenu : IDevMenu
	{
		private readonly WorldManager world;
		private readonly ObjectId guildId;
		private WorldChannelData newChannel;
		
		/// <inheritdoc />
		public string Name => "View guild";

		public GuildInfoMenu(WorldManager world, ObjectId instanceId)
		{
			this.world = world;
			this.guildId = instanceId;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.Label($"Viewing guild ID: {guildId}");

			if (!world.World.Guilds.ContainsId(guildId))
			{
				GUILayout.Label("Guild doesn't exist");
				return;
			}

			WorldGuildData guild = world.World.Guilds[guildId];

			GUILayout.Label($"Name: {guild.Name}");

			var members = new List<WorldMemberData>();
			var groups = new List<WorldChannelData>();
			var playerInGuild = false;

			foreach (WorldChannelData channel in world.World.Channels)
			{
				if (channel.ChannelType != MessageChannelType.Guild)
					continue;

				if (channel.GuildId != guild.InstanceId)
					continue;
				
				groups.Add(channel);
			}
			
			foreach (WorldMemberData member in world.World.Members)
			{
				if (member.GroupId != guild.InstanceId || member.GroupType != MemberGroupType.Guild) 
					continue;
				
				members.Add(member);
				if (member.ProfileId == world.World.PlayerData.Value.PlayerProfile)
					playerInGuild = true;
			}
			
			GUILayout.Label("Members in guild: {members.Count}");

			foreach (WorldMemberData member in members)
			{
				WorldProfileData profile = new();
				profile.ChatName = "Invalid";

				if (world.World.Profiles.ContainsId(member.ProfileId))
					profile = world.World.Profiles[member.ProfileId];
				
				GUILayout.BeginHorizontal();

				GUILayout.Label($"{member.InstanceId}: Profile {profile.InstanceId} ({profile.ChatName})");
				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Kick"))
				{
					world.World.Members.Remove(member);
				}
				
				GUILayout.EndHorizontal();
			}

			if (!playerInGuild && GUILayout.Button("Join Player to Guild"))
			{
				world.World.Members.Add(new WorldMemberData
				{
					InstanceId = world.GetNextObjectId(),
					GroupType = MemberGroupType.Guild,
					GroupId = guild.InstanceId,
					ProfileId = world.World.PlayerData.Value.PlayerProfile
				});
			}

			GUILayout.Label($"Channels in guild: {groups.Count}");

			foreach (WorldChannelData channel in groups)
			{
				GUILayout.BeginVertical();

				GUILayout.Label($"{channel.InstanceId}: {channel.Name}");
				GUILayout.Label(channel.Description);

				GUILayout.EndVertical();
				
				GUILayout.Space(3);
			}
			
			GUILayout.Label("Create New Channel");

			newChannel.Name = GUILayout.TextField(newChannel.Name);
			newChannel.Description = GUILayout.TextField(newChannel.Description);

			if (GUILayout.Button("Create"))
			{
				newChannel.InstanceId = world.GetNextObjectId();
				newChannel.ChannelType = MessageChannelType.Guild;
				newChannel.GuildId = guild.InstanceId;

				world.World.Channels.Add(newChannel);
			}
		}
	}
}