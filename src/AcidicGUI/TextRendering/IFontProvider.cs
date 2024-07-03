namespace AcidicGUI.TextRendering;

public interface IFontProvider
{
    Font GetFont(FontPreset presetFont);
}