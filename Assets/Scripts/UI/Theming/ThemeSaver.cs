#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Steamworks.Ugc;
using UI.Themes.Serialization;
using UI.Themes.ThemeData;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace UI.Theming
{
	public class ThemeSaver :
		ThemeAssets,
		IDisposable
	{
		private readonly OperatingSystemTheme.ThemeEditor editor;
		private readonly Stream outputStream;
		private readonly ZipArchive archive;
		private readonly ConcurrentQueue<TextureSave> textures = new ConcurrentQueue<TextureSave>();

		public ThemeSaver(string path, OperatingSystemTheme.ThemeEditor editor)
		{
			this.editor = editor;
			outputStream = File.OpenWrite(path);
			outputStream.SetLength(0);
			archive = new ZipArchive(outputStream, ZipArchiveMode.Create);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			archive.Dispose();
			outputStream.Dispose();
		}

		public async Task SaveAsync()
		{
			await SavePreviewImage();
			await SaveMetadata();
			await SaveColors();
			await SaveBackdrop();
			await SaveWidgets();
			await SaveWindows();
			await SaveShell();
			await SaveTerminal();
			await SaveTextures();
		}

		private async Task SaveTextures()
		{
			var savedNames = new List<string>();

			ZipArchiveEntry? assets = null;
			
			while (textures.Count > 0)
			{
				if (!textures.TryDequeue(out TextureSave save))
					continue;

				if (save.texture == null)
					continue;
				
				if (savedNames.Contains(save.name))
					continue;
				
				savedNames.Add(save.name);

				if (assets == null)
					assets = archive.CreateEntry(ThemeFileConstants.AssetsDirectoryName);

				await using Stream assetStream = OpenEntryForWriting(ThemeFileConstants.AssetsDirectoryName + save.name);

				byte[] bytes = save.texture.EncodeToPNG();

				using var memory = new MemoryStream(bytes);

				await memory.CopyToAsync(assetStream);
			}
		}

		private async Task SaveBackdrop()
		{
			await using Stream widgetStream = OpenEntryForWriting(ThemeFileConstants.BackdropName);

			var serializer = XmlThemeDataSerializer.Create(nameof(BackdropStyle));

			await Task.Run(() =>
			{
				editor.BackdropStyle.Serialize(serializer, this);
			});

			await serializer.SaveTo(widgetStream);
		}
		
		private async Task SaveShell()
		{
			await using Stream widgetStream = OpenEntryForWriting(ThemeFileConstants.ShellName);

			var serializer = XmlThemeDataSerializer.Create(nameof(ShellStyle));

			await Task.Run(() =>
			{
				editor.ShellStyle.Serialize(serializer, this);
			});

			await serializer.SaveTo(widgetStream);
		}
		
		private async Task SaveTerminal()
		{
			await using Stream widgetStream = OpenEntryForWriting(ThemeFileConstants.TerminalName);

			var serializer = XmlThemeDataSerializer.Create(nameof(TerminalStyle));

			await Task.Run(() =>
			{
				editor.TerminalStyle.Serialize(serializer, this);
			});

			await serializer.SaveTo(widgetStream);
		}
		
		private async Task SaveWidgets()
		{
			await using Stream widgetStream = OpenEntryForWriting(ThemeFileConstants.WidgetDataName);

			var serializer = XmlThemeDataSerializer.Create(nameof(WidgetStyle));

			await Task.Run(() =>
			{
				editor.WidgetStyle.Serialize(serializer, this);
			});

			await serializer.SaveTo(widgetStream);
		}
		
		private async Task SaveWindows()
		{
			await using Stream widgetStream = OpenEntryForWriting(ThemeFileConstants.WindowStyle);

			var serializer = XmlThemeDataSerializer.Create(nameof(WindowStyle));

			await Task.Run(() =>
			{
				editor.WindowStyle.Serialize(serializer, this);
			});

			await serializer.SaveTo(widgetStream);
		}

		private async Task SavePreviewImage()
		{
			if (editor.PreviewImage == null)
				return;

			byte[] pngBytes = editor.PreviewImage.EncodeToPNG();

			using var memory = new MemoryStream(pngBytes);

			await using Stream previewStream = OpenEntryForWriting(ThemeFileConstants.PreviewImageName);
			await memory.CopyToAsync(previewStream);
		}

		private async Task SaveMetadata()
		{
			await using Stream metadataStream = OpenEntryForWriting(ThemeFileConstants.MetadataName);

			var serializer = XmlThemeDataSerializer.Create(nameof(ThemeMetadata));

			string name = editor.Name;
			string localAuthor = editor.LocalAuthor;
			string description = editor.Description;

			serializer.Serialize(ref name, nameof(editor.Name), name);
			serializer.Serialize(ref localAuthor, nameof(editor.LocalAuthor), localAuthor);
			serializer.Serialize(ref description, nameof(editor.Description), description);

			await serializer.SaveTo(metadataStream);
		}

		private async Task SaveColors()
		{
			await using Stream colorsStream = OpenEntryForWriting(ThemeFileConstants.ColorTableName);
			
			var serializer = XmlThemeDataSerializer.Create(nameof(NamedColorTable));

			IElementSerializer? colors = serializer.GetChildElement(nameof(editor.Colors));
			if (colors != null)
			{
				foreach (string key in editor.Colors.Keys)
				{
					NamedColor namedColor = editor.Colors[key];
					
					string name = key;
					string darkHex = ColorUtility.ToHtmlStringRGBA(namedColor.darkColor);
					string lightHex = ColorUtility.ToHtmlStringRGBA(namedColor.lightColor);

					IElementSerializer? item = colors.GetChildElement(nameof(NamedColor));

					if (item == null)
						continue;
					
					item.Serialize(ref name, nameof(name), name);
					item.Serialize(ref lightHex, nameof(lightHex), lightHex);
					item.Serialize(ref darkHex, nameof(darkHex), darkHex);
				}
			}

			await serializer.SaveTo(colorsStream);
		}
		
		private Stream OpenEntryForWriting(string name)
		{
			ZipArchiveEntry entry = archive.CreateEntry(name, CompressionLevel.Optimal);
			return entry.Open();
		}

		/// <inheritdoc />
		public override void RequestTexture(string name, Action<Texture2D?> callback)
		{
			callback?.Invoke(null);
		}

		/// <inheritdoc />
		public override void SaveTexture(string name, Texture2D? texture)
		{
			textures.Enqueue(new TextureSave
			{
				name = name,
				texture = texture
			});
		}

		private struct TextureSave
		{
			public string name;
			public Texture2D? texture;
		}
	}
}