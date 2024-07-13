using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Core.Config;
using SociallyDistant.Core.UI.Recycling;
using SociallyDistant.Core.UI.Recycling.SettingsWidgets;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.Settings;

public sealed class SettingsCategoryView : Widget
{
    private readonly FlexPanel                        root            = new();
    private readonly Box                              headerArea      = new();
    private readonly TextWidget                       titleText       = new();
    private readonly TextWidget                       descriptionText = new();
    private readonly StackPanel                       headerStack     = new();
    private readonly RecyclableWidgetList<ScrollView> recyclables     = new();

    public SettingsCategoryView()
    {
        Children.Add(root);

        root.ChildWidgets.Add(headerArea);
        root.ChildWidgets.Add(recyclables);

        headerArea.Content = headerStack;

        headerStack.ChildWidgets.Add(titleText);
        headerStack.ChildWidgets.Add(descriptionText);

        recyclables.Container.Spacing = 3;

        recyclables.MinimumSize = new Vector2(600, 460);
        recyclables.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;

        recyclables.Padding = 6;

        this.titleText.FontWeight = FontWeight.Bold;
        this.titleText.FontSize = 20;
        titleText.SetCustomProperty(WidgetForegrounds.Common);
    }
    
    public void SetData(SystemSettingsController.SettingsCategoryModel model)
    {
        headerArea.Visibility = Visibility.Visible;
        titleText.Text = model.Title;
        
        if (model.Category != null)
        {
            recyclables.Visibility = Visibility.Visible;
            RebuildWidgets(model.Category);
        }
        else
        {
            recyclables.Visibility = Visibility.Collapsed;
        }
    }

    private void RebuildWidgets(SettingsCategory category)
    {
        var builder = new WidgetBuilder();
        var uiBuilder = new WidgetListSettingsUiBuilder(builder);
        
        builder.Begin();

        category.BuildSettingsUi(uiBuilder);

        this.recyclables.SetWidgets(builder.Build());
    }
}