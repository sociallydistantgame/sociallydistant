using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.Modules;

public static class XnaHelpers
{
    public static void MustGetComponent<T>(this GameComponent context, out T requiredComponent) 
    {
        requiredComponent = MustGetComponent<T>(context);
    }
    
    public static void MustGetComponent<T>(this Game context, out T requiredComponent) 
    {
        requiredComponent = MustGetComponent<T>(context);
    }
    
    public static T MustGetComponent<T>(this GameComponent component)
    {
        return MustGetComponent<T>(component.Game);
    }
		
		
		
    public static T MustGetComponent<T>(this Game game)
    {
        T? component = game.Components.OfType<T>().FirstOrDefault();
        if (component == null)
            throw new Exception(
                $"Component of type {typeof(T).FullName} not found in the game's component list. Are you missing a script mod dependency?");

        return component;
    }
}