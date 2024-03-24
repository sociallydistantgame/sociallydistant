#nullable enable

using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using Core.Scripting.Consoles;
using OS.Devices;
using OS.FileSystems;
using OS.FileSystems.Host;
using UI.Shell;
using Utility;
using Core.Scripting.Instructions;
using Core.Scripting.Parsing;
using Debug = UnityEngine.Debug;

namespace Core.Scripting
{
	public class InteractiveShell :
		ITerminalProcessController,
		ICommandShell,
		IAutoCompleteSource,
		IScriptExecutionContext
	{
		private readonly IScriptExecutionContext scriptContext;
		private ITextConsole? consoleDevice;
		private bool initialized;
		private ShellState shellState;
		private readonly StringBuilder lineBuilder = new StringBuilder();
		private readonly List<string> tokenList = new List<string>();
		private readonly Queue<ShellInstruction> pendingInstructions = new Queue<ShellInstruction>();
		private ShellInstruction? currentInstruction = null;
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

		public bool HandleExceptionsGracefully { get; set; }
		
		public InteractiveShell(IScriptExecutionContext context)
		{
			this.scriptContext = context;
		}

		/// <inheritdoc />
		public void Setup(ITextConsole consoleDevice)
		{
			this.consoleDevice = consoleDevice;
			
			initialized = true;
			
			shellState = ShellState.Executing;
		}

		public void SetVariableValue(string name, string newValue)
		{
			scriptContext.SetVariableValue(name, newValue);
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			switch (name)
			{
				case "[":
				case "[[":
				case "test":
				{
					var shellTest = new ShellTester(args, console);
					return shellTest.Test() ? 0 : 1;
				}
				case "exit":
				case "return":
				{
					int exitCode = 0;

					if (args.Length > 0)
					{
						if (int.TryParse(args[0], out int parsedExitCode))
							exitCode = parsedExitCode;
					}

					throw new ScriptEndException(exitCode, name == "return");
				}
				case "clear":
					console.ClearScreen();
					return 0;
				case "echo":
				{
					string text = string.Join(" ", args);
					console.WriteText(text + Environment.NewLine);
					return 0;
				}
			}
			
			return await this.scriptContext.TryExecuteCommandAsync(name, args, console, callSite);
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			return this.scriptContext.OpenFileConsole(realConsole, filePath, mode);
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, ITextConsole console)
		{
			scriptContext.HandleCommandNotFound(name, console);
		}

		private void UpdateCommandCompletions()
		{
			this.commandCompletions.Clear();
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

		private async Task RunScriptInternal(string scriptText, bool isInteractive)
		{
			if (consoleDevice == null)
				return;
			
			if (!initialized)
				return;

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			
			try
			{
				await ProcessTokens(scriptText.Trim(), !isInteractive);


				while (pendingInstructions.Count > 0)
				{
					ShellInstruction? nextInstruction = pendingInstructions.Dequeue();

					try
					{
						await nextInstruction.RunAsync(this.consoleDevice);
					}
					catch (ScriptEndException endException)
					{
						if (endException.LocalScope)
						{
							consoleDevice.WriteText($"-sh: return: can only `return' from a function or sourced script{Environment.NewLine}");
							continue;
						}

						pendingInstructions.Clear();

						if (isInteractive) throw;
					}
				}
			}
			catch (ScriptEndException)
			{
				stopwatch.Stop();
				PrintTimeDiagnostics(stopwatch.Elapsed, isInteractive);
				throw;
			}
			catch (Exception ex)
			{
				if (!HandleExceptionsGracefully)
				{
					stopwatch.Stop();
					PrintTimeDiagnostics(stopwatch.Elapsed, isInteractive);
					throw;
				}

				// log the full error to Unity
				Debug.LogException(ex);

				// Log a partial error to the console
				this.consoleDevice.WriteText(ex.Message + Environment.NewLine);
			}
			
			stopwatch.Stop();
			PrintTimeDiagnostics(stopwatch.Elapsed, isInteractive);
		}

		private void PrintTimeDiagnostics(TimeSpan time, bool isInteractive)
		{
			var sb = new StringBuilder();
			sb.Append("Script execution took ");
			sb.AppendLine(time.ToString());

			if (isInteractive)
				this.consoleDevice?.WriteText(sb.ToString());

			switch (time)
			{
				case { } when time.TotalSeconds > 5:
				{
					Debug.LogError(sb.ToString());
					break;
				}
				case { } when time.TotalSeconds > 1:
				{
					Debug.LogWarning(sb.ToString());
					break;
				}
				default:
				{
					Debug.Log(sb.ToString());
					break;
				}
			}
		}
		
		public async Task RunScript(string scriptText)
		{
			await RunScriptInternal(scriptText, false);
		}

		/// <inheritdoc />
		public async Task Run()
		{
			if (consoleDevice == null)
				return;

			if (!initialized)
				return;

			lineBuilder.Length = 0;

			UpdateCommandCompletions();

			await WritePrompt();
			var nextLineToExecute = string.Empty;

			try
			{
				nextLineToExecute = await ReadLine();
			}
			catch (Exception ex)
			{
				if (!HandleExceptionsGracefully)
					throw;
				
				Debug.LogException(ex);
				
				this.consoleDevice.WriteText($"-sh: {ex.Message}{Environment.NewLine}");
				return;
			}

			if (string.IsNullOrWhiteSpace(nextLineToExecute))
				return;

			await RunScriptInternal(nextLineToExecute, true);
		}

		public string GetVariableValue(string name)
		{
			return scriptContext.GetVariableValue(name);
		}

		private void StripComments(ref string text)
		{
			string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
			var stringBuilder = new StringBuilder(text.Length);

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				int commentIndex = line.IndexOf('#', StringComparison.Ordinal);
				if (commentIndex == -1 && !string.IsNullOrWhiteSpace(line))
				{
					stringBuilder.AppendLine(line);
					continue;
				}

				string strippedLine = line.Substring(0, commentIndex).Trim();
				if (string.IsNullOrWhiteSpace(strippedLine))
					continue;

				stringBuilder.AppendLine(strippedLine);
			}

			text = stringBuilder.ToString().Trim();
		}
		
		private async Task ProcessTokens(string nextLineToExecute, bool useLocalExecutionContext)
		{
			StripComments(ref nextLineToExecute);
			if (string.IsNullOrWhiteSpace(nextLineToExecute))
				return;
			
			// The first tokenization pass we do, before executing this function,
			// only deals with quotes and escape sequences. It also ignores comments,
			// but it has no concept of the actual syntax of the shell language.
			//
			// As such, we must do a more advanced tokenization of the raw input.
			
			// So let's do it.
			IEnumerable<ShellToken>? typedTokens = ShellUtility.IdentifyTokens(nextLineToExecute);
			
			// Create a view over this array that we can advance during parsing
			var view = new ArrayView<ShellToken>(typedTokens.ToArray());

			var parser = new ScriptParser(this, useLocalExecutionContext);
			
			pendingInstructions.Enqueue(await parser.ParseScript(view));
		}

		public bool ProcessBuiltIn(ISystemProcess process, ITextConsole console, string name, string[] arguments)
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
				case "exit":
					process.Kill();
					break;
				default:
					return false;
			}

