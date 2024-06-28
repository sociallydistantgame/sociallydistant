using ImGuiNET;
using SociallyDistant.GameplaySystems.Chat;

namespace SociallyDistant.DevTools;

public sealed class ConversationInfo : IDevMenu
{
    private readonly ConversationManager conversationManager;
    private readonly string id;

    private string chooseIdentifier = string.Empty;

    /// <inheritdoc />
    public string Name => $"Conversation info: {id}";

    public ConversationInfo(ConversationManager conversationManager, string id)
    {
        this.conversationManager = conversationManager;
        this.id = id;
    }
		
    /// <inheritdoc />
    public void OnMenuGUI(DeveloperMenu devMenu)
    {
        ImGui.Text($"Is Active: {conversationManager.IsConversationActive(id)}");
			
        ImGui.Text("Debug Actions");
        ImGui.Text("Mark Branch as Chosen (enter the identifier stated in the script)");
        chooseIdentifier = chooseIdentifier;
        if (ImGui.Button("MARK"))
        {
            conversationManager.ChooseBranch(id, chooseIdentifier);
            chooseIdentifier = string.Empty;
        }
    }
}