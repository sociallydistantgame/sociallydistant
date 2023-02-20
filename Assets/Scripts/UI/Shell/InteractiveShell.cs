#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Architecture;
using Codice.Client.Common;
using Newtonsoft.Json.Serialization;
using OS.Devices;
using PlasticPipe.PlasticProtocol.Client;
using UnityEngine;
using Utility;

namespace UI.Shell
{
	public class InteractiveShell :
		ITerminalProcessController
	{
		private ISystemProcess process;
		private ITextConsole consoleDevice;
		private bool initialized;
		private MonoBehaviour unityBehaviour;
		private ShellState shellState;
		private string nextCommand;
		private readonly StringBuilder lineBuilder = new StringBuilder();
		private readonly List<string> tokenList = new List<string>();
		private readonly Queue<ShellInstruction> pendingInstructions = new Queue<ShellInstruction>();
		private ShellInstruction? currentInstruction = null;

		/// <inheritdoc />
		public bool IsExecutionHalted => false;

		public InteractiveShell(MonoBehaviour unityBehaviour)
		{
			this.unityBehaviour = unityBehaviour;
		}

		/// <inheritdoc />
		public void Setup(ISystemProcess process, ITextConsole consoleDevice)
		{
			this.process = process;
			this.consoleDevice = consoleDevice;
			initialized = true;
		}

		/// <inheritdoc />
		public void Update()
		{
			if (!initialized)
				return;

			switch (shellState)
			{
				case ShellState.Init:
				{
					WritePrompt();
					lineBuilder.Length = 0;
					shellState = shellState = ShellState.Reading;
					break;
				}
				case ShellState.Reading:
				{
					if (consoleDevice.TryDequeueSubmittedInput(out string nextLine))
					{
						lineBuilder.Append(nextLine);
						shellState = ShellState.Processing;
					}
					break;
				}
				case ShellState.Processing:
				{
					// Clear the token list
					tokenList.Clear();
					
					// Tokenize the current input.
					bool shouldExecute = ShellUtility.SimpleTokenize(lineBuilder, tokenList);

					if (!shouldExecute)
					{
						lineBuilder.AppendLine();
						consoleDevice.WriteText(" --> ");
						shellState = ShellState.Reading;
					}
					else
					{
						ProcessTokens();
						
						shellState = ShellState.Executing;
					}
					
					break;
				}
				case ShellState.Executing:
				{
					if (currentInstruction != null)
					{
						currentInstruction.Update();
						if (!currentInstruction.IsComplete)
							return;

						currentInstruction = null;
					}

					if (pendingInstructions.Count > 0)
					{
						currentInstruction = pendingInstructions.Dequeue();
						currentInstruction.Begin(this.process, this.consoleDevice);
						return;
					}
					
					shellState = ShellState.Init;
					break;
				}
			}
		}

		private void ProcessTokens()
		{
			// The first tokenization pass we do, before executing this function,
			// only deals with quotes and escape sequences. It also ignores comments,
			// but it has no concept of the actual syntax of the shell language.
			//
			// As such, we must do a more advanced tokenization of the raw input.
			
			// So let's do it.
			IEnumerable<ShellToken>? typedTokens = ShellUtility.IdentifyTokens(this.lineBuilder);
			
			// Create a view over this array that we can advance during parsing
			var view = new ArrayView<ShellToken>(typedTokens.ToArray());

			// It's time to parse.
			while (!view.EndOfArray)
			{
				this.pendingInstructions.Enqueue(ParseInstruction(view));
			}
		}

		private ShellInstruction ParseInstruction(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.Current.TokenType != ShellTokenType.Text)
				throw new InvalidOperationException("Instructions must start with text.");

			return ParseParallelInstruction(tokenView);
		}

		private ShellInstruction ParseParallelInstruction(ArrayView<ShellToken> tokenView)
		{
			// Parse sequential command list
			var commandSequence = ParseCommandList(tokenView);

			// End of command-line, no parallel executions
			if (tokenView.EndOfArray)
				return commandSequence;
			
			// This should never execute
			if (tokenView.Current.TokenType != ShellTokenType.ParallelExecute)
				return commandSequence;

			tokenView.Advance();
			
			// Parse another instruction
			ShellInstruction nextInstruction = ParseInstruction(tokenView);

			return new ParallelInstruction(commandSequence, nextInstruction);
		}

