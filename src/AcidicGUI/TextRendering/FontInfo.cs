namespace AcidicGUI.TextRendering;

public struct FontInfo
{
    private IFontFamily? customFont;
    private PresetFontFamily family;

    private FontInfo(PresetFontFamily family, IFontFamily? customFont)
    {
        this.family = family;
        this.customFont = customFont;
    }
    
    public FontInfo(PresetFontFamily family) : this(family, null)
    { }
    
    public FontInfo(IFontFamily customFont) : this(PresetFontFamily.Custom, customFont)
    { }

    public static implicit operator FontInfo(FontFamily custom) => new(custom);
    public static implicit operator FontInfo(PresetFontFamily family) => new(family);
    
    public IFontFamily GetFont(IFontFamilyProvider familyProvider)
    {
        if (family == PresetFontFamily.Custom)
        {
            if (customFont != null)
                return customFont;

            return familyProvider.GetFont(PresetFontFamily.Default);
        }

        return familyProvider.GetFont(family);
    }
}