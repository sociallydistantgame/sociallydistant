#nullable enable

using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq;
using UI.Themes.ThemeData;
using UI.Theming;

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

			bool isZippedFolder = await IdentifyZippedFolder(path);
			bool isJsonObject = await IdentifyJson(path);

			if (isJsonObject)
			{
				OperatingSystemTheme.ThemeEditor theme = OperatingSystemTheme.CreateEmpty(true, true);

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
}