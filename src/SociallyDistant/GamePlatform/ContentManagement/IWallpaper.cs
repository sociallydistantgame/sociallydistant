using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GamePlatform.ContentManagement
{
	/// <summary>
	///		Interface for an object that can be used as a shell background.
	/// </summary>
	public interface IWallpaper : IGameContent
	{
		Task<Texture2D?> LoadWallpaperTexture();
	}
}