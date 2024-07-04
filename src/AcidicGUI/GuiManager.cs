using System.Runtime.CompilerServices;
using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI;

public sealed class GuiManager : IFontProvider
{
    private readonly IGuiContext context;
    private readonly Widget.TopLevelCollection topLevels;
    private readonly GuiRenderer renderer;
    private readonly Queue<Widget> widgetsNeedingLayoutUpdate = new();
    private readonly FallbackVisualStyle fallbackVisualStyle = new FallbackVisualStyle();

    private Vector2 previousMousePosition = new Vector2(-1, -1);
    private IVisualStyle? visualStyleOverride;
    private float screenWidth;
    private float screenHeight;
    private bool isRendering = false;
    private Widget? hoveredWidget;

    public bool IsRendering => isRendering;
    public IOrderedCollection<Widget> TopLevels => topLevels;
    
    public GuiManager(IGuiContext context)
    {
        this.context = context;
        this.topLevels = new Widget.TopLevelCollection(this);
        this.renderer = new GuiRenderer(context);
    }

    public void SetMousePosition(Point newPosition)
    {
        // Translate to UI space before we do anything
        Vector2 uiMousePos = new Vector2(
            ((float)newPosition.X / context.GraphicsDevice.PresentationParameters.BackBufferWidth) *
            context.PhysicalScreenWidget,
            ((float)newPosition.Y / context.GraphicsDevice.PresentationParameters.BackBufferHeight) *
            context.PhysicalScreenHeight
        );

        Vector2 delta = uiMousePos - previousMousePosition;
        previousMousePosition = uiMousePos;

        var moveEvent = new MouseMoveEvent(uiMousePos, delta);

        HandleMouseMovement(moveEvent);
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

    public IVisualStyle GetVisualStyle()
    {
        if (fallbackVisualStyle.FallbackFont == null)
            fallbackVisualStyle.FallbackFont = this.context.GetFallbackFont();
        
        return visualStyleOverride ?? fallbackVisualStyle;
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

    public Font GetFont(FontPreset presetFont)
    {
        return GetVisualStyle().GetFont(presetFont);
    }

    internal GraphicsDevice GetGraphicsDeviceInternal()
    {
        return context.GraphicsDevice;
    }

    private IEnumerable<Widget> CollapseChildren()
    {
        for (var i = topLevels.Count - 1; i >= 0; i--)
        {
            Widget toplevel = topLevels[i];
            if (!toplevel.Enabled)
                continue;
            
            foreach (Widget child in toplevel.CollapseChildren(true, false))
            {
                yield return child;
            }
            
            yield return toplevel;
        }
    }

    private void Bubble<THandler, TEvent>(Widget widget, TEvent e, Func<THandler, Action<TEvent>> getHandler)
        where TEvent : GuiEvent
    {
        Widget? next = widget;

        while (next != null && !e.Handled)
        {
            if (next is THandler handler)
            {
                getHandler(handler)(e);
            }

            next = next.Parent;
        }
    }
    
    private void HandleMouseMovement(MouseMoveEvent e)
    {
        Widget? newHoveredWidget = null;
        
        foreach (Widget widget in CollapseChildren())
        {
            if (widget is not IMouseHandler)
                continue;

            if (widget.ClippedContentArea.Contains(e.Position))
            {
                newHoveredWidget = widget;
                break;
            }
        }

        if (newHoveredWidget != hoveredWidget)
        {
            if (hoveredWidget != null && !hoveredWidget.ContainsChild(newHoveredWidget))
            {
                Bubble<IMouseLeaveHandler, MouseMoveEvent>(hoveredWidget, e, x => x.OnMouseLeave);
            }

            hoveredWidget = newHoveredWidget;

            if (hoveredWidget != null)
            {
                Bubble<IMouseEnterHandler, MouseMoveEvent>(hoveredWidget, e, x => x.OnMouseEnter);
            }
        }

        if (hoveredWidget != null)
        {
            Bubble<IMouseMoveHandler, MouseMoveEvent>(hoveredWidget, e, x => x.OnMouseMove);
        }
    }
}