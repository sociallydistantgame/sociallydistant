using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling;

public interface IWidget
{
    RecyclableWidgetController Build();
}