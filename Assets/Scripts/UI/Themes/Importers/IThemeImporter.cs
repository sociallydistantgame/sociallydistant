#nullable enable

using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq;
using System.Resources;
using Nothke.Utils;
using UI.Themes.ThemeData;
using UI.Theming;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Color = System.Drawing.Color;

namespace UI.Themes.Importers
{
	/// <summary>
	///		An interface for an object that can import a theme from disk and load it.
	/// </summary>
	public interface IThemeImporter : IDisposable
	{
		Task Import(OperatingSystemTheme.ThemeEditor themeEditor);
	}

	public class ShiftOS2017Importer : IThemeImporter
	{
		private readonly string skinPath;

		public ShiftOS2017Importer(string skinPath)
		{
			this.skinPath = skinPath;
		}
		
		/// <inheritdoc />
		public void Dispose()
		{
			// TODO release managed resources here
		}
		
		/// <inheritdoc />
		public async Task Import(OperatingSystemTheme.ThemeEditor themeEditor)
		{
			if (!File.Exists(this.skinPath))
				throw new FileNotFoundException(this.skinPath);

			await using FileStream fileStream = File.OpenRead(skinPath);
			using var textReader = new StreamReader(fileStream);

			string text = await textReader.ReadToEndAsync();

			var skinData = JsonConvert.DeserializeObject<SkinData>(text);

			if (skinData == null)
				throw new FormatException("I feel like we're all nullable");
			
			themeEditor.WindowStyle.WindowBorderSizes.left = skinData.LeftBorderWidth;
			themeEditor.WindowStyle.WindowBorderSizes.bottom = skinData.BottomBorderWidth;
			themeEditor.WindowStyle.WindowBorderSizes.right = skinData.RightBorderWidth;
			themeEditor.WindowStyle.WindowBorderSizes.top = skinData.TitlebarHeight;

			Texture2D windowBorderTexture = CreateWindowTextureFromShiftOS(skinData);
			
			// Apply the window background texture to both inactive and active decorations.
			// Note that ShiftOS never had inactive windows.
			// This involves adding the texture to the theme data and then applying it to the decoration.
			string winTextureName = nameof(windowBorderTexture);
			
			themeEditor.SetGraphic(winTextureName, windowBorderTexture);
			themeEditor.WindowStyle.ActiveDecorations.UseGraphicName(winTextureName, themeEditor);
			themeEditor.WindowStyle.InactiveDecorations.UseGraphicName(winTextureName, themeEditor);
			themeEditor.WindowStyle.ActiveDecorations.SetMargins(themeEditor.WindowStyle.WindowBorderSizes);
			themeEditor.WindowStyle.InactiveDecorations.SetMargins(themeEditor.WindowStyle.WindowBorderSizes);
			
			// Backdrops - ShiftOS only has one
			Texture2D backdropTexture = GetTextureFromBitstream(skinData.DesktopBackgroundImage, skinData.DesktopColor.ToUnityColor(), skinData.SystemKey.ToUnityColor());

			string backdropName = nameof(backdropTexture);
			themeEditor.SetGraphic(backdropName, backdropTexture);
			
			themeEditor.BackdropStyle.DayTime.UseGraphicName(backdropName, themeEditor);
			themeEditor.BackdropStyle.NightTime.UseGraphicName(backdropName, themeEditor);
		}

		private Texture2D CreateWindowTextureFromShiftOS(SkinData skinData)
		{
			var resultTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);

