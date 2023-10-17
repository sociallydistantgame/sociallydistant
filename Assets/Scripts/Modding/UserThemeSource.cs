#nullable enable
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using ContentManagement;
using UI.Themes.ThemeData;
using UI.Theming;
using UnityEngine;
using Utility;

namespace Modding
{
	public class UserThemeSource : IGameContentSource
	{
		public static readonly string ThemesDirectory = Path.Combine(UnityEngine.Application.persistentDataPath, "themes");
		
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder)
		{
			// We may as well load core themes here since this is registered by the system module
			OperatingSystemTheme[] coreThemes = Resources.LoadAll<OperatingSystemTheme>("Themes");
			foreach (OperatingSystemTheme theme in coreThemes)
			{
				await Task.Yield();
				builder.AddContent(theme);
			}
			
			if (!Directory.Exists(ThemesDirectory))
				Directory.CreateDirectory(ThemesDirectory);

			foreach (string? file in Directory.EnumerateFiles(ThemesDirectory, "*.sdtheme", SearchOption.AllDirectories))
			{
				if (string.IsNullOrWhiteSpace(file))
					continue;

				using ThemeLoader loader = ThemeLoader.FromFile(file, false, false);

				Texture2D? preview = await loader.ExtractPreviewImageAsync();

				ThemeMetadata metadata = await loader.ExtractMetadataAsync();
				
				builder.AddContent(new UserTheme(metadata, preview, file));
			}
		}

		public static string GetNewThemePath(string? themeName)
		{
			if (string.IsNullOrWhiteSpace(themeName))
				themeName = "UserTheme";
            
			char[] invalidChars = Path.GetInvalidFileNameChars();

			foreach (char c in invalidChars)
				themeName = themeName.Replace(c, '_');

			int identifier = 0;

			string fullPath = string.Empty;

			do
			{
				string baseDir = ThemesDirectory;
				string ext = ".sdtheme";

				string filename = (identifier == 0)
					? themeName
					: $"{themeName} ({identifier})";

				fullPath = Path.Combine(baseDir, filename + ext);
				identifier++;
			} while (File.Exists(fullPath));

			return fullPath;
		}
	}
}