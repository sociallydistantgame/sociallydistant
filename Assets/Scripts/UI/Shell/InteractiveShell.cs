#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using Core;
using OS.Devices;
using OS.FileSystems;
using OS.FileSystems.Host;
using UnityEngine;
using Utility;

namespace UI.Shell
{
	public class InteractiveShell :
		ITerminalProcessController,
		ICommandShell,
		IAutoCompleteSource
	{
		private ISystemProcess? process;
		private ITextConsole? consoleDevice;
		private bool initialized;
		private MonoBehaviour unityBehaviour;
		private ShellState shellState;
		private readonly StringBuilder lineBuilder = new StringBuilder();
		private readonly List<string> tokenList = new List<string>();
		private readonly Queue<ShellInstruction> pendingInstructions = new Queue<ShellInstruction>();
		private ShellInstruction? currentInstruction = null;
		private IVirtualFileSystem vfs = null!;
		private readonly List<string> commandCompletions = new List<string>();
		private readonly string[] staticCompletions = new string[]
		{
			"cd",
			"clear",
			"exit",
			"echo"
		};

		/// <inheritdoc />
		public bool IsExecutionHalted => currentInstruction != null;

		/// <inheritdoc />
		public string CurrentWorkingDirectory => process?.WorkingDirectory ?? "/";
		
		/// <inheritdoc />
		public string CurrentHomeDirectory => process?.User?.Home ?? "/";
		
		
		public InteractiveShell(MonoBehaviour unityBehaviour)
		{
			this.unityBehaviour = unityBehaviour;
		}

		/// <inheritdoc />
		public void Setup(ISystemProcess process, ITextConsole consoleDevice)
		{
			this.process = process;
			this.consoleDevice = consoleDevice;
			this.vfs = process.User.Computer.GetFileSystem(process.User);

			if (consoleDevice is IAutoCompletedConsole autocompleteConsole)
				autocompleteConsole.AutoCompleteSource = this;
			
			initialized = true;
			
			// Execute the .shrc file if it exists. We don't really have support for scripts yet so
			// this is temporary.
			string homeFolder = process.User.Home;
			string shrcPath = PathUtility.Combine(homeFolder, ".shrc");
			if (!vfs.FileExists(shrcPath))
				return;
            
			shellState = ShellState.Executing;
		}

		public void SetVariableValue(string name, string newValue)
		{
			if (this.process != null)
				this.process.Environment[name] = newValue;
		}

		private void UpdateCommandCompletions()
		{
			this.commandCompletions.Clear();

			if (this.process == null)
				return;
			
			string path = this.process.Environment["PATH"];

			string[] directories = path.Split(':');

			IVirtualFileSystem fs = process.User.Computer.GetFileSystem(process.User);
			foreach (string directory in directories)
			{
				if (!fs.DirectoryExists(directory))
					return;

				foreach (string file in fs.GetFiles(directory))
				{
					string filename = PathUtility.GetFileName(file);
					commandCompletions.Add(filename);
				}
			}
		}

		private async Task<string> ReadLineFromConsole()
		{
			if (consoleDevice == null)
				return string.Empty;

			var lineEditor = new OS.Devices.LineEditor(consoleDevice);
			lineEditor.AutoCompleteSource = this;
			return await lineEditor.ReadLineAsync();
		}
		
		private async Task<string> ReadLine()
		{
			var hasFullLine = false;
			var tempTokenList = new List<string>();

			while (!hasFullLine)
			{
				tempTokenList.Clear();
				
				string text = await ReadLineFromConsole();

				lineBuilder.Append(text);

				hasFullLine = ShellUtility.SimpleTokenize(lineBuilder, tempTokenList);

				if (!hasFullLine)
				{
					lineBuilder.AppendLine();
					consoleDevice?.WriteText(" --> ");
				}
			}

			return lineBuilder.ToString();
		} 
		
		/// <inheritdoc />
		public async Task Run()
		{
			if (process == null)
				return;
			
			if (consoleDevice == null)
				return;
			
			if (!initialized)
				return;

			while (process.IsAlive)
			{
				lineBuilder.Length = 0;
				
				UpdateCommandCompletions();
				
				WritePrompt();
				string nextLineToExecute = await ReadLine();

				if (string.IsNullOrWhiteSpace(nextLineToExecute))
					continue;
				
				ProcessTokens(nextLineToExecute);

				while (pendingInstructions.Count > 0)
				{
					ShellInstruction? nextInstruction = pendingInstructions.Dequeue();

					try
					{
						await nextInstruction.RunAsync(this.process, this.consoleDevice);
					}
					catch (Exception ex)
					{
						// log the full error to Unity
						Debug.LogException(ex);
						
						// Log a partial error to the console
						this.consoleDevice.WriteText(ex.Message + Environment.NewLine);
					}
				}
			}
		}

		public string GetVariableValue(string name)
		{
			return this.process?.Environment[name] ?? name;
		}

		private void ProcessTokens(string nextLineToExecute)
		{
			var sb = new StringBuilder(nextLineToExecute);
			
			// The first tokenization pass we do, before executing this function,
			// only deals with quotes and escape sequences. It also ignores comments,
			// but it has no concept of the actual syntax of the shell language.
			//
			// As such, we must do a more advanced tokenization of the raw input.
			
			// So let's do it.
			IEnumerable<ShellToken>? typedTokens = ShellUtility.IdentifyTokens(sb);
			
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
				case "cd":
				{
					ShellUtility.TrimTrailingSpaces(ref arguments);
					
					if (arguments.Length == 0)
						process.WorkingDirectory = process.User.Home;
					else
					{
						string path = arguments[0];
						string newPath = PathUtility.Combine(process.WorkingDirectory, path);

						newPath = ShellUtility.MakeAbsolutePath(newPath, process.User.Home);

						if (process.User.Computer.GetFileSystem(process.User).DirectoryExists(newPath))
							process.WorkingDirectory = newPath;
						else
						{
							console.WriteText($"sh: cd: {newPath}: Directory not found.{Environment.NewLine}");
						}
					}
					
					break;
				}
				case "clear":
					console.ClearScreen();
					break;
				case "echo":
				{
					string text = string.Join(string.Empty, arguments);
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
			// We first try to parse a variable assignment instruction. If that fails, fall back to parsing a command.
			ShellInstruction? assignment = ParseVariableAssignment(tokenView);
			if (assignment != null)
				return assignment;
			
			CommandData command = ParseCommandWithArguments(tokenView);
			return new SingleInstruction(command);
		}

		private ShellInstruction? ParseVariableAssignment(ArrayView<ShellToken> tokenView)
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
			while (ParseArgument(tokenView, out IArgumentEvaluator? evaluator) && evaluator != null)
			{
				argumentList.Add(evaluator);
			}

			if (argumentList.Count == 0)
			{
				tokenView.Previous();
				tokenView.Previous();
				return null;
			}

			return new AssignmentInstruction(this, identifier, argumentList);
		}
		
		private CommandData ParseCommandWithArguments(ArrayView<ShellToken> tokenView)
		{
			string name = tokenView.Current.Text;
			var arguments = new List<IArgumentEvaluator>();

			tokenView.Advance();
			
			while (ParseArgument(tokenView, out IArgumentEvaluator? evaluator) && evaluator != null)
				arguments.Add(evaluator);

			// If we have tokens remaining, we will need to check for redirection markers.
			return ParseRedirection(tokenView, name, arguments);
		}

		private bool ParseArgument(ArrayView<ShellToken> tokenView, out IArgumentEvaluator? evaluator)
		{
			if (tokenView.EndOfArray)
			{
				evaluator = null;
				return false;
			}

			evaluator = tokenView.Current.TokenType switch
			{
				ShellTokenType.VariableAccess => ParseVariableAccess(tokenView),
				ShellTokenType.Text => ParseTextArgument(tokenView),
				_ => null
			};

			return evaluator != null;
		}

		private IArgumentEvaluator? ParseVariableAccess(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.EndOfArray || tokenView.Current.TokenType != ShellTokenType.VariableAccess)
				return null;

			string text = tokenView.Current.Text;
			tokenView.Advance();

			return new VariableAccessEvaluator(text);
		}
		
