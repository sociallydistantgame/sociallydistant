using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.Core.UI.VisualStyles;

public class SociallyDistantVisualStyle : IVisualStyle
{
    private readonly IGameContext game;
    private readonly Color mainBackground = new Color(0x11, 0x13, 0x15);
    private readonly Color statusBarColor = new Color(0x01,0x22, 0x37, 0xff);
    private readonly Color accentPrimary = new Color(0x13, 0x85, 0xC3, 0xff);

    // Tabs - Inactive
    private readonly Color tabInactiveBackgroundDefault = new Color(0x44, 0x44, 0x44, 191);
    private readonly Color tabInactiveBorderDefault = new Color(0x44, 0x44, 0x44);
    
    // Tabs - Active
    private readonly Color tabActiveBackgroundDefault = new Color(0x16, 0x93, 0xD6, 190);
    private readonly Color tabActiveBorderDefault = new Color(0x16, 0x93, 0xD6);
    
    private Font iconFont;
    private Font defaultFont = null!;

    public Font? IconFont => iconFont;
    
    public SociallyDistantVisualStyle(IGameContext game)
    {
        this.game = game;
    }

    internal void LoadContent()
    {
        iconFont = Font.FromTtfStream(
            game.GameInstance.Content.Load<Stream>("/Core/UI/Fonts/MaterialIcons-Regular.ttf"),
            256
        );
        
        defaultFont = game.GameInstance.Content.Load<SpriteFont>("/Core/UI/Fonts/Rajdhani");
    }
    
    public Font GetFont(FontPreset presetFont)
    {
        return defaultFont;
    }

    public float ScrollBarSize => 18;

    private void DrawInputField(InputField inputField, GeometryHelper geometryHelper)
    {
        var style = inputField.GetCustomProperty<InputFieldStyle>();

        var color = Color.White;

        switch (style)
        {
            case InputFieldStyle.Default:
            {
                var thickness = 1f;
                
                geometryHelper.AddQuad(
                    new LayoutRect(
                        inputField.ContentArea.Left,
                        inputField.ContentArea.Bottom - thickness,
                        inputField.ContentArea.Width,
                        thickness
                    ), color
                );
                break;
            }
            case InputFieldStyle.Box:
            {
                var radius = 3;
                var thickness = 1;
                geometryHelper.AddRoundedRectangleOutline(
                    inputField.ContentArea,
                    thickness,
                    radius,
                    color
                );
                
                break;
            }
        }
    }

    private void DrawWindowTab(Widget widget, IWindowTab tab, GeometryHelper geometry)
    {
        var thickness = 1;
        var background = tabInactiveBackgroundDefault;
        var border = tabInactiveBorderDefault;

        if (tab.Active)
        {
            background = tabActiveBackgroundDefault;
            border = tabActiveBorderDefault;
        }

        geometry.AddRoundedRectangle(
            widget.ContentArea,
            3,
            3,
            0,
            0,
            background
        );
        
        geometry.AddRoundedRectangleOutline(
            widget.ContentArea,
            thickness,
            3,
            3,
            0,
            0,
            border
        );
    }

    private void DrawDecorativeBlock(DecorativeBlock widget, GeometryHelper geometry)
    {
        var color = (widget.BoxColor ?? accentPrimary);

        if (widget.Opaque)
        {
            geometry.AddRoundedRectangle(
                widget.ContentArea,
                3,
                color
            );
        }
        else
        {
            geometry.AddRoundedRectangle(
                widget.ContentArea,
                3,
                color * 0.5f
            );

            geometry.AddRoundedRectangleOutline(
                widget.ContentArea,
                2,
                3,
                color
            );


        }
    }
    
    public void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper)
    {
        if (widget is InputField inputField)
            DrawInputField(inputField, geometryHelper);
        else if (widget is IWindowTab tab)
            DrawWindowTab(widget, tab, geometryHelper);
        else if (widget is DecorativeBlock box)
            DrawDecorativeBlock(box, geometryHelper);
        else
        {
            WidgetBackgrounds background = widget.GetCustomProperty<WidgetBackgrounds>();

            switch (background)
            {
                case WidgetBackgrounds.StatusBar:
                    DrawStatusBar(widget, geometryHelper);
                    break;
                case WidgetBackgrounds.Overlay:
                    geometryHelper.AddQuad(widget.ContentArea, mainBackground * 0.67f);
                    break;
                case WidgetBackgrounds.Dock:
                    DrawDock(widget, geometryHelper);
                    break;
                case WidgetBackgrounds.WindowClient:
                    geometryHelper.AddQuad(widget.ContentArea, mainBackground);
                    break;
                case WidgetBackgrounds.WindowBorder:
                    geometryHelper.AddQuadOutline(widget.ContentArea, 1, accentPrimary);
                    break;
            }
        }
    }

    private void DrawStatusBar(Widget widget, GeometryHelper geometry)
    {
        geometry.AddQuad(widget.ContentArea, statusBarColor);
    }

    private void DrawDock(Widget widget, GeometryHelper geometry)
    {
        geometry.AddRoundedRectangle(widget.ContentArea, 3, mainBackground);
    }
    
    public void DrawScrollBar(Widget widget, GeometryHelper geometry, LayoutRect scrollBarArea, float scrollOffset,
        float scrollViewHeight)
    {
        float barHeight = scrollBarArea.Height / scrollViewHeight * scrollBarArea.Height;
        float barOffset = (scrollOffset / scrollViewHeight) * scrollBarArea.Height;

        geometry.AddQuad(scrollBarArea, Color.Gray);
        
        geometry.AddQuad(
            new LayoutRect(
                scrollBarArea.Left,
                scrollBarArea.Top + barOffset,
                scrollBarArea.Width,
                barHeight
            ),
            Color.White
        );
    }
}

public enum InputFieldStyle
{
    Default,
    Box
}

public enum WidgetBackgrounds
{
    None,
    Overlay,
    StatusBar,
    Dock,
    WindowClient,
    WindowBorder,
}