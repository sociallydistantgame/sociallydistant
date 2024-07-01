using AcidicGUI.Rendering;
using FontStashSharp;

namespace AcidicGUI.Widgets;

public class Label : Widget
{
    private string text = string.Empty;

    public string Text
    {
        get => text;
        set
        {
            text = value;
            InvalidateLayout();
        }
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
    }
}