		private IArgumentEvaluator? ParseTextArgument(ArrayView<ShellToken> tokenView)
		{
			if (tokenView.EndOfArray || tokenView.Current.TokenType != ShellTokenType.Text)
				return null;

			string text = tokenView.Current.Text;
			tokenView.Advance();
			return new TextArgumentEvaluator(text);
		}
		
		private CommandData ParseRedirection(ArrayView<ShellToken> tokenView, string name, List<IArgumentEvaluator> arguments)
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
		
		private void WritePrompt()
		{
			if (consoleDevice == null || process == null)
				return;
			
			consoleDevice.WriteText($"{process.User.UserName}@{process.User.Computer.Name}:{process.WorkingDirectory}$ ");
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
			public IArgumentEvaluator[] ArgumentList { get; }
			public FileRedirectionType RedirectionType { get; }
			public string FilePath { get; }

			public string FullFilePath
			{
				get => PathUtility.Combine(shell.CurrentWorkingDirectory, FilePath);
			}
			
			public CommandData(InteractiveShell shell,string name, IEnumerable<IArgumentEvaluator> argumentSource, FileRedirectionType redirectionType, string path)
			{
				this.shell = shell;
				
				Name = name;
				ArgumentList = argumentSource.ToArray();
				this.RedirectionType = redirectionType;
				this.FilePath = path;
			}

			private ISystemProcess? FindProgram(ISystemProcess shellProcess, ITextConsole console, string name, string[] args)
			{
				// if the program name starts with a /, use the absolute path.
				if (name.StartsWith("/"))
					return shellProcess.User.Computer.ExecuteProgram(shellProcess, console, name, args);
				
				// if the name starts with "./", try the current directory instead
				if (name.StartsWith("./"))
				{
					string filename = name.Substring(2);
					string fullPath = PathUtility.Combine(shellProcess.WorkingDirectory, filename);
					return shellProcess.User.Computer.ExecuteProgram(shellProcess, console, fullPath, args);
				}
				
				// Read the PATH environment variable to find well-known program paths.
				string pathEnvironmentVariable = shellProcess.Environment["PATH"];
				string[] paths = pathEnvironmentVariable.Split(':', StringSplitOptions.RemoveEmptyEntries);
				
				// Try each program path, check if a full file exists at the given location. If it does, try executing it.
				IVirtualFileSystem fs = shellProcess.User.Computer.GetFileSystem(shellProcess.User);
				foreach (string path in paths)
				{
					string fullPath = PathUtility.Combine(path, name);
					if (fs.FileExists(fullPath))
						return shellProcess.User.Computer.ExecuteProgram(shellProcess, console, fullPath, args);
				}
				
				// Found nothing... :(
				return null;
			}
			
			public ISystemProcess? SpawnCommandProcess(ISystemProcess shellProcess, ITextConsole console)
			{
				// Evaluate arguments now.
				string[] args = ArgumentList.Select(x => x.GetArgumentText(this.shell)).ToArray();
				
				// Get the vfs
				var vfs = shellProcess.User.Computer.GetFileSystem(shellProcess.User);

				// What console are we going to send to the command?
				ITextConsole realConsole = console;

				switch (this.RedirectionType)
				{
					case FileRedirectionType.Append:
						realConsole = new FileOutputConsole(console, vfs.OpenWriteAppend(this.FullFilePath));
						break;
					case FileRedirectionType.Overwrite:
						realConsole = new FileOutputConsole(console, vfs.OpenWrite(this.FullFilePath));
						break;
					case FileRedirectionType.Input:
						realConsole = new FileInputConsole(console, vfs.OpenRead(this.FullFilePath));
						break;
				}

				if (shell.ProcessBuiltIn(shellProcess, realConsole, Name, args))
				{
					if (realConsole is IDisposable disposable)
						disposable.Dispose();
					return null;
				}

				// Process text arguments to make sure we remove their trailing spaces
				ShellUtility.TrimTrailingSpaces(ref args);

				ISystemProcess? commandProcess = FindProgram(shellProcess, realConsole, Name, args);
				if (commandProcess == null)
					console.WriteText($"sh: {Name}: command not found" + Environment.NewLine);
				
				// special case for commands that kill the process IMMEDIATELY
				// on the same frame this was called
				if (commandProcess != null && !commandProcess.IsAlive)
				{
					if (realConsole is IDisposable disposable)
						disposable.Dispose();
					return null;
				}

				void handleKilled(ISystemProcess proc)
				{
					if (realConsole is IDisposable disposable)
						disposable.Dispose();
					proc.Killed -= handleKilled;
				}
				
				if (commandProcess != null)
					commandProcess.Killed += handleKilled;
				return commandProcess;
			}
		}

