using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public sealed class CanvasPanel : ContainerWidget
{
    public CanvasPanel()
    {
        LayoutRoot = this;
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        // Canvases do not change their size based on content. It's up to the parent to give
        // us our size.
        return availableSize;
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        foreach (Widget child in Children)
        {
            var anchors = child.GetCustomProperties<CanvasAnchors>();
            var childSize = child.GetCachedContentSize(availableSpace.Size);

            var pivotOffset = childSize * anchors.Pivot;
            var pivotedPosition = anchors.AnchoredPosition - pivotOffset;
            
            // TODO:  Anchors within the canvas bounds.
            child.UpdateLayout(context, new LayoutRect(availableSpace.Left + pivotedPosition.X, availableSpace.Top + pivotedPosition.Y, childSize.X, childSize.Y));
        }
    }
}