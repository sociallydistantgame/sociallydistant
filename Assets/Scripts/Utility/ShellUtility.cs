using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public static IEnumerable<ShellToken> IdentifyTokens(IEnumerable<string> rawTokens)
		{
			foreach (string rawToken in rawTokens)
			{
				yield return rawToken switch
				{
					"|" => new ShellToken(ShellTokenType.Pipe, rawToken),
					">" => new ShellToken(ShellTokenType.Overwrite, rawToken),
					">>" => new ShellToken(ShellTokenType.Append, rawToken),
					"<" => new ShellToken(ShellTokenType.FileInput, rawToken),
					"&" => new ShellToken(ShellTokenType.ParallelExecute, rawToken),
					";" => new ShellToken(ShellTokenType.SequentialExecute, rawToken),
					_ => new ShellToken(ShellTokenType.Text, rawToken)
				};
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