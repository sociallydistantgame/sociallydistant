using System.Threading.Tasks;
using ContentManagement;
using UnityEngine;

namespace GamePlatform.ContentManagement
{
	/// <summary>
	///		Interface for an object that can be used as a shell background.
	/// </summary>
	public interface IWallpaper : IGameContent
	{
		Task<Texture2D?> LoadWallpaperTexture();
	}
}