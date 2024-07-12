using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.Core.UI.VisualStyles;

public class SociallyDistantVisualStyle : IVisualStyle
{
    private readonly IGameContext game;

    private readonly Color mainBackground                  = new Color(0x11, 0x13, 0x15);
    private readonly Color statusBarColor                  = new Color(0x01, 0x22, 0x37, 0xff);
    private readonly Color accentPrimary                   = new Color(0x13, 0x85, 0xC3, 0xff);
    private readonly Color accentEvil                      = new(0xDE, 0x17, 0x17);
    private readonly Color accentWarning                   = new Color(0xE0, 0x86, 0x17);
    private readonly Color accentSuccess                   = new Color(0x17, 0x82, 0x0E);
    private readonly Color accentCyberspace                = new Color(0x34, 0xB1, 0xFD);
    private readonly Color fieldBackground                 = new Color(0x25, 0x28, 0x2B);
    private readonly Color fieldStroke                     = new Color(0x60, 0x64, 0x67);
    private readonly Color tabInactiveBackgroundDefault    = new Color(0x44, 0x44, 0x44, 191);
    private readonly Color tabInactiveBorderDefault        = new Color(0x44, 0x44, 0x44);
    private readonly Color tabActiveBackgroundDefault      = new Color(0x16, 0x93, 0xD6, 190);
    private readonly Color tabActiveBorderDefault          = new Color(0x16, 0x93, 0xD6);
    private readonly Color sectionTextColor                = new(0x85, 0x85, 0x85);
    private readonly Color inputInactiveBorderColor        = new Color(0x54, 0x57, 0x5A);
    private readonly Color inputInactiveHoveredBorderColor = new Color(0x6F, 0x74, 0x77);
    private readonly Color inputActiveBorderColor          = new Color(0x19, 0xA1, 0xEA);
    private readonly Color inputActiveHoveredBorderColor   = new Color(0x80, 0xC3, 0xFD);
    private readonly Color inputInactiveBackground         = new Color(0x19, 0x1C, 0x1D);
    private readonly Color inputInactiveHoveredBackground  = new Color(0x2C, 0x2F, 0x32);
    private readonly Color inputInactivePressedBackground  = new Color(0x1F, 0x22, 0x25);
    private readonly Color inputActiveBackground           = new Color(0x13, 0x85, 0xC3);
    private readonly Color inputActiveHoveredBackground    = new Color(0x19, 0xA1, 0xEA);
    private readonly Color inputActivePressedBackground    = new Color(0x13, 0x85, 0xC3);

    private Font        iconFont;
    private IFontFamily defaultFont = null!;
    private IFontFamily monospace   = null!;
    private Texture2D?  checkboxEmblem;

    public Vector2 ToggleSize => new Vector2(20, 20);
    public Font? IconFont => iconFont;
    
    public SociallyDistantVisualStyle(IGameContext game)
    {
        this.game = game;
    }

    internal void LoadContent()
    {
        checkboxEmblem = game.GameInstance.Content.Load<Texture2D>("/Core/UI/Textures/checkbox_emblem");
        
        iconFont = Font.FromTtfStream(
            game.GameInstance.Content.Load<Stream>("/Core/UI/Fonts/MaterialIcons-Regular.ttf"),
            256
        );

        defaultFont = LoadFont("/Core/UI/Fonts/Rajdhani");
        monospace = LoadFont("/Core/UI/Fonts/Monospace/JetBrainsMono");
    }

    private IFontFamily LoadFont(string basePath)
    {
        // We MUST load a regular font
        var regularFont = LoadFontStream(basePath + "-Regular.ttf");
        if (regularFont == null)
            throw new Exception(
                $"Could not load Regular variant for font {basePath}. Does {basePath}-Regular.tff exist in the Content File System?");
        
        var family = new FontFamily(regularFont);

        foreach (FontWeight weight in Enum.GetValues<FontWeight>())
        {
            Font? normal = LoadFontStream(basePath + $"-{weight}.ttf");
            Font? italic = LoadFontStream(basePath + $"-{weight}Italic.ttf");
            
            family.SetFont(weight, false, normal);
            family.SetFont(weight, true, italic);
        }

        return family;
    }

