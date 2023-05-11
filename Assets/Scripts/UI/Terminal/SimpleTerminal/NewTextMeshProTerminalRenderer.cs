using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Terminal.SimpleTerminal.Data;
using UnityEngine.Assertions;

namespace UI.Terminal.SimpleTerminal
{
	public class NewTextMeshProTerminalRenderer
	{
		private readonly GameObject rootGameObject;
		private readonly GameObject backgroundGameObject;
		private readonly GameObject foregroundGameObject;
		private readonly VerticalLayoutGroup rootLayout;
		private readonly LayoutElement backgroundLayout;
		private readonly TextMeshProUGUI background;
		private readonly TextMeshProUGUI foreground;
		private readonly StringBuilder backgroundBuilder = new StringBuilder();
		private readonly StringBuilder foregroundBuilder = new StringBuilder();
		private readonly List<RowData> rowDataList = new List<RowData>();

		private bool dirty = false;
		private int rows;
		
		public NewTextMeshProTerminalRenderer(Transform parent)
		{
			// Create the gameobjects
			this.rootGameObject = new GameObject("Text Area Root");
			this.backgroundGameObject = new GameObject("Background");
			this.foregroundGameObject = new GameObject("Foreground");
			
			// Add components
			this.rootLayout = this.rootGameObject.AddComponent<VerticalLayoutGroup>();
			this.backgroundLayout = this.backgroundGameObject.AddComponent<LayoutElement>();
			this.background = this.backgroundGameObject.AddComponent<TextMeshProUGUI>();
			this.foreground = this.foregroundGameObject.AddComponent<TextMeshProUGUI>();
			
			// Hierarchy setup
			this.rootGameObject.transform.SetParent(parent);
			this.backgroundGameObject.transform.SetParent(this.rootGameObject.transform);
			this.foregroundGameObject.transform.SetParent(this.rootGameObject.transform);
			
			// Layout setup
			this.backgroundLayout.ignoreLayout = true;
		}

		public void SetFont(TMP_FontAsset font, int fontSize)
		{
			this.background.font = font;
			this.foreground.font = font;
			this.background.fontSize = fontSize;
			this.foreground.fontSize = fontSize;
		}

		public void Update()
		{
			if (!dirty)
				return;

			this.RebuildText();
			
			background.SetText(backgroundBuilder);
			foreground.SetText(foregroundBuilder);
			dirty = false;
		}

		public void UpdateRow(SimpleTerminal term, int row, Glyph[] glyphs, int colStart, int colEnd)
		{
			RowData rowData = rowDataList[row];

			dirty |= rowData.UpdateGlyphs(term, glyphs, colStart, colEnd, row);
		}
		
		public void Resize(int newColumns, int newRows)
		{
			for (var i = 0; i < rows; i++)
				rowDataList[i].Resize(newColumns);
			
			while (rows < newRows)
			{
				var data = new RowData(this, rows == 0 ? null : rowDataList[rows - 1]);
				rows++;
				this.rowDataList.Add(data);
				data.Resize(newColumns);
			}

			if (rows > newRows)
			{
				RowData first = rowDataList[newRows];

				backgroundBuilder.Length = first.BackgroundOffset;
				foregroundBuilder.Length = first.ForegroundOffset;
				dirty = true;

				while (rows > newRows)
				{
					rowDataList.RemoveAt(newRows);
					rows--;
				}
			}
		}

		private void RebuildText()
		{
			backgroundBuilder.Length = 0;
			foregroundBuilder.Length = 0;
			
			// Pass 2: Re-build text.
			for (var i = 0; i < rows; i++)
			{
				RowData row = rowDataList[i];
				row.BuildText(backgroundBuilder, foregroundBuilder);
			}
		}
		
		private class RowData
		{
			private readonly NewTextMeshProTerminalRenderer renderer;
			private readonly RowData? previousRow;

			private int columns;
			private ColumnData? firstColumn;
			private ColumnData? lastColumn;

			private int fgDirtStart = -1;
			private int fgDirtiness;
			private int bgDirtStart = -1;
			private int bgDirtiness;
			
			public int BackgroundOffset => previousRow?.BackgroundEnd ?? 0;
			public int BackgroundLength { get; private set; }
			public int BackgroundEnd => BackgroundOffset + BackgroundLength;

			public int ForegroundOffset => previousRow?.ForegroundEnd ?? 0;
			public int ForegroundLength { get; private set; }
			public int ForegroundEnd => ForegroundOffset + ForegroundLength;

			public RowData(NewTextMeshProTerminalRenderer renderer, RowData? prevRow)
			{
				this.renderer = renderer;
				this.previousRow = prevRow;
			}

