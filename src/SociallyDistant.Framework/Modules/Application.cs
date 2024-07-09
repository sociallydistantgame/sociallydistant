namespace SociallyDistant.Core.Modules;

public abstract class Application
{
    private static Application? current;

    public abstract IGameContext Context { get; }
		
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