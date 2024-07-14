using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI.FloatingTools.FileManager;

public class FileIconModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "File";
    public CompositeIcon Icon { get; set; } = MaterialIcons.FileOpen;
    public bool Selected { get; set; }
    
    public Action<int>? DeleteHandler { get; set; }
    public Action<int>? OpenHandler { get; set; }
}