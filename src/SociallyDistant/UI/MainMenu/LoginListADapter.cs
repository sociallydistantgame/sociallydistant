using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.UI.MainMenu;

public sealed class LoginListADapter : ListAdapter<ScrollView, LoginUserViewHolder>
{
    private readonly DataHelper<IGameData> gameData;

    public event Action<IGameData>? OnShowUser;
    
    public LoginListADapter()
    {
        gameData = new DataHelper<IGameData>(this);
    }

    public void SetUsers(IEnumerable<IGameData> source)
    {
        gameData.SetItems(source);
    }
    
    protected override LoginUserViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new LoginUserViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(LoginUserViewHolder viewHolder)
    {
        viewHolder.UpdateView(gameData[viewHolder.ItemIndex]);
        viewHolder.Callback = ShowUser;
    }

    private void ShowUser(IGameData user)
    {
        OnShowUser?.Invoke(user);
    }
}