		public abstract class ShellInstruction
		{
			public abstract bool IsComplete { get; }

			public abstract void Update();
			public abstract void Begin(ISystemProcess process, ITextConsole consoleDevice);

			public abstract Task RunAsync(ISystemProcess process, ITextConsole console);
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
			public override async Task RunAsync(ISystemProcess process, ITextConsole console)
			{
				await Task.WhenAll(first.RunAsync(process, console), next.RunAsync(process, console));
			}

			/// <inheritdoc />
			public override void Update()
			{
				if (!first.IsComplete)
					first.Update();
				
				if (!next.IsComplete)
					next.Update();
			}
		}

		public sealed class PipeInstruction : ShellInstruction
		{
			private readonly ShellInstruction pipeIn;
			private readonly ShellInstruction pipeOut;

			private ISystemProcess? shellProcess;
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
			}

			/// <inheritdoc />
			public override async Task RunAsync(ISystemProcess process, ITextConsole console)
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
						pipeInConsole = new RedirectedConsole(console, lineList);
						
						// second command goes to whatever our output pipe says.
						pipeOutConsole = pipeInstruction.CreateSlavePipe(lineList, console);
					}

					// anything else
					else
					{
						// redirect first command output into the line list
						pipeInConsole = new RedirectedConsole(console, lineList);
						
						// redirect second command input to the line list
						pipeOutConsole = new RedirectedConsole(lineList, console);
					}
				}
				
				if (shellProcess == null)
					return;
				
				if (pipeInConsole == null)
					return;

				if (pipeOutConsole == null)
					return;

				await pipeIn.RunAsync(shellProcess, pipeInConsole);
				await pipeOut.RunAsync(shellProcess, pipeOutConsole);
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
			}
		}

		public sealed class SequentialInstruction : ShellInstruction
		{
			private readonly ShellInstruction[] instructions;
			private int currentInstruction;
			private bool hasStarted;
			private ISystemProcess? shellProcess;
			private ITextConsole? console;

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
			public override async Task RunAsync(ISystemProcess process, ITextConsole console)
			{
				foreach (ShellInstruction instruction in instructions)
				{
					await instruction.RunAsync(process, console);
				}
			}

			/// <inheritdoc />
			public override void Update()
			{
				if (shellProcess == null)
					return;

				if (console == null)
					return;
				
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
			private ISystemProcess? shellProcess;
			private ISystemProcess? currentCommandProcess;
			private ITextConsole? console;
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
			public override async Task RunAsync(ISystemProcess process, ITextConsole console)
			{
				this.currentCommandProcess = command.SpawnCommandProcess(process, console);

				while (currentCommandProcess != null && currentCommandProcess.IsAlive)
					await Task.Yield();
			}

			/// <inheritdoc />
			public override void Update()
			{
				if (IsComplete)
					return;
				
				if (console == null)
					return;
				
				if (shellProcess == null)
					return;
				
				if (waiting)
					return;
                
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
				isCompleted = true;
				process.Killed -= HandleCommandProcessKilled;
				this.currentCommandProcess = null;
				waiting = false;
			}
		}

		private IEnumerable<string> GetAvailableCompletions(string token)
		{
			foreach (string staticCompletion in staticCompletions)
				if (staticCompletion.StartsWith(token) && staticCompletion.Length > token.Length)
					yield return staticCompletion;
			
			foreach (string completion in commandCompletions)
				if (completion.Length > token.Length && completion.StartsWith(token))
					yield return completion;

			if (process == null)
				yield break;
			
			// Environment variables.
			if (token.StartsWith("$"))
			{
				foreach (string key in process.Environment.Keys)
				{
					if (key.StartsWith("$" + token) && key.Length + 1 > token.Length)
						yield return "$" + key;
				}
			}

			IVirtualFileSystem fs = process.User.Computer.GetFileSystem(process.User);

			string fullPath = ShellUtility.MakeAbsolutePath(PathUtility.Combine(process.WorkingDirectory, token), process.User.Home);
			string directory = PathUtility.GetDirectoryName(fullPath);

			if (fs.DirectoryExists(fullPath))
				directory = fullPath;
			
			if (!fs.DirectoryExists(directory))
				yield break;
			
			foreach (string subdir in fs.GetDirectories(directory))
			{
				if (subdir.Length > fullPath.Length && subdir.StartsWith(fullPath))
				{
					yield return token + subdir.Substring(fullPath.Length);
				}
			}
			
			foreach (string subdir in fs.GetFiles(directory))
			{
				if (subdir.Length > fullPath.Length && subdir.StartsWith(fullPath))
				{
					yield return token + subdir.Substring(fullPath.Length);
				}
			}

		}

		/// <inheritdoc />
		public IReadOnlyList<string> GetCompletions(StringBuilder text, out int insertionPoint)
		{
			insertionPoint = text.Length;

			try
			{
				IEnumerable<ShellToken>? tokens = ShellUtility.IdentifyTokens(text)?.ToArray();

				if (tokens == null || !tokens.Any())
					return Array.Empty<string>();

				ShellToken lastToken = tokens.Last();

				insertionPoint = lastToken.Start;
				return GetAvailableCompletions(lastToken.Text).ToArray();
			}
			catch
			{
				// Likely a syntax error so assume there's no completions
				insertionPoint = text.Length;
				return Array.Empty<string>();
			}
		}
	}
} 