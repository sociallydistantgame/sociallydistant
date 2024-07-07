using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Core.Config;
using SociallyDistant.Core.UI.Recycling;
using SociallyDistant.Core.UI.Recycling.SettingsWidgets;

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
    }
    
    public void SetData(SystemSettingsController.SettingsCategoryModel model)
    {
        if (model.ShowTitleArea)
        {
            headerArea.Visibility = Visibility.Visible;
            titleText.Text = model.Title;
        }
        else
        {
            headerArea.Visibility = Visibility.Collapsed;
        }

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