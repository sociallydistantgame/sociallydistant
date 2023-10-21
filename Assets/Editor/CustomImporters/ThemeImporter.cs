﻿#nullable enable
using System.Collections.Generic;
using System.IO;
using UI.Theming;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor;

namespace Editor.CustomImporters
{
	[ScriptedImporter(1, "sdtheme")]
	public class ThemeImporter : ScriptedImporter
	{
		[SerializeField]
		private ThemeImporterSettings settings = new ThemeImporterSettings();
		
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			using ThemeLoader loader = ThemeLoader.FromFile(ctx.assetPath, false, settings.allowUserCopying);

			// Default behaviour is for the theme loader to store the original file path of the theme in with the
			// theme's data. This allows the in-game theme editor to know where to save changes when opening
			// an existing theme.
			//
			// But in this case, we don't want that - because the path will be referring to somewhere in the game's
			// source assets which won't be available in a release build.
			loader.TrackFilePath = false;

			Texture2D? previewImage = loader.ExtractPreviewImage();

			if (previewImage != null)
			{
				previewImage.name = "PreviewImage";
				ctx.AddObjectToAsset("PreviewImage", previewImage);
			}

			var resourceStorage = new UnityAssetsResourceStorage(ctx, previewImage);

			OperatingSystemTheme theme = loader.LoadTheme(resourceStorage);
			theme.name = Path.GetFileName(ctx.assetPath);
			
			ctx.SetMainObject(theme);
		}
		
		private class UnityAssetsResourceStorage : IThemeResourceStorage
		{
			private readonly AssetImportContext ctx;
			private readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
			private readonly Texture2D? previewImage;

			public UnityAssetsResourceStorage(AssetImportContext ctx, Texture2D? previewImage)
			{
				this.ctx = ctx;
				this.previewImage = previewImage;
			}

			/// <inheritdoc />
			public void SetTheme(OperatingSystemTheme theme)
			{
				theme.SetPreviewImage(previewImage);
				theme.name = Path.GetFileName(ctx.assetPath);
				this.ctx.AddObjectToAsset(theme.name, theme, previewImage);
			}

			/// <inheritdoc />
			public bool TryGetTexture(string textureName, out Texture2D? texture)
			{
				return textures.TryGetValue(textureName, out texture);
			}

			/// <inheritdoc />
			public void AddTexture(string textureName, Texture2D texture)
			{
				textures.Add(textureName, texture);
                ctx.AddObjectToAsset(textureName, texture);
			}
		}
	}
}