using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.GameplaySystems.WebPages;

namespace SociallyDistant.UI.WebSites.SocialMedia;

[WebSite("flock.social")]
public class SocialMediaWebSite : WebSite
{
    private readonly ISocialService socialService;
    private readonly FlexPanel      root            = new();
    private readonly OverlayWidget  mainArea        = new();
    private readonly Timeline       timeline        = new();
    private readonly ProfilePage    profilePage     = new();
    private readonly FlexPanel      navigationPanel = new();
    private readonly Button         playerButton    = new();
    private readonly Avatar         playerAvatar    = new();
    
    public SocialMediaWebSite()
    {
        socialService = Application.Instance.Context.SocialService;

        playerAvatar.AvatarSize = 96;
        
        root.Padding = new Padding(240, 16);
        root.Spacing = 12;
        root.Direction = Direction.Horizontal;

        mainArea.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;

        playerButton.HorizontalAlignment = HorizontalAlignment.Left;

        playerButton.Content = playerAvatar;
        Children.Add(root);
        root.ChildWidgets.Add(navigationPanel);
        root.ChildWidgets.Add(mainArea);
        mainArea.ChildWidgets.Add(profilePage);
        mainArea.ChildWidgets.Add(timeline);
        navigationPanel.ChildWidgets.Add(playerButton);
        
        playerButton.Clicked += OnPlayerButtonClicked;
    }

    private void OnPlayerButtonClicked()
    {
        ShowProfile(socialService.PlayerProfile);
    }

    protected override void GoToIndex()
    {
        timeline.Visibility = Visibility.Visible;
        profilePage.Visibility = Visibility.Collapsed;

        timeline.ShowPosts(socialService.GetTimeline(socialService.PlayerProfile));
    }
    
    [WebPage("profile", ":username")]
    private void ShowProfile(string username)
    {
        IProfile? user = socialService.Profiles
            .FirstOrDefault(x => x.SocialHandle == username);

        if (user == null)
            return;

        ShowProfile(user);
    }
    
    public void ShowProfile(IProfile profile)
    {
        if (profile == socialService.PlayerProfile)
        {
            //playerProfileToggle.SetIsOnWithoutNotify(true);
        }
        else
        {
            //otherPagesToggle.SetIsOnWithoutNotify(true);
        }


        this.profilePage.ShowProfile(profile);

        timeline.Visibility = Visibility.Collapsed;
        profilePage.Visibility = Visibility.Visible;
    }
}