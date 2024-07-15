using System.Globalization;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.MainMenu;

public sealed class LoginUserView : Widget
{
    private readonly ListItem   listItem  = new();
    private readonly FlexPanel  root      = new();
    private readonly Avatar     avatar    = new();
    private readonly StackPanel textStack = new();
    private readonly TextWidget name      = new();
    private readonly TextWidget info      = new();
    
    private IGameData? gameData;
    
    public Action<IGameData>? Callback { get; set; }

    public LoginUserView()
    {
        root.Direction = Direction.Horizontal;
        root.Spacing = 6;
        
        name.FontWeight = FontWeight.Bold;
        
        Children.Add(listItem);
        listItem.Content = root;
        root.ChildWidgets.Add(avatar);
        root.ChildWidgets.Add(textStack);
        textStack.ChildWidgets.Add(name);
        textStack.ChildWidgets.Add(info);

        info.WordWrapping = true;
        info.UseMarkup = true;
        
        listItem.ClickCallback = OnClick;
        avatar.AvatarSize = 48;
    }

    private void OnClick()
    {
        if (gameData == null)
            return;
        
        this.Callback?.Invoke(gameData);
    }

    public void UpdateView(IGameData user)
    {
        this.gameData = user;

        name.Text = user.PlayerInfo.Name;
        info.Text = SociallyDistantUtility.CreateFormattedDataMarkup(new Dictionary<string, string>() { { "Last played", user.PlayerInfo.LastPlayed.ToString(CultureInfo.CurrentCulture) }, { "Mission", user.PlayerInfo.Comment } });
    }
}