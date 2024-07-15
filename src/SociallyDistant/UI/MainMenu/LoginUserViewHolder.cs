using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.UI.MainMenu;

public sealed class LoginUserViewHolder : ViewHolder
{
    private readonly LoginUserView view = new();

    public Action<IGameData>? Callback
    {
        get => view.Callback;
        set => view.Callback = value;
    }
    
    public LoginUserViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(IGameData user)
    {
        view.UpdateView(user);
    }
}