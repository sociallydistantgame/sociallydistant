#nullable enable

using System.Threading.Tasks;

namespace GamePlatform.ContentManagement
{
	public class RestitchedContentSource : IGameContentSource
	{
		/// <inheritdoc />
		public Task LoadAllContent(ContentCollectionBuilder builder)
		{
			// Wallpapers
			builder.AddContent(new WallpaperFromResources("RestitchedContent/ShellBackgrounds/RestitchedBlueprint"));
			builder.AddContent(new WallpaperFromResources("RestitchedContent/ShellBackgrounds/BlueprintProps"));
			builder.AddContent(new WallpaperFromResources("RestitchedContent/ShellBackgrounds/BlueprintPinkPaper"));
			
			// Gonna need Halston to approve more
			return Task.CompletedTask;
		}
	}
}