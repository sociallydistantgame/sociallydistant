using Microsoft.Xna.Framework;
using AcidicGUI.Rendering;

namespace AcidicGUI.Widgets;

public sealed class Box : Widget
{
    private Widget? boxChild;

    public Widget? Content
    {
        get => boxChild;
        set
        {
            if (boxChild != null)
                this.Children.Remove(boxChild);

            boxChild = value;
            
            if (boxChild != null)
                Children.Add(boxChild);
        }
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        geometry.AddRoundedRectangle(ContentArea, 6f, Color.Red);
        geometry.AddRoundedRectangleOutline(ContentArea, 30f,  6f, Color.Cyan);
        
    }
}