using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.Core.Modules;

public interface IVirtualScreen
{
    void Activate();
    void Deactivate();
    void Blit(RenderTarget2D? target = null);
    void SaveScreenshot(Stream destination);
}