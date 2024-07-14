using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.InfoPanel;
using SociallyDistant.Core.UI.Effects;
using SociallyDistant.Core.UI.VisualStyles;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI.InfoWidgets;

public sealed class InfoWidgetView : Widget
{
    private readonly Box                 root        = new();
    private readonly OverlayWidget       overlay     = new();
    private readonly FlexPanel           dataRoot    = new();
    private readonly CompositeIconWidget icon        = new();
    private readonly StackPanel          textRoot    = new();
    private readonly TextWidget          title       = new();
    private readonly TextWidget          description = new();
    private readonly Button              closeButton = new();
    private readonly CompositeIconWidget closeIcon   = new();
    private          int                 itemId;
    public           Action<int>?        ItemClosed;

    public InfoWidgetView()
    {
        root.SetCustomProperty(WidgetBackgrounds.Overlay);

        icon.IconSize = 16;
        icon.VerticalAlignment = VerticalAlignment.Top;
        title.FontWeight = FontWeight.SemiBold;
        dataRoot.Padding = 6;
        dataRoot.Direction = Direction.Horizontal;
        dataRoot.Spacing = 3;
        closeButton.VerticalAlignment = VerticalAlignment.Top;
        closeButton.HorizontalAlignment = HorizontalAlignment.Right;
        closeIcon.IconSize = 12;
        closeIcon.Icon = MaterialIcons.Close;

        textRoot.Direction = Direction.Vertical;
        textRoot.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        
        Children.Add(root);
        root.Content = overlay;
        overlay.ChildWidgets.Add(dataRoot);
        dataRoot.ChildWidgets.Add(icon);
        dataRoot.ChildWidgets.Add(textRoot);
        textRoot.ChildWidgets.Add(title);
        textRoot.ChildWidgets.Add(description);
        overlay.ChildWidgets.Add(closeButton);
        closeButton.Content = closeIcon;
        
        closeButton.Clicked += OnClicked;
    }
    
    public void UpdateView(InfoWidgetData data)
    {
        itemId = data.Id;

        closeButton.Visibility = data.CreationData.Closeable
            ? Visibility.Visible
            : Visibility.Collapsed;
        
        icon.Icon = data.CreationData.Icon;
        title.Text = data.CreationData.Title;
        description.Text = data.CreationData.Text;
    }
    
    private void OnClicked()
    {
        ItemClosed?.Invoke(itemId);
    }
}