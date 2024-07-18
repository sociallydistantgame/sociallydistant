using AcidicGUI;
using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.GameplaySystems.Chat;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatInputField : Widget,
    IGainFocusHandler,
    ILoseFocusHandler,
    IChatMessageField
{
    private readonly InputField             inputField = new();
    private readonly ConversationBranchList branchList = new();
    
    public event Action<string>? OnSubmit
    {
        add => inputField.OnSubmit += value;
        remove => inputField.OnSubmit -= value;
    }
    
    public ChatInputField()
    {
        inputField.Placeholder = "Send message...";
        inputField.WordWrapped = true;
        Children.Add(inputField);
        Children.Add(branchList);
        
        inputField.OnValueChanged += OnQueryChanged;
    }

    private void OnQueryChanged(string query)
    {
        branchList.Filter = query;
    }

    protected override Point GetContentSize(Point availableSize)
    {
        return inputField.GetCachedContentSize(availableSize);
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        inputField.UpdateLayout(context, availableSpace);

        var branchListSize = branchList.GetCachedContentSize(new Point(availableSpace.Width, 300));

        var top = availableSpace.Top - branchListSize.Y;
        var left = availableSpace.Left;
        var width = availableSpace.Width;
        var height = branchListSize.Y;

        branchList.UpdateLayout(context, new LayoutRect(left, top, width, height));
    }

    public void UpdateBranchList(BranchDefinitionList branches)
    {
        branchList.UpdateBranchList(branches);
    }

    public void OnFocusGained(FocusEvent e)
    {
        branchList.ShouldShow = true;
    }

    public void OnFocusLost(FocusEvent e)
    {
        branchList.ShouldShow = false;
    }

    public void SetTextWithoutNotify(string text)
    {
        this.inputField.Value = text;
    }

    public bool PickSelectedBranchIfAny()
    {
        return branchList.PickSelectedIfAny();
    }
}