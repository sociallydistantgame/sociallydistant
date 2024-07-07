namespace AcidicGUI.TextRendering;

public interface IFontFamilyProvider
{
    IFontFamily GetFont(PresetFontFamily family);
}