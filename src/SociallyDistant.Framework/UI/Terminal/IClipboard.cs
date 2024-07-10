namespace SociallyDistant.Core.UI.Terminal;

public interface IClipboard
{
    string GetText();
    void SetText(string text);
}