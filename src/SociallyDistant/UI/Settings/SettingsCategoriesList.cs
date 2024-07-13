using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.Settings;

public sealed class SettingsCategoriesList : ListAdapter<ScrollView, SettingsCategoriesViewHolder>
{
    private readonly DataHelper<SystemSettingsController.SettingsCategoryModel> categories;

    public event Action<SystemSettingsController.SettingsCategoryModel>? OnItemClicked;
    
    public SettingsCategoriesList()
    {
        categories = new DataHelper<SystemSettingsController.SettingsCategoryModel>(this);
    }

    public void SetItems(IReadOnlyList<SystemSettingsController.SettingsCategoryModel> models)
    {
        categories.SetItems(models);
    }

    protected override SettingsCategoriesViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new SettingsCategoriesViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(SettingsCategoriesViewHolder viewHolder)
    {
        var model = categories[viewHolder.ItemIndex];
        viewHolder.SetModel(model);
        
        viewHolder.ClickCallback = OnClick;
    }

    private void OnClick(SystemSettingsController.SettingsCategoryModel model)
    {
        this.OnItemClicked?.Invoke(model);
    }
}