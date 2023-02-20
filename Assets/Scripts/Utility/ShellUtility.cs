using System.Collections.Generic;
using System.Linq;
using System.Text;
using Architecture;
using TMPro;

namespace Utility
{
	public static class ShellUtility
	{
		/// <summary>
		///		Tokenizes the given raw command-line input into a list of words
		///		and returns whether one should continue reading lines of text.
		/// </summary>
		/// <param name="input">The raw input to tokenize</param>
		/// <param name="tokens">The destination token list</param>
		/// <returns>
		///		True if the command is considered complete and may be parsed.
		///		False if the command must continue to be read.
		/// </returns>
		public static bool SimpleTokenize(StringBuilder input, List<string> tokens)
		{
			var wordList = new List<string>();
			var currentWord = new StringBuilder();

			var escape = false;
			var quote = false;
			
			for (var i = 0; i <= input.Length; i++)
			{
				if (i == input.Length)
				{
					if (currentWord.Length > 0)
						wordList.Add(currentWord.ToString());

					currentWord.Length = 0;
					break;
				}
				
				char character = input[i];

				if (escape)
				{
					switch (character)
					{
						case '\r' when !quote:
						case '\n' when !quote:
							escape = false;
							continue;
					}
					currentWord.Append(character);
					escape = false;
					continue;
				}

				if (character == '\\')
				{
					escape = true;
					continue;
				}

				if (character == '"')
				{
					quote = !quote;

					if (!quote && currentWord.Length > 0)
					{
						wordList.Add(currentWord.ToString());
						currentWord.Length = 0;
					}

					continue;
				}

				if (character == '#' && !quote)
				{
					if (currentWord.Length > 0)
					{
						wordList.Add(currentWord.ToString());
						currentWord.Length = 0;
					}
					
					break;
				}
				
				if (char.IsWhiteSpace(character) && !quote)
				{
					if (currentWord.Length > 0)
					{
						wordList.Add(currentWord.ToString());
						currentWord.Length = 0;
					}

					continue;
				}

				currentWord.Append(character);
			}

			tokens.AddRange(wordList);
			return !(quote || escape);
		}

		public static IEnumerable<ShellToken> IdentifyTokens(StringBuilder rawInput)
		{
			// Convert the string to a char array so we can create an ArrayView over it.
			char[] characters = rawInput.ToString().ToCharArray();

			var charView = new ArrayView<char>(characters);

			// token currently being built
			var tokenBuilder = new StringBuilder();
			
			// are we in a quoted string?
			var inQuote = false;
			
			// easy short-hand for ending a text token
			ShellToken EndTextToken()
			{
				var tokenText = tokenBuilder.ToString();
				tokenBuilder.Length = 0;
				return new ShellToken(ShellTokenType.Text, tokenText);
			}
			
			// Keep going until we've ended the string
			while (!charView.EndOfArray)
			{
				switch (charView.Current)
				{
					// Comment, skip until a carriage return or newline
					case '#':
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						while (charView.Next != '\0' && charView.Next != '\n' && charView.Next != '\r')
							charView.Advance();
						
						break;
					}
					
					// Escape sequence
					case '\\':
					{
						charView.Advance();
						if (!charView.EndOfArray)
							tokenBuilder.Append(charView.Current);
						break;
					}
					
					// White-space when not in a quote
					case { } when char.IsWhiteSpace(charView.Current) && !inQuote:
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						break;
					}

					// Quote mark
					case '"':
					{
						inQuote = !inQuote;
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();
						break;
					}
					
					// Sequential operator
					case ';':
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						yield return new ShellToken(ShellTokenType.SequentialExecute, charView.Current.ToString());
						break;
					}
					
					// Parallel operator
					case '&':
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						yield return new ShellToken(ShellTokenType.ParallelExecute, charView.Current.ToString());
						break;
					}
					
					// Pipe
					case '|':
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						yield return new ShellToken(ShellTokenType.Pipe, charView.Current.ToString());
						break;
					}
					
					// Append to File
					case '>' when charView.Next == charView.Current:
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						yield return new ShellToken(ShellTokenType.Append, $"{charView.Current}{charView.Next}");
						charView.Advance();
						break;
					}
					
					// Overwrite File
					case '>':
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						yield return new ShellToken(ShellTokenType.Overwrite, charView.Current.ToString());
						break;
					}
					
					// File Input
					case '<':
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						yield return new ShellToken(ShellTokenType.FileInput, charView.Current.ToString());
						break;
					}

					// ANYTHING else
					default:
					{
						tokenBuilder.Append(charView.Current);
						break;
					}
				}

				charView.Advance();

				if (charView.EndOfArray && tokenBuilder.Length > 0)
					yield return EndTextToken();
			}
		}

		public static IEnumerable<ShellTokenGroup> GroupTokens(IEnumerable<ShellToken> tokens)
		{
			var tokenList = new List<ShellToken>();
			ShellToken? lastToken = null;

			foreach (ShellToken token in tokens)
			{
				if (lastToken != null)
				{
					if (token.TokenType != lastToken.TokenType)
					{
						yield return new ShellTokenGroup(lastToken.TokenType, tokenList);
						tokenList.Clear();
					}
				}
				
				tokenList.Add(token);
				lastToken = token;
			}

			if (tokenList.Count > 0)
			{
				if (lastToken != null)
				{
					yield return new ShellTokenGroup(lastToken.TokenType, tokenList);
					tokenList.Clear();
				}
			}
		}
	}

	public class ShellTokenGroup
	{
		public ShellTokenType GroupType { get; }
		public ShellToken[] Tokens { get; }

		public ShellTokenGroup(ShellTokenType type, IEnumerable<ShellToken> tokenSource)
		{
			this.GroupType = type;
			this.Tokens = tokenSource.ToArray();
		}
	}
	
	public class ShellToken
	{
		public ShellTokenType TokenType { get; set; }
		public string Text { get; }

		public ShellToken(ShellTokenType type, string text)
		{
			this.TokenType = type;
			this.Text = text;
		}
	}

	public enum ShellTokenType
	{
		Text,
		Pipe,
		Overwrite,
		Append,
		FileInput,
		ParallelExecute,
		SequentialExecute
	}
}