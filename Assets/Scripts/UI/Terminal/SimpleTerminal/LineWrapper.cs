using System.Collections.Generic;
using System.Text;

namespace UI.Terminal.SimpleTerminal
{
	public class LineWrapper
	{
		private readonly StringBuilder stringBuilder = new();
		private readonly StringBuilder wordBuilder = new StringBuilder();
		private int caretWord;
		private int caretChar;

		private IEnumerable<string> Break(StringBuilder sourceText, int maxWordLength, int caret)
		{
			wordBuilder.Length = 0;

			caretChar = 0;
			caretWord = 0;
			
			var wasDelim = true;

			for (var i = 0; i <= sourceText.Length; i++)
			{
				if (i == caret)
					caretChar = wordBuilder.Length;

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
						
						yield return wordBuilder.ToString();
						wordBuilder.Length = 0;
					}
				}

				wordBuilder.Append(character);
			}
		}

		public string Wrap(StringBuilder sourceText, int slack, int startY, int maxWidth, int caret, out int caretX, out int caretY, out int height)
		{
			int x = slack;
			int y = startY;

			caretX = x;
			caretY = y;
			height = 1;

			stringBuilder.Length = 0;

			var wordIndex = 0;
			foreach (string word in Break(sourceText, maxWidth, caret))
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

				stringBuilder.Append(word);
				x += word.Length;

				wordIndex++;
			}

			// Compensate for the cursor being off screen in some cases
			if (caretX == maxWidth)
			{
				caretX = 0;
				caretY++;
			}
			
			return stringBuilder.ToString();
		}
	}
}