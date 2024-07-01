using System.Runtime.CompilerServices;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.Widgets;

namespace AcidicGUI;

public sealed class GuiManager
{
    private readonly IGuiContext context;
    private readonly Widget.TopLevelCollection topLevels;
    private readonly GuiRenderer renderer;
    private readonly Queue<Widget> widgetsNeedingLayoutUpdate = new();

    private float screenWidth;
    private float screenHeight;
    private bool isRendering = false;

    public bool IsRendering => isRendering;
    public IOrderedCollection<Widget> TopLevels => topLevels;
    
    public GuiManager(IGuiContext context)
    {
        this.context = context;
        this.topLevels = new Widget.TopLevelCollection(this);
        this.renderer = new GuiRenderer(context);
    }

    public void UpdateLayout()
    {
        var mustRebuildLayout = false;
        var tolerance = 0.001f;

        if (MathF.Abs(screenWidth - context.PhysicalScreenWidget) >= tolerance
            || MathF.Abs(screenHeight - context.PhysicalScreenHeight) >= tolerance)
        {
            screenWidth = context.PhysicalScreenWidget;
            screenHeight = context.PhysicalScreenHeight;

            mustRebuildLayout = true;
        }

        if (mustRebuildLayout)
        {
            foreach (Widget topLevel in topLevels)
            {
                topLevel.InvalidateLayout();
            }
        }

        if (widgetsNeedingLayoutUpdate.Count > 0)
        {
            var layoutRect = new LayoutRect(0, 0, screenWidth, screenHeight);
            
            while (widgetsNeedingLayoutUpdate.TryDequeue(out Widget? widget))
                widget.UpdateLayout(context, layoutRect);
        }
    }

    public void Render()
    {
        isRendering = true;

        foreach (Widget widget in topLevels)
            widget.RenderInternal(renderer);
        
        isRendering = false;
    }

    internal void SubmitForLayoutUpdateInternal(Widget widget)
    {
        widgetsNeedingLayoutUpdate.Enqueue(widget);
    }
}