    private (float borderThickness, Color borderColor, Color backgroundColor) GetInputColor(
        bool isHovered,
        bool isPressed,
        bool isFocused,
        bool isChecked
    )
    {
        float thickness = isFocused
            ? 2
            : 1;

        Color background = default;
        Color foreground = default;

        if (isChecked)
        {
            foreground = (isHovered || isPressed)
                ? inputActiveHoveredBorderColor
                : inputActiveBorderColor;
            
            if (isHovered && isPressed)
            {
                background = inputActivePressedBackground;
            }
            else if (isHovered)
            {
                background = inputActiveHoveredBackground;
            }
            else
            {
                background = inputActiveBackground;
            }
        }
        else
        {
            foreground = (isHovered || isPressed)
                ? inputInactiveHoveredBorderColor
                : inputInactiveBorderColor;

            if (isHovered && isPressed)
            {
                background = inputInactivePressedBackground;
            }
            else if (isHovered)
            {
                background = inputInactiveHoveredBackground;
            }
            else
            {
                background = inputInactiveBackground;
            }
        }

        return (thickness, foreground, background);
    }
    
    private Font? LoadFontStream(string path)
    {
        try
        {
            using var stream = game.GameInstance.Content.Load<Stream>(path);
            return Font.FromTtfStream(stream, 16);
        }
        catch (Exception ex)
        {
            Log.Warning($"Failed to load GUI font: {path} - {ex.Message}");
            Log.Warning(ex.ToString());
            return null;
        }
    }
    
    public IFontFamily GetFont(PresetFontFamily familyPresetFont)
    {
        if (familyPresetFont == PresetFontFamily.Monospace)
            return monospace;
        
        return defaultFont;
    }

    private Color GetCommonColor(CommonColor commonColor)
    {
        return commonColor switch
        {
            CommonColor.Cyan => accentCyberspace,
            CommonColor.Yellow => accentWarning,
            CommonColor.Red => accentEvil,
            CommonColor.Green => accentSuccess,
            CommonColor.Blue => accentPrimary,
            _ => Color.White
        };
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
        var color = (widget.BoxColor ?? GetCommonColor(widget.GetCustomProperty<CommonColor>()));

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

    public Color GetTextColor(Widget? widget = null)
    {
        if (widget != null)
        {
            switch (widget.GetCustomProperty<WidgetForegrounds>())
            {
                case WidgetForegrounds.Common:
                {
                    var commonColor = widget.GetCustomProperty<CommonColor>();
                    return GetCommonColor(commonColor);
                }
                case WidgetForegrounds.SectionTitle:
                    return sectionTextColor;
            }
        }

        return Color.White;
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
                    geometryHelper.AddQuadOutline(widget.ContentArea, 1,
                        GetCommonColor(widget.GetCustomProperty<CommonColor>()));
                    break;
                case WidgetBackgrounds.FormField:
                    geometryHelper.AddRoundedRectangle(widget.ContentArea, 3, fieldBackground);
                    geometryHelper.AddRoundedRectangleOutline(widget.ContentArea, 1, 3, fieldStroke);
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

    public void DrawToggle(
        Toggle toggle,
        GeometryHelper geometry,
        LayoutRect rect,
        bool isHovered,
        bool isPressed,
        bool isFocused,
        bool isChecked
    )
    {
        (float borderThickness, Color borderColor, Color backgroundColor) = GetInputColor(isHovered, isPressed, isFocused, isChecked);

        geometry.AddRoundedRectangle(rect, 3, backgroundColor);
        geometry.AddRoundedRectangleOutline(rect, borderThickness, 3, borderColor);

        if (isChecked && checkboxEmblem != null)
        {
            var emblemRect = new LayoutRect(rect.Left + ((rect.Width - checkboxEmblem.Width) / 2), rect.Top + ((rect.Height - checkboxEmblem.Height) / 2), checkboxEmblem.Width, checkboxEmblem.Height);
            geometry.AddQuad(emblemRect, Color.White, checkboxEmblem);
        }
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
    Common,
    Overlay,
    StatusBar,
    Dock,
    WindowClient,
    WindowBorder,
    FormField
}

public enum WidgetForegrounds
{
    Default,
    Common,
    SectionTitle
}