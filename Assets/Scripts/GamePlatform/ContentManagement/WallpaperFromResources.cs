using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GamePlatform.ContentManagement
{
	public class WallpaperFromResources : IWallpaper
	{
		private readonly string resourcePath;

		public WallpaperFromResources(string resourcePath)
		{
			this.resourcePath = resourcePath;
		}
		
		/// <inheritdoc />
		public async Task<Texture2D?> LoadWallpaperTexture()
		{
			Object? loadedObject = await Resources.LoadAsync<Texture2D>(this.resourcePath);
			return loadedObject is not Texture2D texture ? null : texture;
		}
	}
}