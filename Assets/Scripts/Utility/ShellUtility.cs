using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Architecture;
using TMPro;

namespace Utility
{
	public static class ShellUtility
	{
		public static void TrimTrailingSpaces(ref string[] args)
		{
			for (var i = 0; i < args.Length; i++)
			{
				string arg = args[i];
				if (arg.EndsWith(" "))
				{
					arg = arg.Substring(0, arg.Length - 1);
					args[i] = arg;
				}
			}
		}
		
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

		public static bool IsIdentifier(this string text)
		{
			// TODO: More performant way of handling this?
			return text.All(IsIdentifier);
		}
		
		public static bool IsIdentifier(char character)
		{
			return character == '_'
			       || char.IsLetterOrDigit(character);
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

			var quoteStart = '\0';
			
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
						{
							switch (charView.Current)
							{
								case 'r':
									tokenBuilder.Append('\r');
									break;
								case 'n':
									tokenBuilder.Append('\n');
									break;
								case 'a':
									tokenBuilder.Append('\a');
									break;
								case 'e':
									tokenBuilder.Append('\x1b');
									break;
								case 'b':
									tokenBuilder.Append('\b');
									break;
								case 'v':
									tokenBuilder.Append('\v');
									break;
								case 't':
									tokenBuilder.Append('\t');
									break;
								default:
									tokenBuilder.Append(charView.Current);
									break;
							}
						}

						break;
					}
					
					// White-space when not in a quote
					case { } when char.IsWhiteSpace(charView.Current) && !inQuote:
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						break;
					}

					// String literal end
					case { } when
						charView.Current == quoteStart 
						&& inQuote:
					{
						quoteStart = '\0';
						inQuote = false;
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();
						break;
					}

					// String literal start
					case '"' when !inQuote:
					case '\'' when !inQuote:
					{
						quoteStart = charView.Current;
						inQuote = true;
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
					
					// Assignment
					case '=':
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						yield return new ShellToken(ShellTokenType.AssignmentOperator, charView.Current.ToString());
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
					
					// Variable identifiers when:
					//	- we're not in a string literal at all, or
					//	- we're in a double-quote literal, and
					//	- the next character is an identifier char, or
					//	- the next character is an opening curly brace
					case '$' when
						IsIdentifier(charView.Next)
						|| charView.Next == '{'
						&& (!inQuote || quoteStart != '\''):
					{
						if (tokenBuilder.Length > 0)
							yield return EndTextToken();

						// if next char is an opening curly brace, advance one char and check for an identifier
						if (charView.Next == '{')
						{
							charView.Advance();
							if (!IsIdentifier(charView.Next))
								throw new InvalidOperationException("Identifier expected");
						}
						
						do
						{
							charView.Advance();
							tokenBuilder.Append(charView.Current);
						} while (IsIdentifier(charView.Next));

						// if next char is a closing curly, skip
						if (charView.Next == '}')
							charView.Advance();
						
						yield return new ShellToken(ShellTokenType.VariableAccess, tokenBuilder.ToString());
						tokenBuilder.Length = 0;
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
		SequentialExecute,
		VariableAccess,
		AssignmentOperator
	}
}