#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using Core.Scripting.Instructions;
using UI.Shell;

namespace Core.Scripting.Parsing
{
	public class ScriptParser
	{
		private readonly IScriptExecutionContext context;
		private readonly Stack<LocalScriptExecutionContext> scopeStack = new Stack<LocalScriptExecutionContext>();

		public LocalScriptExecutionContext CurrentScope => scopeStack.Peek();

		public ScriptParser(IScriptExecutionContext context)
		{
			this.context = context;
		}
		
		public async Task<ShellInstruction> ParseScript(ArrayView<ShellToken> script)
		{
			PushScope();
			
			var instructions = new List<ShellInstruction>();
			
			while (!script.EndOfArray)
			{
				instructions.Add(await ParseInstruction(script));
			}

			return new SequentialInstruction(instructions);

			PopScope();
		}

		private void PushScope()
		{
			if (scopeStack.Count == 0)
			{
				scopeStack.Push(new LocalScriptExecutionContext(this.context));
				return;
			}
			
			scopeStack.Push(new LocalScriptExecutionContext(CurrentScope));
		}

		private void PopScope()
		{
			scopeStack.Pop();
		}
		
		private async Task<ShellInstruction> ParseInstruction(ArrayView<ShellToken> tokenView)
		{
			while (tokenView.Current.TokenType == ShellTokenType.SequentialExecute)
				tokenView.Advance();
			
			if (tokenView.Current.TokenType != ShellTokenType.Text)
				throw new InvalidOperationException("Instructions must start with text.");

			return await ParseParallelInstruction(tokenView);
		}

		private async Task<ShellInstruction> ParseParallelInstruction(ArrayView<ShellToken> tokenView)
		{
			// Parse sequential command list
			var commandSequence = await ParseCommandList(tokenView);

			// End of command-line, no parallel executions
			if (tokenView.EndOfArray)
				return commandSequence;

			// This should never execute
			if (tokenView.Current.TokenType != ShellTokenType.ParallelExecute)
				return commandSequence;

			tokenView.Advance();

			// Parse another instruction
			ShellInstruction nextInstruction = await ParseInstruction(tokenView);

			return new ParallelInstruction(commandSequence, nextInstruction);
		}
		
		private async Task<ShellInstruction> ParseCommandList(ArrayView<ShellToken> tokenView)
		{
			// Parse a pipe sequence
			ShellInstruction pipeSequence = await ParsePipeSequence(tokenView);

			// No more tokens
			if (tokenView.EndOfArray)
				return pipeSequence;

			var instructionList = new List<ShellInstruction>();
			instructionList.Add(pipeSequence);
			while (!tokenView.EndOfArray && tokenView.Current.TokenType == ShellTokenType.SequentialExecute)
			{
				// This cursed-ass nested while loop is because of newlines.
				while (!tokenView.EndOfArray && tokenView.Current.TokenType == ShellTokenType.SequentialExecute)
					tokenView.Advance();

				if (!tokenView.EndOfArray)
					instructionList.Add(await ParsePipeSequence(tokenView));
			}

			return new SequentialInstruction(instructionList);
		}
		
		private async Task<ShellInstruction> ParsePipeSequence(ArrayView<ShellToken> tokenView)
		{
			ShellInstruction commandInstruction = await ParseSingleInstruction(tokenView);

			if (tokenView.EndOfArray)
				return commandInstruction;

			if (tokenView.Current.TokenType != ShellTokenType.Pipe)
				return commandInstruction;

			tokenView.Advance();

			ShellInstruction pipeDestination = await ParsePipeSequence(tokenView);

			return new PipeInstruction(commandInstruction, pipeDestination);
		}
		
		private async Task<ShellInstruction> ParseSingleInstruction(ArrayView<ShellToken> tokenView)
		{
			// We first try to parse a variable assignment instruction. If that fails, fall back to parsing a command.
			ShellInstruction? assignment = await ParseVariableAssignment(tokenView);
			if (assignment != null)
				return assignment;
			
			CommandData command = await ParseCommandWithArguments(tokenView);
			return new SingleInstruction(command);
		}
		