		private bool ProcessBuiltIn(ISystemProcess process, ITextConsole console, string name, string[] arguments)
		{
			switch (name)
			{
				case "clear":
					console.ClearScreen();
					break;
				case "echo":
				{
					string text = string.Join(" ", arguments);
					console.WriteText(text + Environment.NewLine);
					break;
				}
				case "exit":
					process.Kill();
					break;
				default:
					return false;
			}

			return true;
		}

		private ShellInstruction ParseCommandList(ArrayView<ShellToken> tokenView)
		{
			// Parse a pipe sequence
			ShellInstruction pipeSequence = ParsePipeSequence(tokenView);

			// No more tokens
			if (tokenView.EndOfArray)
				return pipeSequence;

			var instructionList = new List<ShellInstruction>();
			instructionList.Add(pipeSequence);

			while (!tokenView.EndOfArray && tokenView.Current.TokenType == ShellTokenType.SequentialExecute)
			{
				tokenView.Advance();
				instructionList.Add(ParsePipeSequence(tokenView));
			}

			return new SequentialInstruction(instructionList);
		}

		private ShellInstruction ParsePipeSequence(ArrayView<ShellToken> tokenView)
		{
			ShellInstruction commandInstruction = ParseSingleInstruction(tokenView);

			if (tokenView.EndOfArray)
				return commandInstruction;

			if (tokenView.Current.TokenType != ShellTokenType.Pipe)
				return commandInstruction;

			tokenView.Advance();

			ShellInstruction pipeDestination = ParsePipeSequence(tokenView);

			return new PipeInstruction(commandInstruction, pipeDestination);
		}

		private ShellInstruction ParseSingleInstruction(ArrayView<ShellToken> tokenView)
		{
			CommandData command = ParseCommandWithArguments(tokenView);
			return new SingleInstruction(command);
		}
		
		private CommandData ParseCommandWithArguments(ArrayView<ShellToken> tokenView)
		{
			string name = tokenView.Current.Text;
			var arguments = new List<string>();

			tokenView.Advance();
			
			while (!tokenView.EndOfArray && tokenView.Current.TokenType == ShellTokenType.Text)
			{
				arguments.Add(tokenView.Current.Text);
				tokenView.Advance();
			}
			
			// If we have tokens remaining, we will need to check for redirection markers.
			return ParseRedirection(tokenView, name, arguments);
		}

		private CommandData ParseRedirection(ArrayView<ShellToken> tokenView, string name, List<string> arguments)
		{
			// No tokens left.
			if (tokenView.Next == null)
				return new CommandData(this, name, arguments, FileRedirectionType.None, string.Empty);

			FileRedirectionType redirectionType = GetRedirectionTypeAndPath(tokenView, out string filePath);
			return new CommandData(this, name, arguments, redirectionType, filePath);
		}

