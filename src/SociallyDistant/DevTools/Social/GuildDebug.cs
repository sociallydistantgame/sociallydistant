using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools.Social
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
			if (ImGui.Button("Create New Guild"))
				devMenu.PushMenu(new CreateGuildMenu(world));
			
			ImGui.Text("Existing Guilds");

			foreach (WorldGuildData data in world.World.Guilds)
			{
				if (ImGui.Button($"Guild {data.InstanceId}: {data.Name}"))
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
			ImGui.Text($"Viewing guild ID: {guildId}");

			if (!world.World.Guilds.ContainsId(guildId))
			{
				ImGui.Text("Guild doesn't exist");
				return;
			}

			WorldGuildData guild = world.World.Guilds[guildId];

			ImGui.Text($"Name: {guild.Name}");

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
			
			ImGui.Text("Members in guild: {members.Count}");

			foreach (WorldMemberData member in members)
			{
				WorldProfileData profile = new();
				profile.ChatName = "Invalid";

				if (world.World.Profiles.ContainsId(member.ProfileId))
					profile = world.World.Profiles[member.ProfileId];
				
				ImGui.Columns(2);

				ImGui.Text($"{member.InstanceId}: Profile {profile.InstanceId} ({profile.ChatName})");
				ImGui.NextColumn();

				if (ImGui.Button("Kick"))
				{
					world.World.Members.Remove(member);
				}

				ImGui.Columns(1);
			}

			if (ImGui.Button("Add member to guild"))
				devMenu.PushMenu(new AddGuildMember(world, guildId, members));

			if (!playerInGuild && ImGui.Button("Join Player to Guild"))
			{
				world.World.Members.Add(new WorldMemberData
				{
					InstanceId = world.GetNextObjectId(),
					GroupType = MemberGroupType.Guild,
					GroupId = guild.InstanceId,
					ProfileId = world.World.PlayerData.Value.PlayerProfile
				});
			}

			ImGui.Text($"Channels in guild: {groups.Count}");

			foreach (WorldChannelData channel in groups)
			{
				ImGui.Text($"{channel.InstanceId}: {channel.Name}");
				ImGui.Text(channel.Description);
			}
			
			ImGui.Text("Create New Channel");

			//newChannel.Name = GUILayout.TextField(newChannel.Name);
			//newChannel.Description = GUILayout.TextField(newChannel.Description);

			if (ImGui.Button("Create"))
			{
				newChannel.InstanceId = world.GetNextObjectId();
				newChannel.ChannelType = MessageChannelType.Guild;
				newChannel.GuildId = guild.InstanceId;

				world.World.Channels.Add(newChannel);
			}
		}
	}
}