			// Step 1: Load/generate border textures
			Texture2D leftBorderTexture = GetTextureFromBitstream(skinData.LeftBorderBG, skinData.BorderLeftBackground.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			Texture2D topBorderTexture = GetTextureFromBitstream(skinData.TitleBarBackground, skinData.TitleBackgroundColor.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			Texture2D bottomBorderTexture = GetTextureFromBitstream(skinData.BottomBorderBG, skinData.BorderBottomBackground.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			Texture2D rightBorderTexture = GetTextureFromBitstream(skinData.RightBorderBG, skinData.BorderRightBackground.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			Texture2D topLeftCornerTexture = GetTextureFromBitstream(skinData.TitleLeftBG, skinData.TitleLeftCornerBackground.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			Texture2D topRightCornerTexture = GetTextureFromBitstream(skinData.TitleRightBG, skinData.TitleRightCornerBackground.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			Texture2D bottomLeftCornerTexture = GetTextureFromBitstream(skinData.BottomLBorderBG, skinData.BorderBottomLeftBackground.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			Texture2D bottomRightCornerTexture = GetTextureFromBitstream(skinData.BottomRBorderBG, skinData.BorderBottomRightBackground.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			Texture2D clientAreaTexture = GetTextureFromBitstream(null, skinData.ControlColor.ToUnityColor(), skinData.SystemKey.ToUnityColor());
			
			// Step 2: Blit them
			RenderTexture? rt = RenderTexture.GetTemporary(1024, 1024);
			rt.BeginPixelRendering();

			GL.Clear(true, true, default);

			var renderTitleCorners = true;
            
			int titleLeftInner = skinData.TitleLeftCornerWidth;
			int titleRightInner = rt.width - skinData.TitleRightCornerWidth;

			if (!skinData.ShowTitleCorners)
			{
				renderTitleCorners = false;
				titleLeftInner = 0;
				titleRightInner = rt.width;
			}
			
			int titleWidth = titleRightInner - titleLeftInner;
			
			int leftInner = skinData.LeftBorderWidth;
			int topInner = skinData.TitlebarHeight;
			int rightInner = rt.width - skinData.RightBorderWidth;
			int bottomInner = rt.height - skinData.BottomBorderWidth;

			int clientWidth = rightInner - leftInner;
			int clientHeight = bottomInner - topInner;
			int clientWithBottomHeight = clientHeight + skinData.BottomBorderWidth;
			int clientWithBorderWidth = rt.width;

			rt.DrawSprite(
				topBorderTexture,
				new Rect(
					titleLeftInner,
					rt.height - skinData.TitlebarHeight,
					titleWidth,
					skinData.TitlebarHeight
				)
			);

			if (renderTitleCorners)
			{
				rt.DrawSprite(
					topLeftCornerTexture,
					new Rect(
						0,
						rt.height - skinData.TitlebarHeight,
						titleLeftInner,
						skinData.TitlebarHeight
					)
				);

				rt.DrawSprite(
					topRightCornerTexture,
					new Rect(
						titleRightInner,
						rt.height - skinData.TitlebarHeight,
						skinData.TitleRightCornerWidth,
						skinData.TitlebarHeight
					)
				);
			}

			rt.DrawSprite(
				leftBorderTexture,
				new Rect(
					0,
					skinData.BottomBorderWidth,
					leftInner,
					clientHeight
				)
			);
			
			rt.DrawSprite(
				rightBorderTexture,
				new Rect(
					rightInner,
					skinData.BottomBorderWidth,
					skinData.RightBorderWidth,
					clientHeight
				)
			);

			rt.DrawSprite(
				bottomBorderTexture,
				new Rect(
					leftInner,
					0,
					clientWidth,
					skinData.BottomBorderWidth
				)
			);

			rt.DrawSprite(
				bottomLeftCornerTexture,
				new Rect(
					0,
					0,
					leftInner,
					skinData.BottomBorderWidth
				)
			);

			rt.DrawSprite(
				bottomRightCornerTexture,
				new Rect(
					rightInner,
					0,
					skinData.LeftBorderWidth,
					skinData.BottomBorderWidth
				)
			);

			rt.DrawSprite(
				clientAreaTexture,
				new Rect(
					leftInner,
					skinData.BottomBorderWidth,
					clientWidth,
					clientHeight
				)
			);
			
			rt.EndRendering();

			var activeOld = RenderTexture.active;
			RenderTexture.active = rt;
			
			resultTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
			resultTexture.Apply();
			RenderTexture.active = activeOld;
			
			RenderTexture.ReleaseTemporary(rt);

			// This shit prevents my computer from crashing when I try to screenshare to my partner on discord
			// And yknow
			// You can't spell crash without ash!
			// And it's important we don't crash when we're with ash!
			// so DO NOT fuck this up!
			if (Application.isEditor && !Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(leftBorderTexture);
				UnityEngine.Object.DestroyImmediate(topBorderTexture);
				UnityEngine.Object.DestroyImmediate(bottomBorderTexture);
				UnityEngine.Object.DestroyImmediate(rightBorderTexture);
				UnityEngine.Object.DestroyImmediate(topLeftCornerTexture);
				UnityEngine.Object.DestroyImmediate(topRightCornerTexture);
				UnityEngine.Object.DestroyImmediate(bottomLeftCornerTexture);
				UnityEngine.Object.DestroyImmediate(bottomRightCornerTexture);
				UnityEngine.Object.DestroyImmediate(clientAreaTexture);
			}
			else
			{
				UnityEngine.Object.Destroy(leftBorderTexture);
				UnityEngine.Object.Destroy(topBorderTexture);
				UnityEngine.Object.Destroy(bottomBorderTexture);
				UnityEngine.Object.Destroy(rightBorderTexture);
				UnityEngine.Object.Destroy(topLeftCornerTexture);
				UnityEngine.Object.Destroy(topRightCornerTexture);
				UnityEngine.Object.Destroy(bottomLeftCornerTexture);
				UnityEngine.Object.Destroy(bottomRightCornerTexture);
				UnityEngine.Object.Destroy(clientAreaTexture);
			}
			
			return resultTexture;
		}
		
		private Texture2D GetTextureFromBitstream(byte[]? bitstream, UnityEngine.Color backgroundColorForInvalid, UnityEngine.Color key)
		{
			if (bitstream == null || bitstream.Length == 0)
			{
				var solidTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
				solidTexture.SetPixels(new[] { backgroundColorForInvalid });
				solidTexture.Apply();
				return solidTexture;
			}

			var imageTexture = new Texture2D(1, 1);
			imageTexture.LoadImage(bitstream);
			imageTexture.Apply();
			
			UnityEngine.Color[] colorData = imageTexture.GetPixels();
			
			for (var i = 0; i < colorData.Length; i++)
			{
				if (colorData[i] == key)
					colorData[i] = default;
			}
            
			imageTexture.SetPixels(colorData);
			imageTexture.Apply();
			
			return imageTexture;
		}
		
		private class SkinData
		{
			public string TitleFont = "Microsoft Sans Serif, 8pt, style=Bold";
			public string MainFont = "Microsoft Sans Serif, 8pt";
			public int TitleButtonPosition = 0;
			public string HeaderFont = "Microsoft Sans Serif, 20pt, style=Bold";
			public string Header2Font = "Microsoft Sans Serif, 17.5pt, style=Bold";
			public string Header3Font = "Microsoft Sans Serif, 15pt, style=Bold";
			public Color ControlColor;
			public Color ControlTextColor;
			public Color TitleTextColor;
			public Color TitleBackgroundColor;
			public Color BorderLeftBackground;
			public Color BorderRightBackground;
			public int PanelButtonFromTop = 4;
			public Color BorderBottomBackground;
			public int PanelButtonHolderFromLeft = 60;
			public Color BorderBottomLeftBackground;
			public Color BorderBottomRightBackground;
			public Color CloseButtonColor;
			public Color MaximizeButtonColor;
			public Color MinimizeButtonColor;
			public Color DesktopPanelColor;
			public Color DesktopPanelClockColor;
			public Color DesktopPanelClockBackgroundColor;
			public string DesktopPanelClockFont = "Microsoft Sans Serif, 8pt";
			public Point DesktopPanelClockFromRight;
			public int DesktopPanelHeight = 28;
			public int DesktopPanelPosition = 1;
			public int TitlebarHeight = 23;
			public Size CloseButtonSize;
			public Size MaximizeButtonSize;
			public Size MinimizeButtonSize;
			public Size CloseButtonFromSide;
			public Size MaximizeButtonFromSide;
			public Size MinimizeButtonFromSide;
			public bool TitleTextCentered = false;
			public Point TitleTextLeft;
			public Color DesktopColor;
			public byte[] CloseButtonImage;
			public byte[] MinimizeButtonImage;
			public byte[] MaximizeButtonImage;
			public byte[] DesktopBackgroundImage = null;
			public Color AppLauncherTextColor;
			public Color AppLauncherSelectedTextColor;
			public string AppLauncherFont = "Microsoft Sans Serif, 8pt";
			public string AppLauncherText = "";
			public Point AppLauncherFromLeft;
			public Size AppLauncherHolderSize;
			public byte[] AppLauncherImage;
			public string TerminalFont = "Consolas, 9pt";
			public Color TerminalForeColor;
			public Color TerminalBackColor;
			public byte[] DesktopPanelBackground;
			public byte[] TitleBarBackground;
			public bool ShowTitleCorners = true;
			public Color TitleLeftCornerBackground;
			public Color TitleRightCornerBackground;
			public int TitleLeftCornerWidth = 4;
			public int TitleRightCornerWidth = 4;
			public byte[] TitleLeftBG;
			public byte[] TitleRightBG;
			public Color SystemKey;
			public byte[] BottomBorderBG;
			public byte[] BottomRBorderBG;
			public byte[] BottomLBorderBG;
			public byte[] LeftBorderBG;
			public byte[] RightBorderBG;
			public int LeftBorderWidth = 4;
			public int RightBorderWidth = 4;
			public int BottomBorderWidth = 4;
			public byte[] PanelButtonBG;
			public Size PanelButtonSize;
			public Color PanelButtonColor;
			public Color PanelButtonTextColor;
			public Point PanelButtonFromLeft;
			public string PanelButtonFont = "Microsoft Sans Serif, 8pt";
		}
	}

	public class ShiftOSSkinFactory
	{
		private static readonly byte[] zipMagic = new byte[] { 0x50, 0x4b, 0x03, 0x04 };
		
		public static async Task<OperatingSystemTheme> ImportTheme(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException(path);

			string themeName = Path.GetFileNameWithoutExtension(path);
			
			bool isZippedFolder = await IdentifyZippedFolder(path);
			bool isJsonObject = await IdentifyJson(path);

			if (isJsonObject)
			{
				OperatingSystemTheme.ThemeEditor theme = OperatingSystemTheme.CreateEmpty(true, true);
				theme.Name = themeName;

				var twentySeventeenImporter = new ShiftOS2017Importer(path);

				await twentySeventeenImporter.Import(theme);
				
				return theme.Theme;
			}
			
			if (isZippedFolder)
			{
				// TODO: do stuff, also ash is cute <3
			}

			throw new FormatException("Not a recognized ShiftOS skin");
		}

		private static async Task<bool> IdentifyJson(string path)
		{
			await using FileStream fileStream = File.OpenRead(path);
			using var textReader = new StreamReader(fileStream);

			var buffer = new char[1];
			int charsRead = await textReader.ReadAsync(buffer, 0, buffer.Length);

			return charsRead == buffer.Length && buffer[0] == '{';

		}
		
		private static async Task<bool> IdentifyZippedFolder(string path)
		{
			await using FileStream fileStream = File.OpenRead(path);

			var magicNumber = new byte[4];
			int readCount = await fileStream.ReadAsync(magicNumber, 0, magicNumber.Length);

			return readCount == magicNumber.Length && magicNumber.SequenceEqual(zipMagic);

		}
	}

	public static class PhilipAdamsReallyFuckingAnnoysMeBecauseIHaveToWriteTheseExtensions
	{
		public static UnityEngine.Color ToUnityColor(this System.Drawing.Color gdiColor)
		{
			return new UnityEngine.Color(
				gdiColor.R / 255f,
				gdiColor.G / 255f,
				gdiColor.B / 255f,
				gdiColor.A / 255f
			);
		}
	}
}