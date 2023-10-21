#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI.Themes.Serialization;
using UI.Themes.ThemeData;
using UnityEngine;
using UnityEngine.Networking;

namespace UI.Theming
{
	public class ThemeLoader : IDisposable
	{
		private readonly Stream stream;
		private readonly ZipArchive archive;
		private readonly bool canUserEdit;
		private readonly bool canUserCopy;
		private string? filePath;

		public bool TrackFilePath { get; set; } = true;
		
		private ThemeLoader(Stream stream, bool canEdit, bool canCopy)
		{
			this.stream = stream;
			this.archive = new ZipArchive(stream, ZipArchiveMode.Read);
			this.canUserEdit = canEdit;
			this.canUserCopy = canCopy;
		}

		public OperatingSystemTheme LoadTheme(IThemeResourceStorage storage)
		{
			OperatingSystemTheme.ThemeEditor themeEditor = OperatingSystemTheme.CreateEmpty(canUserEdit, canUserCopy);

			if (TrackFilePath)
				themeEditor.FilePath = filePath;
			
			var assets = new LoadThemeAssets(this, storage);
			
			LoadMetadata(themeEditor);
			LoadColors(themeEditor);
			LoadBackdrop(themeEditor, assets);
			LoadWidgetStyle(themeEditor, assets);
			LoadWindowStyle(themeEditor, assets);
			LoadShellStyle(themeEditor, assets);
			LoadTerminalStyle(themeEditor, assets);
			
			storage.SetTheme(themeEditor.Theme);
			
			return themeEditor.Theme;
		}
		
		public async Task<OperatingSystemTheme> LoadThemeAsync(IThemeResourceStorage storage)
		{
			OperatingSystemTheme.ThemeEditor themeEditor = OperatingSystemTheme.CreateEmpty(canUserEdit, canUserCopy);

			if (TrackFilePath)
				themeEditor.FilePath = filePath;
			
			var assets = new LoadThemeAssetsAsync(this, storage);
			
			// This has to block, because Unity is a stupid game engine that
			// *requires* HTML colors to be parsed on the main thread. :)
			// ...Fuck my life.
			LoadColors(themeEditor);
			
			// The XML serialization is done on a background thread and we wait for it.
			// This is because LoadThemeAssetsAsync will not immediately load assets,
			// we load them all at once after batching asset requests.
			await Task.Run(() =>
			{
				LoadMetadata(themeEditor);
				LoadBackdrop(themeEditor, assets);
				LoadWidgetStyle(themeEditor, assets);
				LoadShellStyle(themeEditor, assets);
				LoadTerminalStyle(themeEditor, assets);
				LoadWindowStyle(themeEditor, assets);
			});
			
			// Load all assets!
			await assets.Load();
			
			storage.SetTheme(themeEditor.Theme);

			return themeEditor.Theme;
		}

		private void LoadBackdrop(OperatingSystemTheme.ThemeEditor editor, ThemeAssets assets)
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.BackdropName);
			if (entry == null)
			{
				editor.Name = "Unnamed theme";
				return;
			}

			using Stream entryStream = entry.Open();

			var serializer = XmlThemeDataSerializer.CreateFromStream(entryStream, nameof(BackdropStyle));

