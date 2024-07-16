using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.Common;

public sealed class InfoBox : Widget
{
    private readonly FlexPanel       root            = new();
    private readonly DecorativeBlock decorativeBlock = new();
    private readonly StackPanel      contentStack    = new();
    private readonly TextWidget      title           = new();
    private readonly Box             contentBox      = new();

    public bool UseOpaqueBlock
    {
        get => decorativeBlock.Opaque;
        set => decorativeBlock.Opaque = value;
    }
    
    public CommonColor Color
    {
        get => decorativeBlock.GetCustomProperty<CommonColor>();
        set
        {
            decorativeBlock.SetCustomProperty(value);
            title.SetCustomProperty(value);
        }
    }
    
    public string TitleText
    {
        get => title.Text;
        set => title.Text = value;
    }
    
    public Widget? Content
    {
        get => contentBox.Content;
        set => contentBox.Content = value;
    }

    public InfoBox()
    {
        Children.Add(root);
        root.ChildWidgets.Add(decorativeBlock);
        root.ChildWidgets.Add(contentStack);
        contentStack.ChildWidgets.Add(title);
        contentStack.ChildWidgets.Add(contentBox);

        contentBox.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        
        title.WordWrapping = true;
        title.UseMarkup = false;
        title.MaximumSize = new Point(420, 0);
        title.FontSize = 20;
        title.FontWeight = FontWeight.Bold;
        title.SetCustomProperty(WidgetForegrounds.Common);
        
        root.Padding = 3;
        root.Spacing = 3;
        root.Direction = Direction.Horizontal;

        contentStack.Spacing = 3;
        contentStack.Padding = new Padding(0, 3);
        contentStack.Direction = Direction.Vertical;

        decorativeBlock.MinimumSize = new Point(18, 0);
    }
}