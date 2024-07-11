namespace SociallyDistant.Core.OS.Tasks;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class CommandAttribute : Attribute
{
    private readonly string name;

    public string Name => name;
		
    public CommandAttribute(string name)
    {
        this.name = name;
    }

}