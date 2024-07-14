using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.UI.FloatingTools.FileManager;

public sealed class FileGrid : ListAdapter<WrapPanel, FileViewHolder>
{
    private readonly DataHelper<FileIconModel> files;

    public FileGrid()
    {
        Container.Padding = 12;
        Container.SpacingX = 6;
        Container.SpacingY = 6;
        Container.Direction = Direction.Horizontal;
        files = new DataHelper<FileIconModel>(this);
    }

    public void SetFiles(IEnumerable<FileIconModel> source)
    {
        files.SetItems(source);
    }
    
    protected override FileViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new FileViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(FileViewHolder viewHolder)
    {
        viewHolder.UpdateView(files[viewHolder.ItemIndex]);
    }
}