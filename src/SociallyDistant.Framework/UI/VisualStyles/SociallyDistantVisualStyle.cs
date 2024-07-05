using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.UI.VisualStyles;

public class SociallyDistantVisualStyle : IVisualStyle
{
    private readonly IGameContext game;
    private readonly Color statusBarColor = new Color(0x01,0x22, 0x37, 0xff);

    private Font defaultFont = null!; 

    public SociallyDistantVisualStyle(IGameContext game)
    {
        this.game = game;
    }

    internal void LoadContent()
    {
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
    
    public void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper)
    {
        if (widget is InputField inputField)
            DrawInputField(inputField, geometryHelper);
        else
        {
            WidgetBackgrounds background = widget.GetCustomProperty<WidgetBackgrounds>();

            switch (background)
            {
                case WidgetBackgrounds.StatusBar:
                    DrawStatusBar(widget, geometryHelper);
                    break;
            }
        }
    }

    private void DrawStatusBar(Widget widget, GeometryHelper geometry)
    {
        geometry.AddQuad(widget.ContentArea, statusBarColor);
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
    StatusBar
}