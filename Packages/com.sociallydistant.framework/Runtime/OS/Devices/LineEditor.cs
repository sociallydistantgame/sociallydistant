#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace OS.Devices
{
	public class LineEditor
	{
		private readonly ITextConsole console;
		private readonly StringBuilder lineBuilder = new StringBuilder();
		private readonly LineWrapper lineWrapper = new LineWrapper();
		private readonly List<string> completions = new List<string>();
		private int completionInsertionPoint;
		private int caretIndex;
		private State state;
		private int previousLineCount;
		private int firstColumn;
		private int firstRow;
		private int width;
		private int height;
		private int selectedCompletion;
		private int lastCursorX;
		private int lastCursorY;
		private IAutoCompleteSource? autoCompleteSource;
		private bool completionsAreDirty = false;

		public IAutoCompleteSource? AutoCompleteSource
		{
			get => autoCompleteSource;
			set => autoCompleteSource = value;
		}
		
		public bool UsePasswordChars { get; set; }
		
		public LineEditor(ITextConsole console)
		{
			this.console = console;
		}

		public bool Update(out string text)
		{
			text = string.Empty;

			switch (state)
			{
				case State.None:
					if (console.IsInteractive)
					{
						SendInformationRequests();
						state = State.Preparing;
					}
					else
					{
						state = State.Editing;
					}
					break;
				case State.Preparing:
					// TODO: Columns, rows, cursor position, etc. should be retrieved here if in interactive mode.
					state = State.Editing;
					break;
				case State.Editing:
					if (console.IsInteractive)
					{
						InteractiveUpdate();
					}
					else
					{
						BasicUpdate();
					}
					break;
				
				case State.Done:
					if (console.IsInteractive)
						WriteText(Environment.NewLine);
					
					text = lineBuilder.ToString();
					CancelCurrentLine();
					return true;
			}
			
			return false;
		}

		public async Task<string> ReadLineAsync()
		{
			CancelCurrentLine();

			var result = string.Empty;
            
			while (!Update(out result))
				await Task.Yield();

			return result;
		}

		private void CancelCurrentLine()
		{
			state = State.None;
			lineBuilder.Length = 0;
			caretIndex = 0;
			previousLineCount = 0;
			firstColumn = 0;
			firstRow = 0;
		}

		private void SendInformationRequests()
		{
			if (console is ITextConsoleWithPhysicalDisplay physicalConsole)
			{
				firstColumn = physicalConsole.CursorLeft;
				firstRow = physicalConsole.CursorTop;
				width = physicalConsole.Width;
				height = physicalConsole.Height;
			}
			else
			{
				// if for whatever reason we get a console that doesn't implement ITextConsoleWithPhysicalScreen,
				// then fall back to default values.
				//
				// TODO: We should consider using ANSI escape sequences to query the terminal for this information, particularly
				// in cases where the physical terminal isn't directly visible to us (i.e, pipes or in multiplayer where the terminal is on client and we're the server).
				firstColumn = 0;
				firstRow = 0;
				width = 80;
				height = 24;
			}

			lastCursorX = firstColumn;
			lastCursorY = firstRow;
		}

		private bool CheckScreenChanges()
		{
			if (console is ITextConsoleWithPhysicalDisplay screen)
			{
				var hasChanged = false;

				if (screen.Width != width || screen.Height != height)
				{
					width = screen.Width;
					height = screen.Height;
					hasChanged = true;
				}

				if (hasChanged || (screen.CursorLeft != lastCursorX || screen.CursorTop != lastCursorY))
				{
					lastCursorX = screen.CursorLeft;
					lastCursorY = screen.CursorTop;
					firstColumn = lastCursorX;
					firstRow = lastCursorY;
					hasChanged = true;
				}
				
				return hasChanged;
			}

			return false;
		}
		
		private void InteractiveUpdate()
		{
			bool hadKeyStroke = BasicUpdate();
			bool hasScreenChanged = CheckScreenChanges();
			bool needsRender = hadKeyStroke || hasScreenChanged;

			if (!needsRender)
				return;

			this.UpdateCompletions();
			this.RenderLineEditor();
		}

		private bool BasicUpdate()
		{
			ConsoleInputData? readData = console.ReadInput();
			if (!readData.HasValue)
				return false;
			
			HandleInput(readData.Value);
			return true;
		}

		private void HandleInput(ConsoleInputData input)
		{
			if (!char.IsControl(input.Character))
			{
				this.lineBuilder.Insert(caretIndex, input.Character);
				caretIndex++;
				completionsAreDirty = true;
				return;
			}

			// Interactive mode keystrokes
			if (console.IsInteractive)
			{
				switch (input.KeyCode)
				{
					// Insert selected completion
					case KeyCode.Tab when !input.HasModifiers && !UsePasswordChars:
						this.InsertAutoComplete();
						break;
					
					case KeyCode.UpArrow when !input.HasModifiers && !UsePasswordChars:
						SelectPreviousCompletion();
						break;
					case KeyCode.DownArrow when !input.HasModifiers && !UsePasswordChars:
						SelectNextCompletion();
						break;
					
				}
			}
			
			switch (input.KeyCode)
			{
				case KeyCode.LeftArrow:
					caretIndex = Math.Max(caretIndex - 1, 0);
					break;
				case KeyCode.RightArrow:
					caretIndex = Math.Min(caretIndex + 1, lineBuilder.Length);
					break;
				case KeyCode.Home:
					caretIndex = 0;
					break;
				case KeyCode.End:
					caretIndex = lineBuilder.Length;
					break;
				case KeyCode.KeypadEnter:
				case KeyCode.Return:
					state = State.Done;
					break;
				case KeyCode.Backspace:
					if (caretIndex <= 0)
						break;

					caretIndex--;
					lineBuilder.Remove(caretIndex, 1);
					completionsAreDirty = true;
					break;
				case KeyCode.Delete:
					if (caretIndex == lineBuilder.Length)
						break;

					lineBuilder.Remove(caretIndex, 1);
					completionsAreDirty = true;
					break;
			}
		}

		private void WriteText(string text)
		{
			this.console.WriteText(text);
		}
		
		private void RenderLineEditor()
        {
	        string wordWrapped = lineWrapper.Wrap(this.lineBuilder, firstColumn, firstRow, width, this.caretIndex, out int cx, out int cy, out int lineCount, out int lastLineWidth);
            var completionIndicator = string.Empty;
            var completionsOnNewLine = false;

            if (caretIndex >= completionInsertionPoint && completions.Count > 0)
            {
                if (completions.Count > 1)
                    completionIndicator = $"{completions[selectedCompletion].Substring(lineBuilder.Length - completionInsertionPoint)} ({selectedCompletion + 1}/{completions.Count})";
                else
                    completionIndicator = completions[0].Substring(lineBuilder.Length - completionInsertionPoint);

                if (lastLineWidth + completionIndicator.Length >= width)
                {
                    lineCount++;
                    completionsOnNewLine = true;
                }
            }

            // Determine if we need to scroll.
            int bottom = this.firstRow + lineCount;
            int previousBottom = this.firstRow + previousLineCount; 
            if (bottom > this.height)
            {
	            int scroll = bottom - this.height;
                this.firstRow -= scroll; // Adjust first row to compensate

                // Scroll up
                this.WriteText($"\x1b[{scroll}S");
            }
            
            // Handle scrolling back down when we no longer occupy the last line.
            if (previousBottom > bottom && previousBottom==this.height)
            {
                int scroll = previousBottom - bottom;
                this.firstRow += scroll; // Adjust first row to compensate

                this.WriteText($"\x1b[{scroll}T");
            }

            // Move the cursor back to the start of the line, visually
            this.WriteText($"\x1b[{this.firstRow + 1};{this.firstColumn + 1}H");

            // If we have previous text, then we need to delete it
            int linesToDelete = Math.Max(this.previousLineCount, lineCount);
            if (linesToDelete > 0) this.WriteText("\x1b[0J");

            // Write each line
            this.WriteText(wordWrapped);

            // Write the completions indicator
            if (completionIndicator.Length > 0)
            {
                if (completionsOnNewLine)
                    WriteText(Environment.NewLine);
                WriteText("\x1b[2;3;90m");
                WriteText(completionIndicator);
                WriteText("\x1b[22;23;39m");
            }
            
            // Move cursor to where it should be in the line editor
            this.WriteText($"\x1b[{cy + 1};{cx + 1}H");
            lastCursorX = cx;
            lastCursorY = cy;

            this.previousLineCount = lineCount;
        }
		
		private void UpdateCompletions()
		{
			if (!completionsAreDirty)
				return;
            
			this.completions.Clear();
			if (this.autoCompleteSource == null)
				return;
            
			// ignore completions when entering a password.
			if (UsePasswordChars)
				return;
			
			this.completions.AddRange(this.autoCompleteSource.GetCompletions(this.lineBuilder, out completionInsertionPoint));
			this.selectedCompletion = 0;
			this.completionsAreDirty = false;
		}
		
		private void InsertAutoComplete()
		{
			if (UsePasswordChars)
				return;
			
			if (completions.Count == 0)
			{
				this.WriteText("\a");
				return;
			}

			completionsAreDirty = true;

			string completion = completions[selectedCompletion];
			lineBuilder.Length = completionInsertionPoint;
			lineBuilder.Append(completion);
			caretIndex = lineBuilder.Length;
			completionsAreDirty = true;
		}
		
        private void SelectPreviousCompletion()
        {
        	if (UsePasswordChars)
            	return;
            
            if (completions.Count == 0)
            	return;
            
            if (selectedCompletion > 0)
            {
                this.WriteText("\a");
                return;
            }
            
            this.selectedCompletion--;
        }
		
        private void SelectNextCompletion()
        {
        	if (UsePasswordChars)
            	return;
            
            if (completions.Count == 0)
            	return;

            if (selectedCompletion >= completions.Count - 1)
            {
	            this.WriteText("\a");
	            return;
            }

            this.selectedCompletion++;
        }
		
		private enum State
		{
			None,
			Preparing,
			Editing,
			Done
		}
	}
}