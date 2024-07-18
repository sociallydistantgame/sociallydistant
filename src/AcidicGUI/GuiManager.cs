using System.Runtime.CompilerServices;
using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AcidicGUI;

public sealed class GuiManager : IFontFamilyProvider
{
    private readonly IGuiContext               context;
    private readonly Widget.TopLevelCollection topLevels;
    private readonly GuiRenderer               renderer;
    private readonly Queue<Widget>             widgetsNeedingLayoutUpdate = new();
    private readonly FallbackVisualStyle       fallbackVisualStyle        = new FallbackVisualStyle();
    private          ButtonState               leftControl;
    private          ButtonState               rightControl;
    private          ButtonState               leftShift;
    private          ButtonState               rightShift;
    private          ButtonState               leftAlt;
    private          ButtonState               rightAlt;
    
    
    private IVisualStyle? visualStyleOverride;
    private int screenWidth;
    private int screenHeight;
    private bool isRendering = false;
    private Widget? hoveredWidget;
    private Widget? widgetBeingDragged;
    private Widget? keyboardFocus;
    private MouseState? previousMouseState;
    private bool reachedFirstUpdate;

    public bool IsRendering => isRendering;
    public IOrderedCollection<Widget> TopLevels => topLevels;
    
    public GuiManager(IGuiContext context, IVisualStyle? globalStyle = null)
    {
        this.context = context;
        this.topLevels = new Widget.TopLevelCollection(this);
        this.renderer = new GuiRenderer(context);

        this.visualStyleOverride = globalStyle;
    }

    public bool IsFocused(Widget widget)
    {
        return widget == keyboardFocus;
    }

    public void SetFocusedWidget(Widget? widgetToFocus)
    {
        if (widgetToFocus == keyboardFocus)
            return;
        
        var e = new FocusEvent(widgetToFocus);
        
        if (keyboardFocus != null)
        {
            Bubble<ILoseFocusHandler, FocusEvent>(keyboardFocus, e, x => x.OnFocusLost);
        }

        keyboardFocus = widgetToFocus;

        if (keyboardFocus != null)
        {
            Bubble<IGainFocusHandler, FocusEvent>(keyboardFocus, e, x => x.OnFocusGained);
        }
    }
    
    public void SendCharacter(Keys key, char character)
    {
        var e = new KeyCharEvent(key, GetKeyModifiers(), character);

        if (hoveredWidget != null)
        {
            Bubble<IPreviewKeyCharHandler, KeyCharEvent>(hoveredWidget, e, x => x.OnPreviewKeyChar);
        }
        
        if (keyboardFocus != null)
        {
            Bubble<IKeyCharHandler, KeyCharEvent>(keyboardFocus, e, x => x.OnKeyChar);
        }
    }

    private ModifierKeys GetKeyModifiers()
    {
        var result = ModifierKeys.None;

        if (leftControl == ButtonState.Pressed || rightControl == ButtonState.Pressed)
            result |= ModifierKeys.Control;
        
        if (leftShift == ButtonState.Pressed || rightShift == ButtonState.Pressed)
            result |= ModifierKeys.Shift;
        
        if (leftAlt == ButtonState.Pressed || leftAlt == ButtonState.Pressed)
            result |= ModifierKeys.Alt;
        
        return result;
    }
    
    public void SendKey(Keys key, ButtonState state)
    {
        switch (key)
        {
            case Keys.LeftControl:
                leftControl = state;
                break;
            case Keys.RightControl:
                rightControl = state;
                break;
            case Keys.LeftShift:
                leftShift = state;
                break;
            case Keys.RightShift:
                rightShift = state;
                break;
            case Keys.LeftAlt:
                leftAlt = state;
                break;
            case Keys.RightAlt:
                rightAlt = state;
                break;
        }
        
        var e = new KeyEvent(key, GetKeyModifiers());
        
        if (hoveredWidget != null)
        {
            if (state == ButtonState.Pressed)
            {
                Bubble<IPreviewKeyDownHandler, KeyEvent>(hoveredWidget, e, x => x.OnPreviewKeyDown);
            }
            else
            {
                Bubble<IPreviewKeyUpHandler, KeyEvent>(hoveredWidget, e, x => x.OnPreviewKeyUp);
            }
        }
        
        if (keyboardFocus != null)
        {
            if (state == ButtonState.Pressed)
            {
                Bubble<IKeyDownHandler, KeyEvent>(keyboardFocus, e, x => x.OnKeyDown);
            }
            else
            {
                Bubble<IKeyUpHandler, KeyEvent>(keyboardFocus, e, x => x.OnKeyUp);
            }
        }
    }
    
    public void SetMouseState(MouseState state)
    {
        if (previousMouseState == null)
            previousMouseState = state;
        else if (previousMouseState.Value == state)
            return;

        HandleMouseEvents(previousMouseState.Value, state);
        previousMouseState = state;
    }
    
