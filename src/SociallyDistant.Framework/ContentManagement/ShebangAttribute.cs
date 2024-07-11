namespace SociallyDistant.Core.ContentManagement;

public sealed class ShebangAttribute : Attribute
{
    private readonly string shebang;

    public string Shebang => shebang;

    public ShebangAttribute(string shebang)
    {
        this.shebang = shebang;
    }
}