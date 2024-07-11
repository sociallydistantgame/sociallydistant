namespace SociallyDistant.Core.OS.Devices;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class NetworkServiceAttribute : Attribute
{
    private readonly string id;

    public string Id => id;
		
    public NetworkServiceAttribute(string id)
    {
        this.id = id;
    }
}