			public void Resize(int newColumns)
			{
				while (columns < newColumns)
				{
					if (firstColumn == null || lastColumn==null)
					{
						firstColumn = new ColumnData(this);
						lastColumn = firstColumn;
					}
					else
					{
						lastColumn.Next = new ColumnData(this);
						lastColumn.Next.Previous = lastColumn;
						lastColumn = lastColumn.Next;
					}

					columns++;
				}

				if (columns > newColumns)
				{
					int start = 0;
					int length = 0;
					int fStart = 0;
					int fLength = 0;


					while (columns > newColumns)
					{
						ColumnData column = lastColumn;

						start = column?.BackgroundOffset ?? BackgroundOffset;
						length += column?.BackgroundLength ?? 0;

						fStart = column?.ForegroundOffset ?? ForegroundOffset;
						fLength += column?.ForegroundLength ?? 0;

						lastColumn = column?.Previous;

						if (lastColumn != null)
							lastColumn.Next = null;
						
						if (column != null)
						{
							column.Next = null;
							column.Previous = null;
						}

						columns--;
					}
				}
			}

			public bool UpdateGlyphs(SimpleTerminal term, Glyph[] glyphs, int start, int end, int y)
			{
				var i = 0;
				ColumnData column = this.firstColumn;
				var dirty = false;

				while (column != null && i < end)
				{
					if (i >= start)
					{
						Glyph glyph = glyphs[i];
						dirty |= column.SetGlyph(term, glyph, term.Selected(i, y));
					}
					
					column = column.Next;
					i++;
				}

				return dirty;
			}

			public void BuildText(StringBuilder backgroundBuilder, StringBuilder foregroundBuilder)
			{
				ColumnData column = firstColumn;
				while (column != null)
				{
					column.BuildText(backgroundBuilder, foregroundBuilder);
					column = column.Next;
				}

				this.BackgroundLength = lastColumn.BackgroundEnd - firstColumn.BackgroundOffset;
				this.ForegroundLength = this.lastColumn.ForegroundEnd - this.firstColumn.ForegroundOffset;
			}
		}

		private class ColumnData
		{
			private RowData row;
			private ColumnData? next;
			private ColumnData? previous;
			private bool isDirty;

			private char character;

			public int BackgroundOffset => previous?.BackgroundEnd ?? row.BackgroundOffset;
			public int BackgroundLength { get; private set; }
			public int BackgroundEnd { get; private set; }

			public int ForegroundOffset => previous?.ForegroundEnd ?? row.ForegroundOffset;
			public int ForegroundLength { get; private set; }
			public int ForegroundEnd => ForegroundOffset + ForegroundLength;

			public ColumnData? Next
			{
				get => next;
				set
				{
					bool wasNull = next == null;
					next = value;
					if (wasNull && next != null || next == null && !wasNull)
						isDirty = true;
				}
			}

			public ColumnData? Previous
			{
				get => previous;
				set
				{
					previous = value;
					BackgroundEnd = BackgroundOffset;
					//ForegroundEnd = ForegroundOffset;
				}
			}
			
			public ColumnData(RowData row)
			{
				this.row = row;
			}

			public void BuildText(StringBuilder backgroundBuilder, StringBuilder foregroundBuilder)
			{
				//ForegroundEnd = ForegroundOffset + ForegroundLength;
				BackgroundEnd = BackgroundOffset + BackgroundLength;


				// Capture the lengths of the builders before we've modified them.
				// This is used later on so we know how much text the column has.
				int oldLengthBG = backgroundBuilder.Length;
				int oldLengthFG = foregroundBuilder.Length;
				
				// This is where text must be inserted from.
				int startBG = this.BackgroundOffset;
				int startFG = this.ForegroundOffset;

				// Insert the actual character.
				foregroundBuilder.Append(character);
				
				// Insert newlines if we're the last character in a row.
				if (Next == null)
				{
					backgroundBuilder.Append(Environment.NewLine);
					foregroundBuilder.Append(Environment.NewLine);
				}
				
				// Update the column lengths.
				this.BackgroundLength = Math.Max(oldLengthBG, backgroundBuilder.Length) - Math.Min(oldLengthBG, backgroundBuilder.Length);
				this.ForegroundLength = Math.Max(oldLengthFG, foregroundBuilder.Length) - Math.Min(oldLengthFG, foregroundBuilder.Length);

				BackgroundEnd = BackgroundOffset + BackgroundLength;
				//ForegroundEnd = ForegroundOffset + ForegroundLength;
				
				//ForegroundEnd = ForegroundOffset + ForegroundLength;
				BackgroundEnd = BackgroundOffset + BackgroundLength;

				
				isDirty = false;
			}
			
			public bool SetGlyph(SimpleTerminal term, Glyph glyph, bool selected)
			{
				bool dirty = SetCharacter(glyph.character);

				// TODO: Color
				// TODO: Font attributes

				isDirty |= dirty;
				return dirty;
			}

			private bool SetCharacter(char newChar)
			{
				if (newChar == '\0')
					newChar = ' ';

				bool different = this.character != newChar;
				this.character = newChar;
				return different;
			}
		}
	}
}