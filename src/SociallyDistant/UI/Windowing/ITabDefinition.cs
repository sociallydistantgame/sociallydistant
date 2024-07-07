namespace SociallyDistant.UI.Windowing;

public interface ITabDefinition
{
    string Title { get; set; }
    bool Closeable { get; set; }
    bool Active { get; set; }
}