		private FileRedirectionType GetRedirectionTypeAndPath(ArrayView<ShellToken> tokenView, out string filePath)
		{
			filePath = string.Empty;

			if (tokenView.Next == null)
				return FileRedirectionType.None;

			FileRedirectionType redirectionType = tokenView.Next.TokenType switch
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

			while (tokenView.Current.TokenType == expectedType)
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
		
		private void WritePrompt()
		{
			consoleDevice.WriteText($"{process.User.UserName}@{process.User.Computer.Name}:/$ ");
		}

		private enum ShellState
		{
			Init,
			Reading,
			Processing,
			Executing
		}

		public enum FileRedirectionType
		{
			None,
			Overwrite,
			Append,
			Input
		}
		
		public class CommandData
		{
			private readonly InteractiveShell shell;
			
			public string Name { get; }
			public string[] ArgumentList { get; }
			public FileRedirectionType RedirectionType { get; }
			public string FilePath { get; }
			
			public CommandData(InteractiveShell shell,string name, IEnumerable<string> argumentSource, FileRedirectionType redirectionType, string path)
			{
				this.shell = shell;
				
				Name = name;
				ArgumentList = argumentSource.ToArray();
				this.RedirectionType = redirectionType;
				this.FilePath = path;
			}
			
			public ISystemProcess? SpawnCommandProcess(ISystemProcess shellProcess, ITextConsole console)
			{
				// TODO: File redirection. Should be easy.
				
				if (shell.ProcessBuiltIn(shellProcess, console, Name, ArgumentList))
					return null;
				
				console.WriteText($"sh: {Name}: command not found");
				return null;
			}
		}

		public abstract class ShellInstruction
		{
			public abstract bool IsComplete { get; }

			public abstract void Update();
			public abstract void Begin(ISystemProcess process, ITextConsole consoleDevice);
		}

		public sealed class ParallelInstruction : ShellInstruction
		{
			private readonly ShellInstruction first;
			private readonly ShellInstruction next;

			public ParallelInstruction(ShellInstruction first, ShellInstruction next)
			{
				this.first = first;
				this.next = next;
			}

			/// <inheritdoc />
			public override bool IsComplete =>
				first.IsComplete
				&& next.IsComplete;

			/// <inheritdoc />
			public override void Begin(ISystemProcess process, ITextConsole consoleDevice)
			{
				first.Begin(process, consoleDevice);
				next.Begin(process, consoleDevice);
			}

			/// <inheritdoc />
			public override void Update()
			{
				first.Update();
				next.Update();
			}
		}

		public sealed class PipeInstruction : ShellInstruction
		{
			private readonly ShellInstruction pipeIn;
			private readonly ShellInstruction pipeOut;

			private ISystemProcess shellProcess;
			private ITextConsole? pipeInConsole;
			private ITextConsole? pipeOutConsole;
			private bool hasInputStarted;
			private bool hasOutputStarted;
			
			
			public PipeInstruction(ShellInstruction pipeIn, ShellInstruction pipeOut)
			{
				this.pipeIn = pipeIn;
				this.pipeOut = pipeOut;
			}

			public override bool IsComplete =>
				pipeIn.IsComplete
				&& pipeOut.IsComplete;

			/// <inheritdoc />
			public override void Begin(ISystemProcess process, ITextConsole consoleDevice)
			{
				shellProcess = process;
				
				// Pipe instructions are weird.
				//
				// We need to do something special for them.
				// 
				// If we're the first pipe in a pipe sequence, we were likely given the same
				// console device as the shell.
				//
				// If the above is the case, we need to create the sequence of pipe consoles
				// that will be used for the rest of the pipe. How we do this depends on
				// whether our output is another pipe.
				//
				// If we're just outputting into a single command, we create two pipe consoles
				// One that takes input from the REAL console, and outputs to a line list.
				// The other console reads from the line list and writes to the real console.
				//
				// If we're outputting to another pipe, we need to have both our own consoles
				// output to a different line list, and give the second console we create as well
				// as the real console to the output pipe. It's confusing as fuck.
				// 
				// Also, disregard all of this text if we already have the two consoles we need...
				if (pipeInConsole == null && pipeOutConsole == null)
				{
					// Create the first line list
					var lineList = new LineListConsole();
					
					// outputting to another pipe
					if (pipeOut is PipeInstruction pipeInstruction)
					{
						// first command goes to our line list
						pipeInConsole = new RedirectedConsole(consoleDevice, lineList);
						
						// second command goes to whatever our output pipe says.
						pipeOutConsole = pipeInstruction.CreateSlavePipe(lineList, consoleDevice);
					}

					// anything else
					else
					{
						// redirect first command output into the line list
						pipeInConsole = new RedirectedConsole(consoleDevice, lineList);
						
						// redirect second command input to the line list
						pipeOutConsole = new RedirectedConsole(lineList, consoleDevice);
					}
				}
			}

			private ITextConsole CreateSlavePipe(ITextConsole input, ITextConsole masterOutput)
			{
				// Create a line list to send input to
				var newLineList = new LineListConsole();
				pipeInConsole = new RedirectedConsole(input, newLineList);

				if (pipeOut is PipeInstruction pipeInstruction)
				{
					pipeOutConsole = pipeInstruction.CreateSlavePipe(newLineList, masterOutput);
				}
				else
				{
					pipeOutConsole = new RedirectedConsole(newLineList, masterOutput);
				}

				return pipeInConsole;
			}
			
			/// <inheritdoc />
			public override void Update()
			{
				if (pipeInConsole == null)
					return;

				if (pipeOutConsole == null)
					return;
				
				if (!hasInputStarted)
				{
					pipeIn.Begin(shellProcess, pipeInConsole);
					hasInputStarted = true;
				}

				if (hasInputStarted)
					pipeIn.Update();

				if (pipeIn.IsComplete && !hasOutputStarted)
				{
					hasOutputStarted = true;
					pipeOut.Begin(shellProcess, pipeOutConsole);
				}

				if (hasOutputStarted)
					pipeOut.Update();
			}
		}

		public sealed class SequentialInstruction : ShellInstruction
		{
			private readonly ShellInstruction[] instructions;
			private int currentInstruction;
			private bool hasStarted;
			private ISystemProcess shellProcess;
			private ITextConsole console;

			public SequentialInstruction(IEnumerable<ShellInstruction> instructionSource)
			{
				instructions = instructionSource.ToArray();
			}

			/// <inheritdoc />
			public override bool IsComplete => instructions.All(x => x.IsComplete);

			/// <inheritdoc />
			public override void Begin(ISystemProcess process, ITextConsole consoleDevice)
			{
				this.shellProcess = process;
				this.console = consoleDevice;
				this.currentInstruction = 0;
			}

			/// <inheritdoc />
			public override void Update()
			{
				if (currentInstruction >= instructions.Length)
					return;

				if (!hasStarted)
				{
					instructions[currentInstruction].Begin(shellProcess, console);
					hasStarted = true;
				}

				instructions[currentInstruction].Update();
				if (instructions[currentInstruction].IsComplete)
				{
					currentInstruction++;
					hasStarted = false;
				}
			}
		}
		
		public sealed class SingleInstruction : ShellInstruction
		{
			private readonly CommandData command;
			private ISystemProcess shellProcess;
			private ISystemProcess? currentCommandProcess;
			private ITextConsole console;
			private bool waiting;
			private bool isCompleted;
			
			public SingleInstruction(CommandData command)
			{
				this.command = command;
			}

			/// <inheritdoc />
			public override bool IsComplete => isCompleted;

			/// <inheritdoc />
			public override void Begin(ISystemProcess process, ITextConsole consoleDevice)
			{
				this.console = consoleDevice;
				this.shellProcess = process;
				this.waiting = false;
				this.isCompleted = false;
			}

			/// <inheritdoc />
			public override void Update()
			{
				if (waiting)
					return;

				this.currentCommandProcess = command.SpawnCommandProcess(shellProcess, console);
				if (this.currentCommandProcess == null)
				{
					isCompleted = true;
					return;
				}

				waiting = true;
				this.currentCommandProcess.Killed += HandleCommandProcessKilled;
			}

			private void HandleCommandProcessKilled(ISystemProcess process)
			{
				waiting = false;
				process.Killed -= HandleCommandProcessKilled;
				this.currentCommandProcess = null;
				isCompleted = true;
			}
		}
	}

