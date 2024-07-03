namespace AcidicGUI.TextRendering;

public struct FontInfo
{
    private Font? customFont;
    private FontPreset preset;

    private FontInfo(FontPreset preset, Font? customFont)
    {
        this.preset = preset;
        this.customFont = customFont;
    }
    
    public FontInfo(FontPreset preset) : this(preset, null)
    { }
    
    public FontInfo(Font customFont) : this(FontPreset.Custom, customFont)
    { }

    public static implicit operator FontInfo(Font custom) => new(custom);
    public static implicit operator FontInfo(FontPreset preset) => new(preset);
    
    public Font GetFont(IFontProvider provider)
    {
        if (preset == FontPreset.Custom)
        {
            if (customFont != null)
                return customFont;

            return provider.GetFont(FontPreset.Default);
        }

        return provider.GetFont(preset);
    }
}