		private async Task<ShellInstruction?> ParseVariableAssignment(ArrayView<ShellToken> tokenView)
		{
			// End of array? Parse nothing
			if (tokenView.EndOfArray)
				return null;
			
			// Only parse assignments if the current token is either Text or VariableAccess,
			// and is an identifier.
			if (tokenView.Current.TokenType != ShellTokenType.Text && tokenView.Current.TokenType != ShellTokenType.VariableAccess)
				return null;

			if (!tokenView.Current.Text.IsIdentifier())
				return null;
			
			// If there isn't a next token, parse nothing
			if (tokenView.Next == null)
				return null;
			
			// Only parse assignments if the assignment operator is present as the next token
			if (tokenView.Next.TokenType != ShellTokenType.AssignmentOperator)
				return null;
			
			// get identifier
			string identifier = tokenView.Current.Text;
			tokenView.Advance();
			
			// Skip the assignment operator
			tokenView.Advance();
			
			// We must parse at least one argument evaluator for the assignment instruction
			// to be considered valid. If we get a null, then we must go back two elements before
			// returning. Otherwise we can't fallback to parsing a command instruction.
			var argumentList = new List<IArgumentEvaluator>();

			IArgumentEvaluator? nextEvaluator = await ParseArgument(tokenView);
			
			while (nextEvaluator != null)
			{
				argumentList.Add(nextEvaluator);
				nextEvaluator = await ParseArgument(tokenView);
			}

			if (argumentList.Count == 0)
			{
				tokenView.Previous();
				tokenView.Previous();
				return null;
			}

			return new AssignmentInstruction(CurrentScope, identifier, argumentList);
		}
		
		private async Task<CommandData> ParseCommandWithArguments(ArrayView<ShellToken> tokenView)
		{
			string name = tokenView.Current.Text;
			var arguments = new List<IArgumentEvaluator>();

			tokenView.Advance();

			IArgumentEvaluator? nextEvaulator = await ParseArgument(tokenView);
			while (nextEvaulator != null)
			{
				arguments.Add(nextEvaulator);
				nextEvaulator = await ParseArgument(tokenView);
			}

			// If we have tokens remaining, we will need to check for redirection markers.
			return await ParseRedirection(tokenView, name, arguments);
		}
		
		private async Task<IArgumentEvaluator?> ParseArgument(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.EndOfArray)
				return null;

			return tokenView.Current.TokenType switch
			{
				ShellTokenType.VariableAccess => await ParseVariableAccess(tokenView),
				ShellTokenType.Text => await ParseTextArgument(tokenView),
				_ => null
			};
		}
		
		private async Task<IArgumentEvaluator?> ParseVariableAccess(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.EndOfArray || tokenView.Current.TokenType != ShellTokenType.VariableAccess)
				return null;

			string text = tokenView.Current.Text;
			tokenView.Advance();

			return new VariableAccessEvaluator(text);
		}
		
		private async Task<IArgumentEvaluator?> ParseTextArgument(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.EndOfArray || tokenView.Current.TokenType != ShellTokenType.Text)
				return null;

			string text = tokenView.Current.Text;
			tokenView.Advance();
			return new TextArgumentEvaluator(text);
		}
		
		private async Task<CommandData> ParseRedirection(ArrayView<ShellToken> tokenView, string name, List<IArgumentEvaluator> arguments)
		{
			// No tokens left.
			if (tokenView.Next == null)
				return new CommandData(CurrentScope, name, arguments, FileRedirectionType.None, string.Empty);

			FileRedirectionType redirectionType = GetRedirectionTypeAndPath(tokenView, out string filePath);
			return new CommandData(CurrentScope, name, arguments, redirectionType, filePath);
		}
		
		private FileRedirectionType GetRedirectionTypeAndPath(ArrayView<ShellToken> tokenView, out string filePath)
		{
			filePath = string.Empty;

			if (tokenView.EndOfArray)
				return FileRedirectionType.None;

			FileRedirectionType redirectionType = tokenView.Current.TokenType switch
			{
				ShellTokenType.Append => FileRedirectionType.Append,
				ShellTokenType.Overwrite => FileRedirectionType.Overwrite,
				ShellTokenType.FileInput => FileRedirectionType.Input,
				_ => FileRedirectionType.None
			};
			
			// if we've identified a redirection type other than none, read the file path
			if (redirectionType != FileRedirectionType.None)
			{
				tokenView.Advance();
				filePath = MustReadMany(tokenView, ShellTokenType.Text);
			}

			return redirectionType;
		}
		
		private string MustReadMany(ArrayView<ShellToken> tokenView, ShellTokenType expectedType)
		{
			if (tokenView.Current.TokenType != expectedType)
				throw new InvalidOperationException($"Expected at least one {expectedType} token");

			var sb = new StringBuilder();

			while (!tokenView.EndOfArray && tokenView.Current.TokenType == expectedType)
			{
				sb.Append(tokenView.Current.Text);
				sb.Append(" ");
				tokenView.Advance();
			}

			// Removes the trailing space
			if (sb.Length > 0)
				sb.Length--;

			return sb.ToString();
		}
	}
}