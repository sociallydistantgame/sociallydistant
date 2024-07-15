using System.Globalization;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.Core.UI.Effects;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.MainMenu;

public class LoginScreen : Widget
{
    private readonly SociallyDistantGame game;
    private readonly OverlayWidget       overlay              = new();
    private readonly StackPanel          versionArea          = new();
    private readonly StackPanel          mainArea             = new();
    private readonly TextWidget          versionText          = new();
    private readonly StackPanel          usersArea            = new();
    private readonly Image               gameLogo             = new();
    private readonly TextWidget          title                = new();
    private readonly TextWidget          prompt               = new();
    private readonly Box                 userBox              = new();
    private readonly TextButton          createNewUserButton  = new();
    private readonly StackPanel          currentUserStack     = new();
    private readonly Avatar              currentUserAvatar    = new();
    private readonly TextWidget          currentUserName      = new();
    private readonly TextWidget          currentUserInfo      = new();
    private readonly TextButton          loginButtonn         = new();
    private readonly TextButton          switchUserButton     = new();
    private readonly ToolbarIcon         userOptionsButton    = new();
    private readonly StackPanel          currentUserInfoStack = new();
    private readonly LoginListADapter    usersList            = new();
    private readonly WrapPanel           userOptionsArea      = new();
    private          IGameData?          currentProfile;
    
    internal LoginScreen(SociallyDistantGame game)
    {
        currentUserInfo.UseMarkup = true;
        currentUserInfo.WordWrapping = true;
        
        userOptionsArea.Direction = Direction.Horizontal;
        userOptionsArea.SpacingX = 3;
        userOptionsArea.Padding = new Padding(0, 3, 0, 0);
        
        currentUserName.FontWeight = FontWeight.Bold;
        currentUserAvatar.AvatarSize = 72;
        
        currentUserStack.Direction = Direction.Horizontal;
        createNewUserButton.Text = "Create New User";
        switchUserButton.Text = "Switch User";
        loginButtonn.Text = "Log in";
        userOptionsButton.Icon = MaterialIcons.Settings;
        
        title.FontSize = 36;
        prompt.FontSize = 20;
        
        this.game = game;

        overlay.Padding = 16;

        versionText.UseMarkup = true;
        
        versionArea.VerticalAlignment = VerticalAlignment.Bottom;
        versionArea.HorizontalAlignment = HorizontalAlignment.Left;

        mainArea.VerticalAlignment = VerticalAlignment.Middle;
        mainArea.HorizontalAlignment = HorizontalAlignment.Left;

        usersArea.MaximumSize = new Point(391, 320);
        usersArea.MinimumSize = new Point(391, 0);
        usersArea.SetCustomProperty(WidgetBackgrounds.Overlay);
        usersArea.Margin = 12;

        
        
        Children.Add(overlay);
        overlay.ChildWidgets.Add(versionArea);
        overlay.ChildWidgets.Add(mainArea);
        versionArea.ChildWidgets.Add(gameLogo);
        versionArea.ChildWidgets.Add(versionText);
        mainArea.ChildWidgets.Add(title);
        mainArea.ChildWidgets.Add(prompt);
        mainArea.ChildWidgets.Add(usersArea);
        usersArea.ChildWidgets.Add(currentUserStack);
        usersArea.ChildWidgets.Add(usersList);
        mainArea.ChildWidgets.Add(switchUserButton);
        mainArea.ChildWidgets.Add(createNewUserButton);
        currentUserStack.ChildWidgets.Add(currentUserAvatar);
        currentUserStack.ChildWidgets.Add(currentUserInfoStack);
        currentUserInfoStack.ChildWidgets.Add(currentUserName);
        currentUserInfoStack.ChildWidgets.Add(currentUserInfo);
        currentUserInfoStack.ChildWidgets.Add(userOptionsArea);
        userOptionsArea.ChildWidgets.Add(loginButtonn);
        userOptionsArea.ChildWidgets.Add(userOptionsButton);

        switchUserButton.ClickCallback = SwitchUser;
        createNewUserButton.ClickCallback = CreateNewUser;
        loginButtonn.ClickCallback = LogIn;
    }

    private async void LogIn()
    {
        if (currentProfile == null)
            return;
        
        await game.StartGame(currentProfile);
    }
    
    private void SwitchUser()
    {
        currentProfile = null;
        UpdateView();
    }

    private async void CreateNewUser()
    {
        await game.StartCharacterCreator();
    }
    
    public void Start()
    {
        this.RenderEffect = BackgroundBlurWidgetEffect.GetEffect(game);
        usersArea.RenderEffect = BackgroundBlurWidgetEffect.GetEffect(game);
        versionText.Text = $"version <b>{Application.Instance.Version}</b>";

        usersList.OnShowUser += ShowUser;
        
        UpdateView();
    }

    private void UpdateView()
    {
        if (currentProfile != null)
        {
            currentUserStack.Visibility = Visibility.Visible;
            switchUserButton.Visibility = Visibility.Visible;
            createNewUserButton.Visibility = Visibility.Collapsed;
            usersList.Visibility = Visibility.Collapsed;

            currentUserName.Text = currentProfile.PlayerInfo.Name;

            currentUserInfo.Text = SociallyDistantUtility.CreateFormattedDataMarkup(new Dictionary<string, string>() { { "Last played", currentProfile.PlayerInfo.LastPlayed.ToString(CultureInfo.CurrentCulture) }, { "Mission", currentProfile.PlayerInfo.Comment } });
        }
        else
        {
            usersList.SetUsers(game.ContentManager.GetContentOfType<IGameData>().OrderByDescending(x => x.PlayerInfo.LastPlayed));
            
            title.Text = "Welcome";
            prompt.Text = "Please log in.";
            
            currentUserStack.Visibility = Visibility.Collapsed;
            switchUserButton.Visibility = Visibility.Collapsed;
            createNewUserButton.Visibility = Visibility.Visible;
            usersList.Visibility = Visibility.Visible;
        }
    }

    private void ShowUser(IGameData user)
    {
        this.currentProfile = user;
        UpdateView();
    }
}