    public void UpdateLayout()
    {
        reachedFirstUpdate = true;
        
        var mustRebuildLayout = false;
        var tolerance = 0.001f;

        if (MathF.Abs(screenWidth - context.PhysicalScreenWidth) >= tolerance
            || MathF.Abs(screenHeight - context.PhysicalScreenHeight) >= tolerance)
        {
            screenWidth = context.PhysicalScreenWidth;
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

    public void Update(float deltaTime)
    {
        foreach (Widget widget in this.CollapseChildren())
        {
            if (widget is IUpdateHandler handler)
                handler.Update(deltaTime);
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

        renderer.SetLayer(-32768);
        
        foreach (Widget widget in topLevels)
            widget.RenderInternal(renderer);

        renderer.RenderBatches();
        
        isRendering = false;
    }

    internal void SubmitForLayoutUpdateInternal(Widget widget)
    {
        if (!reachedFirstUpdate)
            return;
        
        if (widget.Parent != null)
        {
            widget.UpdateLayout(context, widget.ContentArea);
        }
        else
        {
            widget.UpdateLayout(context, new LayoutRect(0, 0, screenWidth, screenHeight));
        }
    }

    public IFontFamily GetFont(PresetFontFamily family)
    {
        return GetVisualStyle().GetFont(family);
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

    private Point TranslateMousePosition(Point mousePosition)
    {
        return new Vector2(
            ((float)mousePosition.X / context.GraphicsDevice.PresentationParameters.BackBufferWidth) *
            context.PhysicalScreenWidth,
            ((float)mousePosition.Y / context.GraphicsDevice.PresentationParameters.BackBufferHeight) *
            context.PhysicalScreenHeight
        ).ToPoint();
    }
    
    private void HandleMouseEvents(MouseState previous, MouseState current)
    {
        var prevPosition = TranslateMousePosition(previous.Position);
        var currPosition = TranslateMousePosition(current.Position);

        var prevWheel = previous.ScrollWheelValue;
        var currWheel = current.ScrollWheelValue;

        var positionDelta = currPosition - prevPosition;
        var wheelDelta = -(currWheel - prevWheel);

        HandleMouseMovement(new MouseMoveEvent(currPosition, positionDelta));
        HandleMouseScroll(new MouseScrollEvent(currPosition, wheelDelta));

        HandleMouseButton(MouseButton.Left, previous.LeftButton, current.LeftButton, currPosition, positionDelta);
        HandleMouseButton(MouseButton.Middle, previous.MiddleButton, current.MiddleButton, currPosition, positionDelta);
        HandleMouseButton(MouseButton.Right, previous.RightButton, current.RightButton, currPosition, positionDelta);
    }

    private void HandleMouseButton(MouseButton button, ButtonState previous, ButtonState current, Point position, Point delta)
    {
        var e = new MouseButtonEvent(position, delta, button, current);
        
        if (previous == current)
        {
            if (widgetBeingDragged == null || current == ButtonState.Released)
                return;

            Bubble<IDragHandler, MouseButtonEvent>(widgetBeingDragged, e, x => x.OnDrag);
        }
        else if (previous == ButtonState.Released)
        {
            if (hoveredWidget == null)
                return;
            
            Bubble<IMouseDownHandler, MouseButtonEvent>(hoveredWidget, e, x => x.OnMouseDown);

            if (e.Handled)
                e.Unhandle();

            widgetBeingDragged = hoveredWidget;
            Bubble<IDragStartHandler, MouseButtonEvent>(hoveredWidget, e, x => x.OnDragStart);
        }
        else if (previous == ButtonState.Pressed)
        {
            if (widgetBeingDragged != null)
            {
                Bubble<IDragEndHandler, MouseButtonEvent>(widgetBeingDragged, e, x => x.OnDragEnd);
                widgetBeingDragged = null;
            }
            
            if (hoveredWidget != null)
            {
                Bubble<IMouseClickHandler, MouseButtonEvent>(hoveredWidget, e, x => x.OnMouseClick);
                Bubble<IMouseUpHandler, MouseButtonEvent>(hoveredWidget, e, x => x.OnMouseUp);
            }

            if (!e.FocusWanted && hoveredWidget?.IsFocused == false)
                SetFocusedWidget(null);

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

            if (e.FocusWanted)
            {
                SetFocusedWidget(next);
                break;
            }

            next = next.Parent;
        }
    }
    
    private void HandleMouseMovement(MouseMoveEvent e)
    {
        Widget? newHoveredWidget = null;
        
        foreach (Widget widget in CollapseChildren())
        {
            if (!widget.IsVisible)
                continue;
            
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

    private void HandleMouseScroll(MouseScrollEvent e)
    {
        if (e.ScrollDelta == 0)
            return;
        
        if (hoveredWidget == null)
            return;

        Bubble<IMouseScrollHandler, MouseScrollEvent>(hoveredWidget, e, x => x.OnMouseScroll);
    }
}