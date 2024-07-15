using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Architecture;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.CharacterCreation;

public sealed class LifepathWidget : Widget
{
    private readonly ListItem       listItem     = new();
    private readonly StackPanel     root         = new();
    private readonly Avatar         avatar       = new();
    private readonly StackPanel     textStack    = new();
    private readonly TextWidget     lifepathName = new();
    private readonly TextWidget     description  = new();
    private          LifepathAsset? lifepathId;

    public Action<LifepathAsset>? Callback { get; set; }
    
    public LifepathWidget()
    {
        root.Direction = Direction.Horizontal;
        root.Spacing = 6;

        textStack.Direction = Direction.Vertical;

        lifepathName.FontWeight = FontWeight.SemiBold;

        avatar.AvatarSize = 48;
        
        Children.Add(listItem);
        listItem.Content = root;

        root.ChildWidgets.Add(avatar);
        root.ChildWidgets.Add(textStack);

        textStack.ChildWidgets.Add(lifepathName);
        textStack.ChildWidgets.Add(description);

        lifepathName.Text = "Redshedder";
        description.Text = "Placeholder text";

        listItem.ClickCallback = OnClick;
    }

    public void UpdateView(LifepathAsset model)
    {
        lifepathId = model;
        lifepathName.Text = model.LifepathName;
        description.Text = model.Description;
    }

    private void OnClick()
    {
        if (lifepathId == null)
            return;
        
        Callback?.Invoke(lifepathId);
    }
}