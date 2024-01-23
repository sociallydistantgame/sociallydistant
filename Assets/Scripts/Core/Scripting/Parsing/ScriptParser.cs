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
		private readonly Stack<LanguageContext> languageContextSTack = new Stack<LanguageContext>();

		private LanguageContext CurrentContext => languageContextSTack.Count > 0 ? languageContextSTack.Peek() : LanguageContext.None;
		

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
				ShellInstruction? next = await ParseInstruction(script);
				if (next == null)
					break;
				
				instructions.Add(next);
			}

			return new SequentialInstruction(instructions);

			PopScope();
		}

		private void PushContext(LanguageContext context)
		{
			this.languageContextSTack.Push(context);
		}

		private void PopContext()
		{
			this.languageContextSTack.Pop();
		}

		private void Require(ArrayView<ShellToken> tokenView, ShellTokenType requiredType, string error)
		{
			while (!tokenView.EndOfArray && tokenView.Current.TokenType == ShellTokenType.Newline)
				tokenView.Advance();
			
			if (tokenView.EndOfArray)
				throw new InvalidOperationException(error);

			if (tokenView.Current.TokenType != requiredType)
				throw new InvalidOperationException(error);

			tokenView.Advance();
		}

		private void RequireKeyword(ArrayView<ShellToken> tokenView, string expected)
		{
			while (!tokenView.EndOfArray && tokenView.Current.TokenType == ShellTokenType.Newline)
				tokenView.Advance();
			
			if (tokenView.EndOfArray)
				throw new InvalidOperationException($"Expected {expected} but instead reached end of file");

			if (tokenView.Current.TokenType != ShellTokenType.Text || tokenView.Current.Text != expected)
				throw new InvalidOperationException($"Expected {expected} but instead got {tokenView.Current.Text}");

			tokenView.Advance();
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
		
		private async Task<ShellInstruction?> ParseInstruction(ArrayView<ShellToken> tokenView)
		{
			while (!tokenView.EndOfArray && tokenView.Current.TokenType == ShellTokenType.Newline)
				tokenView.Advance();
			
			if (tokenView.EndOfArray)
				return null;

			if (tokenView.Current.TokenType == ShellTokenType.CloseCurly)
				return null;
			
			if (tokenView.Current.TokenType == ShellTokenType.Text)
			{
				switch (tokenView.Current.Text)
				{
					case "function":
						await ParseFunction(tokenView);
						return await ParseInstruction(tokenView);
					case "if":
						return await ParseIfStatement(tokenView);
					case "while":
						return await ParseWhileLoop(tokenView);
				}
			}

			if (CheckReserved(tokenView))
				return null;
			
			// Special case: You can have functions without the "function" keyword. Because of course you can.
			if (tokenView.Current.TokenType == ShellTokenType.Text && tokenView.Next?.TokenType == ShellTokenType.OpenParen)
			{
				await ParseFunction(tokenView);
				return await ParseInstruction(tokenView);
			}
			
			return await ParseLogicalAnd(tokenView);
		}

		private async Task<ShellInstruction> ParseWhileLoop(ArrayView<ShellToken> tokenView)
		{
			RequireKeyword(tokenView, "while");

			PushContext(LanguageContext.LoopCondition);
			ShellInstruction? condition = await ParseInstruction(tokenView);
			if (condition == null)
				throw new InvalidOperationException("Expected a condition instruction next to the while statement.");
			PopContext();
			
			RequireKeyword(tokenView, "do");
			
			PushContext(LanguageContext.LoopBody);
			PushScope();

			var body = new List<ShellInstruction>();
			while (!tokenView.EndOfArray)
			{
				ShellInstruction? instruction = await ParseInstruction(tokenView);
				if (instruction == null)
					break;
				
				body.Add(instruction);
			}
            
			PopScope();
			PopContext();
			
			RequireKeyword(tokenView, "done");

			return new WhileLoop(condition, body);
		}

		private async Task<ShellInstruction> ParseIfStatement(ArrayView<ShellToken> tokenView)
		{
			RequireKeyword(tokenView, "if");
			PushContext(LanguageContext.IfStatement);
			
			// TODO: Add support for && and || operators.
			PushContext(LanguageContext.IfCondition);
			ShellInstruction? condition = await ParseInstruction(tokenView);
			if (condition == null)
				throw new InvalidOperationException("Expected a condition instruction next to the if statement.");
			PopContext();
			
			RequireKeyword(tokenView, "then");
			
			var mainBody = new List<ShellInstruction>();

			PushScope();
			while (!tokenView.EndOfArray)
			{
				if (tokenView.Current.TokenType == ShellTokenType.Text)
				{
					// Make sure we don't parse the else, elif, or fi keywords as commands.
					if (tokenView.Current.Text == "elif" || tokenView.Current.Text == "else" || tokenView.Current.Text == "fi")
						break;
				}

				ShellInstruction? instruction = await ParseInstruction(tokenView);
				if (instruction == null)
					break;
				
				mainBody.Add(instruction);
			}

			PopScope();

			var branches = new List<ShellInstruction>();
			branches.Add(new BranchInstruction(condition, mainBody));
			while (!tokenView.EndOfArray)
			{
				if (tokenView.Current.TokenType != ShellTokenType.Text)
					break;

				if (tokenView.Current.Text != "elif")
					break;
				
				RequireKeyword(tokenView, "elif");
				PushContext(LanguageContext.ElifStatement);

				PushContext(LanguageContext.IfCondition);
				ShellInstruction? elifCondition = await ParseInstruction(tokenView);
				if (elifCondition == null)
					throw new InvalidOperationException("Expected a condition instruction next to the elif statement.");
				PopContext();
				
				var elifBody = new List<ShellInstruction>();
				
				PushScope();
				while (!tokenView.EndOfArray)
				{
					if (tokenView.Current.TokenType == ShellTokenType.Text)
					{
						// Make sure we don't parse the else, elif, or fi keywords as commands.
						if (tokenView.Current.Text == "elif" || tokenView.Current.Text == "else" || tokenView.Current.Text=="fi")
							break;
					}
					
					ShellInstruction? instruction = await ParseInstruction(tokenView);
					if (instruction == null)
						break;
					
					elifBody.Add(instruction);
				}

				PopScope();
				PopContext();
				
				branches.Add(new BranchInstruction(elifCondition, elifBody));
			}

			var defaultBody = new List<ShellInstruction>();
			
			while (!tokenView.EndOfArray)
			{
				if (tokenView.Current.TokenType != ShellTokenType.Text)
					break;

				if (tokenView.Current.TokenType == ShellTokenType.Text && tokenView.Current.Text == "fi")
					break;
				
				if (tokenView.Current.Text != "else")
					break;
				
				RequireKeyword(tokenView, "else");
				PushContext(LanguageContext.ElseStatement);
				while (!tokenView.EndOfArray)
				{
					if (tokenView.Current.TokenType == ShellTokenType.Text && tokenView.Current.Text == "fi")
						break;

					ShellInstruction instruction = await ParseInstruction(tokenView);
					if (instruction == null)
						break;
					
					defaultBody.Add(instruction);
				}

				PopContext();
			}

			PopContext();
			RequireKeyword(tokenView, "fi");

			return new BranchEvaluator(branches, defaultBody);
		}

		private async Task<ShellInstruction> ParseTest(ArrayView<ShellToken> tokenView)
		{
			Require(tokenView, ShellTokenType.OpenSquare, $"Expected '['");

			// Test expressions are just syntactic sugar for running the 'test' built-in command. 
			var commandName = "test";

			var argumentList = new List<IArgumentEvaluator>();
			while (!tokenView.EndOfArray)
			{
				IArgumentEvaluator? argumentEvaluator = await ParseArgument(tokenView);
				if (argumentEvaluator == null)
					break;
				
				argumentList.Add(argumentEvaluator);
			}
			
			Require(tokenView, ShellTokenType.ClosedSquare, $"Expected '['");

			var commandData = new CommandData(CurrentScope, commandName, argumentList, FileRedirectionType.None, string.Empty);

			return new SingleInstruction(commandData);
		}
		
		private async Task ParseFunction(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.Current.TokenType != ShellTokenType.Text)
				return;

			if (tokenView.Current.Text != "function")
				return;

			tokenView.Advance();

			if (tokenView.EndOfArray || tokenView.Current.TokenType != ShellTokenType.Text)
				throw new InvalidOperationException("Expected name of function");

			string functionName = tokenView.Current.Text;

			tokenView.Advance();

			Require(tokenView, ShellTokenType.OpenParen, "'(' expected");
			Require(tokenView, ShellTokenType.CloseParen, "')' expected");

			ShellInstruction block = await ParseBlock(tokenView);

			CurrentScope.DeclareFunction(functionName, block);
		}
		
		private async Task<ShellInstruction> ParseBlock(ArrayView<ShellToken> tokenView)
		{
			Require(tokenView, ShellTokenType.OpenCurly, "'{' expected");

			PushScope();

			var instructionList = new List<ShellInstruction>();

			while (!tokenView.EndOfArray)
			{
				if (tokenView.Current.TokenType == ShellTokenType.CloseCurly)
					break;

				if (tokenView.Current.TokenType == ShellTokenType.SequentialExecute)
				{
					tokenView.Advance();
					continue;
				}

				ShellInstruction instruction = await ParseInstruction(tokenView);
				
				instructionList.Add(instruction);
			}
			
			Require(tokenView, ShellTokenType.CloseCurly, "'}' expected");
			PopScope();

			// Skip any newlines after the block
			while (!tokenView.EndOfArray)
			{
				if (tokenView.Current.TokenType != ShellTokenType.SequentialExecute)
					break;
					
				tokenView.Advance();
			}
			
			return new SequentialInstruction(instructionList);
		}

		private async Task<ShellInstruction?> ParseGroup(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.EndOfArray)
				return null;

			if (tokenView.Current.TokenType != ShellTokenType.OpenParen)
				return await ParseParallelInstruction(tokenView);

			tokenView.Advance();

			ShellInstruction? groupedInstruction = await ParseInstruction(tokenView);
			
			Require(tokenView, ShellTokenType.CloseParen, "')' expected");

			return groupedInstruction;
		}

		private async Task<ShellInstruction?> ParseLogicalAnd(ArrayView<ShellToken> tokenView)
		{
			ShellInstruction? left = await ParseLogicalOr(tokenView);

			if (tokenView.EndOfArray)
				return left;

			if (tokenView.Current.TokenType != ShellTokenType.LogicalAnd)
				return left;

			tokenView.Advance();

			ShellInstruction? right = await ParseInstruction(tokenView);
			if (right == null)
				throw new InvalidOperationException("Unexpected end of file");

			return new LogicalAndInstruction(left, right);
		}

		private async Task<ShellInstruction?> ParseLogicalOr(ArrayView<ShellToken> tokenView)
		{
			ShellInstruction? left = await ParseGroup(tokenView);

			if (tokenView.EndOfArray)
				return left;

			if (tokenView.Current.TokenType != ShellTokenType.LogicalOr)
				return left;

			tokenView.Advance();

			ShellInstruction? right = await ParseInstruction(tokenView);
			if (right == null)
				throw new InvalidOperationException("Unexpected end of file");

			return new LogicalOrInstruction(left, right);
		}
		
		private async Task<ShellInstruction> ParseParallelInstruction(ArrayView<ShellToken> tokenView)
		{
			// Parse sequential command list
			ShellInstruction leftSide = await ParseCommandList(tokenView);

			if (tokenView.EndOfArray)
				return leftSide;

			if (tokenView.Current.TokenType != ShellTokenType.ParallelExecute)
				return leftSide;

			tokenView.Advance();

			ShellInstruction? rightSide = await ParseInstruction(tokenView);
			if (rightSide == null)
				return leftSide;
			
			return new ParallelInstruction(leftSide, rightSide);
		}
		
		private async Task<ShellInstruction> ParseCommandList(ArrayView<ShellToken> tokenView)
		{
			var instructionList = new List<ShellInstruction>();
			
			// Parse a pipe sequence
			ShellInstruction pipeSequence = await ParsePipeSequence(tokenView);
			instructionList.Add(pipeSequence);
			
			while (!tokenView.EndOfArray)
			{
				if (tokenView.Current.TokenType != ShellTokenType.SequentialExecute)
					break;

				tokenView.Advance();
				ShellInstruction? nextPipe = await ParsePipeSequence(tokenView);
				if (nextPipe == null)
					break;
				
				instructionList.Add(nextPipe);
			}
			
			return new SequentialInstruction(instructionList);
		}
		
		private async Task<ShellInstruction?> ParsePipeSequence(ArrayView<ShellToken> tokenView)
		{
			ShellInstruction? commandInstruction = await ParseSingleInstruction(tokenView);
			if (commandInstruction == null)
				return null;

			if (tokenView.EndOfArray)
				return commandInstruction;
			
			if (tokenView.Current.TokenType != ShellTokenType.Pipe)
				return commandInstruction;

			tokenView.Advance();

			ShellInstruction? pipeDestination = await ParsePipeSequence(tokenView);
			if (pipeDestination == null)
				return commandInstruction;

			return new PipeInstruction(commandInstruction, pipeDestination);
		}
		
		private async Task<ShellInstruction?> ParseSingleInstruction(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.EndOfArray)
				return null;
			
			// If we get an OpenSquare token, then we interpret it as an expression test.
			// See https://man.sociallydistantgame.com/index.php/Shell_Expression_Tests
			if (tokenView.Current.TokenType == ShellTokenType.OpenSquare)
				return await ParseTest(tokenView);
			
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
			if (tokenView.Current.TokenType != ShellTokenType.Text)
				throw new InvalidOperationException($"Syntax error near unexpected token '{tokenView.Current.Text}'");
			
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

			if (CheckReserved(tokenView))
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

		private bool CheckReserved(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.EndOfArray)
				return false;
			
			var willThrow = false;
			
			if (tokenView.Current.TokenType == ShellTokenType.Text)
			{
				switch (tokenView.Current.Text)
				{
					case "do":
						if (CurrentContext == LanguageContext.LoopCondition)
							return true;

						willThrow = true;
						break;
					case "done":
						if (CurrentContext == LanguageContext.LoopBody)
							return true;

						willThrow = true;
						break;
					case "then":
						if (CurrentContext == LanguageContext.IfCondition)
							return true;

						willThrow = true;
						break;
					case "if" when CurrentContext == LanguageContext.IfCondition || CurrentContext == LanguageContext.LoopCondition:
						willThrow = true;
						break;
					case "elif" when CurrentContext == LanguageContext.ElseStatement:
						willThrow = true;
						break;
					case "else":
					case "elif":
					case "fi":
						if (CurrentContext == LanguageContext.IfStatement
						    || CurrentContext == LanguageContext.ElifStatement
						    || CurrentContext == LanguageContext.ElseStatement)
							return true;

						willThrow = true;
						break;
					case "function" when CurrentContext == LanguageContext.IfCondition || CurrentContext == LanguageContext.LoopCondition:
						willThrow = true;
						break;
				}
			}
			
			if (willThrow)
				throw new InvalidOperationException($"syntax error near unexpected token '{tokenView.Current.Text}'");

			return false;
		}
		
		private enum LanguageContext
		{
			None,
			IfCondition,
			LoopCondition,
			LoopBody,
			IfStatement,
			ElifStatement,
			ElseStatement
		}
	}
}
