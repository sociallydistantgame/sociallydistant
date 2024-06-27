#nullable enable
using System.Text;
using System.Collections.Generic;

namespace OS.Devices
{
	public class LineWrapper
	{
		private readonly StringBuilder stringBuilder = new();
		private readonly StringBuilder wordBuilder = new StringBuilder();
		private int caretWord;
		private int caretChar;
		private int selStartChar;
		private int selEndChar;
		private int selStartWord;
		private int selEndWord;

		private IEnumerable<string> Break(StringBuilder sourceText, int maxWordLength, int caret, int selectionStart, int selectionEnd)
		{
			wordBuilder.Length = 0;

			caretChar = 0;
			caretWord = 0;
			selStartChar = 0;
			selStartWord = 0;
			selEndChar = 0;
			selEndWord = 0;
			
			var wasDelim = true;

			for (var i = 0; i <= sourceText.Length; i++)
			{
				if (i == caret)
					caretChar = wordBuilder.Length;

				if (i == selectionStart)
					selStartChar = wordBuilder.Length;
				
				if (i == selectionEnd)
					selEndChar = wordBuilder.Length;
				
				if (i == sourceText.Length)
				{
					if (wordBuilder.Length > 0)
					{
						yield return wordBuilder.ToString();
						wordBuilder.Length = 0;
					}
					
					break;
				}

				char character = sourceText[i];

				bool isDelim = char.IsWhiteSpace(character);

				if (isDelim != wasDelim)
				{
					if (!isDelim)
					{
						// Start of a new word.
						if (wordBuilder.Length > 0)
						{
							if (i < caret)
								caretWord++;

							if (i < selectionStart)
								selStartWord++;

							if (i < selectionEnd)
								selEndWord++;
							
							yield return wordBuilder.ToString();
							wordBuilder.Length = 0;
						}
					}
					
					wasDelim = isDelim;
				}
				else
				{
					if (wordBuilder.Length == maxWordLength)
					{
						if (i < caret)
							caretWord++;
						
						if (i < selectionStart)
							selStartWord++;

						if (i < selectionEnd)
							selEndWord++;
						
						yield return wordBuilder.ToString();
						wordBuilder.Length = 0;
					}
				}

				wordBuilder.Append(character);
			}
		}

		public string Wrap(StringBuilder sourceText, int slack, int startY, int maxWidth, int caret, int selectionStart, int selectionEnd, out int caretX, out int caretY, out int height, out int lastLineWidth, out int wrappedSelectionStart, out int wrappedSelectionEnd)
		{
			int x = slack;
			int y = startY;

			lastLineWidth = 0;
			caretX = x;
			caretY = y;
			height = 1;
			wrappedSelectionStart = -1;
			wrappedSelectionEnd = -1;

			stringBuilder.Length = 0;

			var wordIndex = 0;
			foreach (string word in Break(sourceText, maxWidth, caret, selectionStart, selectionEnd))
			{
				if (x + word.Length > maxWidth)
				{
					stringBuilder.AppendLine();
					x = 0;
					y++;
					height++;
				}

				if (wordIndex == caretWord)
				{
					caretY = y;
					caretX = x + caretChar;
				}

				if (wordIndex == selStartWord)
					wrappedSelectionStart = stringBuilder.Length + selStartChar;
				
				if (wordIndex == selEndWord)
					wrappedSelectionEnd = stringBuilder.Length + selEndChar;

				
				stringBuilder.Append(word);
				x += word.Length;
				lastLineWidth = x;

				wordIndex++;
			}

			// Compensate for the cursor being off screen in some cases
			if (caretX == maxWidth)
			{
				caretX = 0;
				caretY++;

				if (caretY > y)
				{
					height++;
					lastLineWidth = 0;
				}
			}
			
			return stringBuilder.ToString();
		}
	}
}