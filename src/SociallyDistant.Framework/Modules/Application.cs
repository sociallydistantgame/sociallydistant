namespace SociallyDistant.Core.Modules;

public abstract class Application
{
    private static Application? current;

    public abstract IGameContext Context { get; }

    public string Name => "Socially Distant";
    public string Version => "24.07-indev";
    public string EngineVersion => "24.07";
    
    public Application()
    {
        if (current != null)
            throw new InvalidOperationException("Cannot create more than one instance of Application.");

        current = this;
    }

    public static Application Instance
    {
        get
        {
            if (current == null)
                throw new InvalidOperationException("Socially Distant is not running.");
            return current;
        }
    }
}