			return true;
		}

		private async Task WritePrompt()
		{
			if (consoleDevice == null)
				return;

			if (this.scriptContext is IInteractiveShellContext interactiveContext)
			{
				await interactiveContext.WritePrompt(this.consoleDevice);
			}
			else
			{
				this.consoleDevice.WriteText("$ ");
			}
		}

		private enum ShellState
		{
			Init,
			Reading,
			Processing,
			Executing
		}

		/// <inheritdoc />
		public IReadOnlyList<string> GetCompletions(StringBuilder text, out int insertionPoint)
		{
			insertionPoint = text.Length;

			while (insertionPoint > 0)
			{
				char character = text[insertionPoint - 1];
				if (!char.IsLetterOrDigit(character) && character != '_')
					break;

				insertionPoint--;
			}	;

			string textToComplete = text.ToString().Substring(insertionPoint);

			var completionList = new List<string>();

			if (string.IsNullOrWhiteSpace(textToComplete))
				return completionList;
			
			foreach (string staticCompletion in staticCompletions)
				if (staticCompletion.StartsWith(textToComplete))
					completionList.Add(staticCompletion);
			
			foreach (string command in commandCompletions)
				if (command.StartsWith(textToComplete))
					completionList.Add(command);

			return completionList;
		}
	}
} 