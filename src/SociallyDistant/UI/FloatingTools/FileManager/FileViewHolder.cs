using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.FloatingTools.FileManager;

public sealed class FileViewHolder : ViewHolder
{
    private readonly FileIcon view = new(); 
    
    public FileViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(FileIconModel model)
    {
        view.UpdateView(model);
    }
}