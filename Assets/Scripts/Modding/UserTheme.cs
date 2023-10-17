#nullable enable
using System.Threading.Tasks;
using UI.Themes.ThemeData;
using UI.Theming;
using UnityEngine;

namespace Modding
{
	public class UserTheme : IThemeAsset
	{
		private readonly ThemeMetadata metadata;
		private readonly Texture2D? preview;
		private readonly string path;

		public UserTheme(ThemeMetadata metadata, Texture2D? preview, string path)
		{
			this.metadata = metadata;
			this.preview = preview;
			this.path = path;
		}

		/// <inheritdoc />
		public bool CanEdit => true;

		/// <inheritdoc />
		public bool CanCopy => true;

		/// <inheritdoc />
		public string Name => metadata.Name;

		/// <inheritdoc />
		public string Author => metadata.LocalAuthorName;

		/// <inheritdoc />
		public string Description => metadata.Description;

		/// <inheritdoc />
		public Texture2D? PreviewImage => preview;

		/// <inheritdoc />
		public async Task<OperatingSystemTheme> LoadAsync()
		{
			using ThemeLoader loader = ThemeLoader.FromFile(path, CanEdit, CanCopy);

			var storage = new BasicThemeResourceStorage();
			return await loader.LoadThemeAsync(storage);
		}
	}
}