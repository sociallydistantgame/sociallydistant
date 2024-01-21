#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using Core.Scripting.Consoles;
using OS.Devices;
using OS.FileSystems;
using OS.FileSystems.Host;
using UI.Shell;
using UnityEngine;
using Utility;
using Core.Scripting.Instructions;
using Core.Scripting.Parsing;

namespace Core.Scripting
{
	public class InteractiveShell :
		ITerminalProcessController,
		ICommandShell,
		IAutoCompleteSource,
		IScriptExecutionContext
	{
		private readonly IScriptExecutionContext scriptContext;
		private ISystemProcess? process;
		private ITextConsole? consoleDevice;
		private bool initialized;
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
		
		public bool HandleExceptionsGracefully { get; set; }
		
		public InteractiveShell(IScriptExecutionContext context)
		{
			this.scriptContext = context;
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
			scriptContext.SetVariableValue(name, newValue);
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			switch (name)
			{
				case "test":
				{
					var shellTest = new ShellTester(args);
					return shellTest.Test();
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
					string text = string.Join(string.Empty, args);
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

		public async Task RunScript(string scriptText)
		{
			if (process == null)
				return;
			
			if (consoleDevice == null)
				return;
			
			if (!initialized)
				return;

			await ProcessTokens(scriptText);

			while (pendingInstructions.Count > 0)
			{
				ShellInstruction? nextInstruction = pendingInstructions.Dequeue();
				
				try
				{
					await nextInstruction.RunAsync(this.consoleDevice);
				}
				catch (ScriptEndException)
				{
					pendingInstructions.Clear();
					return;
				}
				catch (Exception ex)
				{
					if (!HandleExceptionsGracefully)
						throw;
					
					// log the full error to Unity
					Debug.LogException(ex);
						
					// Log a partial error to the console
					this.consoleDevice.WriteText(ex.Message + Environment.NewLine);
				}
			}
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
				
				await ProcessTokens(nextLineToExecute);

				while (pendingInstructions.Count > 0)
				{
					ShellInstruction? nextInstruction = pendingInstructions.Dequeue();

					try
					{
						await nextInstruction.RunAsync(this.consoleDevice);
					}
					catch (ScriptEndException)
					{
						pendingInstructions.Clear();
						return;
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
			return scriptContext.GetVariableValue(name);
		}

		private async Task ProcessTokens(string nextLineToExecute)
		{
			// The first tokenization pass we do, before executing this function,
			// only deals with quotes and escape sequences. It also ignores comments,
			// but it has no concept of the actual syntax of the shell language.
			//
			// As such, we must do a more advanced tokenization of the raw input.
			
			// So let's do it.
			IEnumerable<ShellToken>? typedTokens = ShellUtility.IdentifyTokens(nextLineToExecute);
			
			// Create a view over this array that we can advance during parsing
			var view = new ArrayView<ShellToken>(typedTokens.ToArray());

			var parser = new ScriptParser(this);
			
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
				IEnumerable<ShellToken>? tokens = ShellUtility.IdentifyTokens(text.ToString())?.ToArray();

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