using AcidicGUI.ListAdapters;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Recycling;

namespace SociallyDistant.UI.Common;

public sealed class Embed : IWidget
{
    public CommonColor Color { get; set; }
    public string? Title { get; set; }
    public Dictionary<string, string>? Fields { get; set; }
    public Dictionary<string, Action>? Buttons { get; set; }
    
    public RecyclableWidgetController Build()
    {
        var embedController = new EmbedController();

        embedController.Color = Color;
        embedController.Title = Title;
        embedController.Fields = Fields;
        embedController.Buttons = Buttons;
        
        return embedController;
    }
}