			editor.BackdropStyle.Serialize(serializer, assets);
		}
		
		private void LoadShellStyle(OperatingSystemTheme.ThemeEditor editor, ThemeAssets assets)
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.ShellName);
			if (entry == null)
			{
				editor.Name = "Unnamed theme";
				return;
			}

			using Stream entryStream = entry.Open();

			var serializer = XmlThemeDataSerializer.CreateFromStream(entryStream, nameof(ShellStyle));

			editor.ShellStyle.Serialize(serializer, assets);
		}
		
		private void LoadTerminalStyle(OperatingSystemTheme.ThemeEditor editor, ThemeAssets assets)
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.TerminalName);
			if (entry == null)
			{
				editor.Name = "Unnamed theme";
				return;
			}

			using Stream entryStream = entry.Open();

			var serializer = XmlThemeDataSerializer.CreateFromStream(entryStream, nameof(TerminalStyle));

			editor.TerminalStyle.Serialize(serializer, assets);
		}
		
		private void LoadWidgetStyle(OperatingSystemTheme.ThemeEditor editor, ThemeAssets assets)
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.WidgetDataName);
			if (entry == null)
			{
				editor.Name = "Unnamed theme";
				return;
			}

			using Stream entryStream = entry.Open();

			var serializer = XmlThemeDataSerializer.CreateFromStream(entryStream, nameof(WidgetStyle));

			editor.WidgetStyle.Serialize(serializer, assets);
		}
		
		private void LoadWindowStyle(OperatingSystemTheme.ThemeEditor editor, ThemeAssets assets)
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.WindowStyle);
			if (entry == null)
			{
				editor.Name = "Unnamed theme";
				return;
			}

			using Stream entryStream = entry.Open();

			var serializer = XmlThemeDataSerializer.CreateFromStream(entryStream, nameof(WindowStyle));

			editor.WindowStyle.Serialize(serializer, assets);
		}
		
		private void LoadMetadata(OperatingSystemTheme.ThemeEditor editor)
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.MetadataName);
			if (entry == null)
			{
				editor.Name = "Unnamed theme";
				return;
			}

			using Stream entryStream = entry.Open();

			var serializer = XmlThemeDataSerializer.CreateFromStream(entryStream, nameof(ThemeMetadata));

			string name = string.Empty;
			string localAuthor = string.Empty;
			string description = string.Empty;
			
			serializer.Serialize(ref name, nameof(editor.Name), "Unnamed theme");
			serializer.Serialize(ref localAuthor, nameof(editor.LocalAuthor), string.Empty);
			serializer.Serialize(ref description, nameof(editor.Description), string.Empty);

			editor.Name = name;
			editor.LocalAuthor = localAuthor;
			editor.Description = description;
		}
		
		private void LoadColors(OperatingSystemTheme.ThemeEditor editor)
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.ColorTableName);
			if (entry == null)
				return;

			editor.Colors.Clear();

			using Stream entryStream = entry.Open();

			var serializer = XmlThemeDataSerializer.CreateFromStream(entryStream, nameof(NamedColorTable));

			IElementIterator colors = serializer.GetChildCollection(nameof(editor.Colors), nameof(NamedColor));

			foreach (IElementSerializer? item in colors.ChildElements)
			{
				string name = string.Empty;
				string darkHex = string.Empty;
				string lightHex = string.Empty;

				item.Serialize(ref name, nameof(name), string.Empty);

				if (string.IsNullOrWhiteSpace(name))
					continue;

				item.Serialize(ref lightHex, nameof(lightHex), string.Empty);
				item.Serialize(ref darkHex, nameof(darkHex), lightHex);

				if (!lightHex.StartsWith("#"))
					lightHex = "#" + lightHex;

				if (!darkHex.StartsWith("#"))
					darkHex = "#" + darkHex;
                
				if (!ColorUtility.TryParseHtmlString(lightHex, out Color lightColor))
					continue;

				if (!ColorUtility.TryParseHtmlString(darkHex, out Color darkColor))
					darkColor = lightColor;

				editor.Colors[name] = new NamedColor
				{
					darkColor = darkColor,
					lightColor = lightColor
				};
			}
		}

		public Texture2D? ExtractPreviewImage()
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.PreviewImageName);
			if (entry == null)
				return null;

			return ExtractTextureResource(entry);
		}

		public async Task<ThemeMetadata> ExtractMetadataAsync()
		{
			OperatingSystemTheme.ThemeEditor editor = OperatingSystemTheme.CreateEmpty(false, false);

			await Task.Run(() =>
			{
				LoadMetadata(editor);
			});

			string name = editor.Name;
			string author = editor.LocalAuthor;
			string description = editor.Description;
			
			UnityEngine.Object.Destroy(editor.Theme);

			return new ThemeMetadata()
			{
				Name = name,
				LocalAuthorName = author,
				Description = description
			};
		}
		
		public async Task<Texture2D?> ExtractPreviewImageAsync()
		{
			ZipArchiveEntry? entry = archive.GetEntry(ThemeFileConstants.PreviewImageName);
			if (entry == null)
				return null;

			return await ExtractTextureResourceAsync(entry);
		}
		
		/// <inheritdoc />
		public void Dispose()
		{
			archive.Dispose();
			stream.Close();
			stream.Dispose();
		}
		
		public static ThemeLoader FromFile(string path, bool canUserEdit, bool canUserCopy)
		{
			ThemeLoader result =  FromStream(File.OpenRead(path), canUserEdit, canUserCopy);
			result.filePath = path;
			return result;
		}

		public static ThemeLoader FromStream(Stream stream, bool canUserEdit, bool canUserCopy)
		{
			return new ThemeLoader(stream, canUserEdit, canUserCopy);
		}

		private Texture2D? GetNamedTexture(IThemeResourceStorage storage, string textureName)
		{
			char[] invalidChars = Path.GetInvalidFileNameChars();
			if (invalidChars.Any(x => textureName.Contains((char)x)))
			{
				Debug.LogError($"Invalid named texture asset in theme file: {textureName} contains illegal filename chars. You gon' do some time fo' this crime.");
				return null;
			}

			if (storage.TryGetTexture(textureName, out Texture2D? texture))
				return Texture2D.blackTexture;

			ZipArchiveEntry? assetEntry = archive.GetEntry($"{ThemeFileConstants.AssetsDirectoryName}{textureName}");
			if (assetEntry == null)
				return null;

			texture = ExtractTextureResource(assetEntry);

			if (texture == null)
				return null;
			
			storage.AddTexture(textureName, texture);
			return texture;
		}
		
		private async Task<Texture2D?> GetNamedTextureAsync(IThemeResourceStorage storage, string textureName)
		{
			char[] invalidChars = Path.GetInvalidFileNameChars();
			if (invalidChars.Any(x => textureName.Contains((char)x)))
			{
				Debug.LogError($"Invalid named texture asset in theme file: {textureName} contains illegal filename chars. You gon' do some time fo' this crime.");
				return null;
			}

			if (storage.TryGetTexture(textureName, out Texture2D? texture))
				return Texture2D.blackTexture;

			ZipArchiveEntry? assetEntry = archive.GetEntry($"{ThemeFileConstants.AssetsDirectoryName}{textureName}");
			if (assetEntry == null)
				return null;

			texture = await ExtractTextureResourceAsync(assetEntry);

			if (texture == null)
				return null;

			Debug.Log($"Successfully loaded theme texture: {textureName}");
			storage.AddTexture(textureName, texture);
			return texture;
		}

		private Texture2D? ExtractTextureResource(ZipArchiveEntry entry)
		{
			// Open the image entry
			using Stream entryStream = entry.Open();

			// Place in memory to extract the texture into
			using var memory = new MemoryStream();
			
			// Extract it!
			entryStream.CopyTo(memory);
			memory.Seek(0, SeekOrigin.Begin);
			
			// Create the actual texture
			byte[] textureData = memory.ToArray();
			var texture = new Texture2D(1, 1);
			texture.LoadImage(textureData, false);
            
			return texture;
		}
		
		private async Task<Texture2D?> ExtractTextureResourceAsync(ZipArchiveEntry entry)
		{
			// Open the image entry
			await using Stream entryStream = entry.Open();
			
			// Get a temporary path to extract it to
			string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

			await using(FileStream tempStream = File.OpenWrite(temp))
			{
				await entryStream.CopyToAsync(tempStream);
			}

			Texture2D? result = null;
			try
			{
				result = await GetTextureFromPath(temp);
			}
			finally
			{
				File.Delete(temp);
			}

			return result;
		}
		
		private async Task<Texture2D?> GetTextureFromPath(string path)
		{
			UnityWebRequest? uwr = UnityWebRequestTexture.GetTexture(path);
			if (uwr == null)
				return null;

			await uwr.SendWebRequest();

			if (uwr.result != UnityWebRequest.Result.Success)
				return  null;

			return DownloadHandlerTexture.GetContent(uwr);
		}

		private class LoadThemeAssets : ThemeAssets
		{
			private readonly ThemeLoader themeLoader;
			private readonly IThemeResourceStorage storage;

			public LoadThemeAssets(ThemeLoader loader, IThemeResourceStorage storage)
			{
				this.themeLoader = loader;
				this.storage = storage;
			}

			/// <inheritdoc />
			public override void RequestTexture(string name, Action<Texture2D?> callback)
			{
				if (!storage.TryGetTexture(name, out Texture2D? texture))
				{
					callback?.Invoke(null);
					return;
				}

				texture = themeLoader.GetNamedTexture(storage, name);
				callback?.Invoke(texture);
			}

			/// <inheritdoc />
			public override void SaveTexture(string name, Texture2D texture)
			{
				throw new NotSupportedException();
			}
		}

		private class LoadThemeAssetsAsync : ThemeAssets
		{
			private readonly ThemeLoader themeLoader;
			private readonly IThemeResourceStorage storage;
			private readonly ConcurrentQueue<TextureRequest> requests = new ConcurrentQueue<TextureRequest>();

			public LoadThemeAssetsAsync(ThemeLoader loader, IThemeResourceStorage storage)
			{
				this.themeLoader = loader;
				this.storage = storage;
			}

			/// <inheritdoc />
			public override void RequestTexture(string name, Action<Texture2D?> callback)
			{
				requests.Enqueue(new TextureRequest
				{
					name = name,
					callback = callback
				});
			}

			/// <inheritdoc />
			public override void SaveTexture(string name, Texture2D texture)
			{
				throw new NotSupportedException();
			}

			public async Task Load()
			{
				while (requests.Count > 0)
				{
					if (!requests.TryDequeue(out TextureRequest request))
						continue;

					await LoadRequest(request);
				}
			}

			private async Task LoadRequest(TextureRequest request)
			{
				if (storage.TryGetTexture(request.name, out Texture2D? texture))
				{
					request.callback?.Invoke(texture);
					return;
				}

				texture = await themeLoader.GetNamedTextureAsync(storage, request.name);
				request.callback?.Invoke(texture);
			}

			private struct TextureRequest
			{
				public string name;
				public Action<Texture2D?> callback;
			}
		}
	}
}