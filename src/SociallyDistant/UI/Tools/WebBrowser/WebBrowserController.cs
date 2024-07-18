using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.GameplaySystems.WebPages;

namespace SociallyDistant.UI.Tools.WebBrowser;

public sealed class WebBrowserController : ProgramController
{
    private readonly SociallyDistantGame gameManager;
    private readonly FlexPanel           root       = new();
    private readonly FlexPanel           toolbar    = new();
    private readonly Box                 page       = new();
    private readonly ToolbarIcon         back       = new();
    private readonly ToolbarIcon         forward    = new();
    private readonly ToolbarIcon         home       = new();
    private readonly ToolbarIcon         go         = new();
    private readonly InputField          addressBar = new();
    private readonly Stack<string>       future     = new Stack<string>();
    private readonly Stack<string>       history    = new Stack<string>();
    private readonly string              homePage   = "web://start.page/";
    private          IDisposable?        websitePathObserver;
    private          WebSite?            current;
    
    public string CurrentUrl => addressBar.Value;
    
    private WebBrowserController(ProgramContext context) : base(context)
    {
        this.gameManager = SociallyDistantGame.Instance;

        toolbar.Direction = Direction.Horizontal;
        toolbar.Spacing = 3;
        toolbar.Padding = 3;

        back.Icon = MaterialIcons.ArrowBack;
        forward.Icon = MaterialIcons.ArrowForward;
        home.Icon = MaterialIcons.Home;
        go.Icon = MaterialIcons.Send;
        
        back.ClickHandler = GoBack;
        forward.ClickHandler = GoForward;
        home.ClickHandler = GoHome;
        go.ClickHandler = Navigate;
        
        addressBar.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        page.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        
        back.VerticalAlignment = VerticalAlignment.Middle;
        forward.VerticalAlignment = VerticalAlignment.Middle;
        home.VerticalAlignment = VerticalAlignment.Middle;
        addressBar.VerticalAlignment = VerticalAlignment.Middle;
        go.VerticalAlignment = VerticalAlignment.Middle;
        
        context.Window.Content = root;
        root.ChildWidgets.Add(toolbar);
        root.ChildWidgets.Add(page);
        toolbar.ChildWidgets.Add(back);
        toolbar.ChildWidgets.Add(forward);
        toolbar.ChildWidgets.Add(home);
        toolbar.ChildWidgets.Add(addressBar);
        toolbar.ChildWidgets.Add(go);

        addressBar.OnSubmit += NavigateTo;
    }

    public void Navigate(Uri uri)
    {
        addressBar.Value = uri.ToString();
        Navigate();
    }
    
    protected override void Main()
    {
        NavigateTo(homePage, false);
    }
    
    private void UpdateUI()
    {
        back.Enabled = history.Count > 0 || (current != null && current.CanGoBack);
        forward.Enabled = future.Count > 0 || (current != null && current.CanGoForward);

        if (current != null)
        {
            addressBar.Value = current.Url;
        }
    }
    
    private void GoBack()
    {
        if (current != null)
        {
            if (current.CanGoBack)
            {
                current.GoBack();
                UpdateUI();
                return;
            }

            page.Content = null;
        }

        future.Push(CurrentUrl);

        NavigateTo(history.Pop(), false);
    }
    
    private void GoForward()
    {
        if (current != null)
        {
            if (current.CanGoForward)
            {
                current.GoForward();
                UpdateUI();
                return;
            }

            page.Content = null;
        }

        history.Push(CurrentUrl);
        NavigateTo(future.Pop(), false);
    }
    
    private void GoHome()
    {
        NavigateTo(homePage, true);
    }
    
    private void NavigateTo(string urlOrSearchQuery)
    {
        NavigateTo(urlOrSearchQuery, true);
    }
    
    private void NavigateTo(string urlOrSearchQuery, bool addToHistory)
    {
        if (!ParseUrl(urlOrSearchQuery, out Uri uri))
        {
            Search(urlOrSearchQuery, addToHistory);
            return;
        }

        // Find a website with a matching hostname
        WebPageAsset? asset = null;
        asset = gameManager.ContentManager.GetContentOfType<WebPageAsset>()
            .FirstOrDefault(x => x.HostName == uri.Host);
			
        // TODO: 404: Code Not Found.
        if (asset == null)
            return;
			
        NavigateTo(asset, uri.PathAndQuery, addToHistory);
    }
    
    private void Search(string searchQuery, bool addToHistory)
    {
        if (searchQuery == homePage)
        {
            GoHome();
            return;
        }

        if (searchQuery == "about:blank")
        {
            if (addToHistory)
            {
                future.Clear();
                history.Push(CurrentUrl);
            }

            page.Content = null;

            current = null;
            return;
        }
    }
    
    private void NavigateTo(WebPageAsset asset, string? path = null, bool pushHistory = false)
    {
        path ??= "/";

        if (pushHistory)
        {
            future.Clear();
            history.Push(CurrentUrl);
        }

        page.Content = null;

        websitePathObserver?.Dispose();
            
        current = asset.InstantiateWebSite(page, path);

        websitePathObserver = current.UrlObservable.Subscribe(OnWebsitePathChanged);
			
        UpdateUI();
    }
    
    private void OnWebsitePathChanged(string url)
    {
        UpdateUI();
    }
    
    private void Navigate()
    {
        NavigateTo(addressBar.Value);
    }
    
    private bool ParseUrl(string text, out Uri uri)
    {
        if (text.StartsWith("web://"))
            return Uri.TryCreate(text, UriKind.Absolute, out uri);
			
        return Uri.TryCreate("web://" + text, UriKind.Absolute, out uri);
    }
}