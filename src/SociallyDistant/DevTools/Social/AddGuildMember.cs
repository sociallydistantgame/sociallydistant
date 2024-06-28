using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools.Social;

public class AddGuildMember : IDevMenu
{
    private readonly WorldManager world;
    private readonly ObjectId guild;
    private readonly IReadOnlyList<ObjectId> membersBlocked;

    public AddGuildMember(WorldManager world, ObjectId guild, IReadOnlyList<WorldMemberData> members)
    {
        this.world = world;
        this.guild = guild;
        this.membersBlocked = members.Select(x => x.ProfileId).ToList();
    }

    /// <inheritdoc />
    public string Name => "Add member to guild";

    /// <inheritdoc />
    public void OnMenuGUI(DeveloperMenu devMenu)
    {
        foreach (WorldProfileData profile in world.World.Profiles)
        {
            if (membersBlocked.Contains(profile.InstanceId))
                continue;

            if (ImGui.Button($"{profile.InstanceId}: {profile.ChatName} (@{profile.ChatUsername})"))
            {
                world.World.Members.Add(new WorldMemberData
                {
                    InstanceId = world.GetNextObjectId(),
                    ProfileId = profile.InstanceId,
                    GroupId = guild,
                    GroupType = MemberGroupType.Guild
                });

                devMenu.PopMenu();
            }
        }
    }
}