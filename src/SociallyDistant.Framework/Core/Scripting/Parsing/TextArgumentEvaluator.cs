﻿#nullable enable
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting.Parsing
{
	public class TextArgumentEvaluator : IArgumentEvaluator
	{
		private readonly string text;

		public string Text => text;
		
		public TextArgumentEvaluator(string text)
		{
			this.text = text;
		}
		
		/// <inheritdoc />
		public Task<string>GetArgumentText(IScriptExecutionContext context, ITextConsole console)
		{
			// We add a trailing space for shell builtins to use, but the shell will automatically trim this.
			return Task.FromResult(text);
		}

		public static implicit operator TextArgumentEvaluator(string text)
		{
			return new TextArgumentEvaluator(text);
		}
	}
}