	public class LineListConsole : ITextConsole
	{
		private readonly StringBuilder buffer = new StringBuilder();
		private int readPosition = 0;
		
		/// <inheritdoc />
		public void ClearScreen()
		{
			buffer.Length = 0;
			readPosition = 0;
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
			buffer.Append(text);
		}

		/// <inheritdoc />
		public bool TryDequeueSubmittedInput(out string input)
		{
			input = string.Empty;

			if (readPosition == buffer.Length)
				return false;

			input = ReadUntil('\n');
			return true;
		}

		private string ReadUntil(char character)
		{
			var sb = new StringBuilder();

			while (readPosition < buffer.Length)
			{
				char current = buffer[readPosition];
				if (character == current)
					break;

				sb.Append(current);
				readPosition++;
			}
			
			return sb.ToString();
		}
	}

	public class RedirectedConsole : ITextConsole
	{
		private readonly ITextConsole input;
		private readonly ITextConsole output;

		public RedirectedConsole(ITextConsole input, ITextConsole output)
		{
			this.input = input;
			this.output = output;
		}

		/// <inheritdoc />
		public void ClearScreen()
		{
			output.ClearScreen();
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
			output.WriteText(text);
		}

		/// <inheritdoc />
		public bool TryDequeueSubmittedInput(out string input)
		{
			return this.input.TryDequeueSubmittedInput(out input);
		}
	}
} 