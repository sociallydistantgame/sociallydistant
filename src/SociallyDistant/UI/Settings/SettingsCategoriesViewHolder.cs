using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.Settings;

public sealed class SettingsCategoriesViewHolder : ViewHolder
{
    private readonly ListItemWithHeader view = new();

    private SystemSettingsController.SettingsCategoryModel?         model;
    public  Action<SystemSettingsController.SettingsCategoryModel>? ClickCallback;
    
    public SettingsCategoriesViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
        
        view.ClickCallback = HandleClick;
    }
    
    public void SetModel(SystemSettingsController.SettingsCategoryModel model)
    {
        this.model = model;
        view.Value = model.Title;
        this.view.IsActive = model.IsActive;
        view.Title = model.MetaTitle ?? string.Empty;
        view.ShowTitle = model.ShowTitleArea;
    }
    
    private void HandleClick()
    {
        if (model == null)
            return;
        
        ClickCallback?.Invoke(model);
    }
}