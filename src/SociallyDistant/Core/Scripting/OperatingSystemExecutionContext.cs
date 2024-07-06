#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.Scripting.Consoles;
using SociallyDistant.Core.Shell.Commands;
using SociallyDistant.OS.Devices;
using SociallyDistant.OS.FileSystems.Host;

namespace SociallyDistant.Core.Scripting
{
	public class OperatingSystemExecutionContext : IInteractiveShellContext
	{
		private readonly ISystemProcess process;
		private readonly ScriptFunctionManager functions = new();

		public OperatingSystemExecutionContext(ISystemProcess process)
		{
			this.process = process;
		}

		/// <inheritdoc />
		public string Title => $"{process.User.UserName} on {process.User.Computer.Name}";

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			return process.Environment[variableName];
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			process.Environment[variableName] = value;
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			int? functionResult = await functions.CallFunction(name, args, console, callSite ?? this);
			if (functionResult != null)
				return functionResult;

			if (HandleBuiltin(name, args, console, callSite ?? this))
				return 0;
			
			var sys = SystemModule.GetSystemModule();
			CustomCommandAsset? customCommand = sys.Context.ContentManager.GetContentOfType<CustomCommandAsset>()
				.FirstOrDefault(x => x.Name == name);

			if (customCommand != null)
				return await HandleCustomCommand(customCommand, name, args, console);
				
			
			ISystemProcess? commandProcess = await FindProgram(this.process, console, name, args);
			if (commandProcess == null)
				return null;
			
			// special case for commands that kill the process IMMEDIATELY
			// on the same frame this was called
			if (commandProcess != null && !commandProcess.IsAlive)
				return commandProcess.ExitCode;

			return await WaitForProcessKill(commandProcess);
		}

		private Task<int?> WaitForProcessKill(ISystemProcess process)
		{
			var completionSource = new TaskCompletionSource<int?>();

			process.Killed += HandleKill;

			return completionSource.Task;
			
			void HandleKill(ISystemProcess killed)
			{
				killed.Killed -= HandleKill;
				completionSource.SetResult(killed.ExitCode);
			}
		}
		
		private bool HandleBuiltin(string name, string[] args, ITextConsole console, IScriptExecutionContext callSite)
		{
			switch (name)
			{
				case "pwd":
				{
					console.WriteText(process.WorkingDirectory + Environment.NewLine);
					return true;
				}
				case "cd":
				{
					if (args.Length == 0)
					{
						process.WorkingDirectory = process.User.Home;
						return true;
					}
					
					string relativePath = string.Join(" ", args);

					string fullPath = ShellUtility.MakeAbsolutePath(PathUtility.Combine(process.WorkingDirectory, relativePath), process.User.Home);
					
					IVirtualFileSystem vfs = process.User.Computer.GetFileSystem(process.User);

					if (vfs.FileExists(fullPath))
					{
						console.WriteText($"-sh: cd: {fullPath}: Not a directory.{Environment.NewLine}");
					}
					else if (!vfs.DirectoryExists(fullPath))
					{
						console.WriteText($"-sh: cd: {fullPath}: No such file or directory.{Environment.NewLine}");
					}
					else
					{
						process.WorkingDirectory = fullPath;
					}
					
					return true;
				}
				case "export":
					if (args.Length == 1)
					{
						string varName = args[0];
						string value = callSite.GetVariableValue(varName);
						process.Environment[varName] = value;
					} 
					else if (args.Length == 3)
					{
						string varName = args[0];
						string value = args[2];
						
						SetVariableValue(varName, value);
						process.Environment[varName] = value;
					}
					return true;
				default: return false;
			}
		}
		
		private async Task<int?> HandleCustomCommand(CustomCommandAsset command, string name, string[] args, ITextConsole console)
		{
			if (command.IsPlayerOnly && process.User.Computer is not PlayerComputer)
				return null;

			var consoleWrapper = new ConsoleWrapper(console);
			ISystemProcess commandProcess = process.Fork();

			if (command.AdminRequired && commandProcess.User.PrivilegeLevel != PrivilegeLevel.Root)
			{
				consoleWrapper.WriteLine($"{name}: must be root");
				
				commandProcess.Kill(1);
				return commandProcess.ExitCode;
			}
			
			CustomCommand commandInstance = command.CreateInstance();
			int result = await commandInstance.Run(args, commandProcess, consoleWrapper);
			
			if (commandProcess.IsAlive)
				commandProcess.Kill(result);

			return result;
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			IVirtualFileSystem vfs = process.User.Computer.GetFileSystem(process.User);
			string fullFilePath = PathUtility.Combine(this.process.WorkingDirectory, filePath);
			
			switch (mode)
			{
				case FileRedirectionType.Append:
					return new FileOutputConsole(realConsole, vfs.OpenWriteAppend(fullFilePath));
				case FileRedirectionType.Overwrite:
					return new FileOutputConsole(realConsole, vfs.OpenWrite(fullFilePath));
				case FileRedirectionType.Input:
					return new FileInputConsole(realConsole, vfs.OpenRead(fullFilePath));
			}

			return realConsole;
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, string[] args, ITextConsole console)
		{
			console.WriteText($"sh: \x1b[1m{name}\x1b[0m: \x1b[91mCommand not found.\x1b[0m{Environment.NewLine}");
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			functions.DeclareFunction(name, body);
		}

		/// <inheritdoc />
		public async Task WritePrompt(ITextConsole console)
		{
			string promptCommand = GetVariableValue("PROMPT_COMMAND");
			if (!string.IsNullOrWhiteSpace(promptCommand))
			{
				var shell = new InteractiveShell(this);
				shell.Setup(console);
				await shell.RunScript(promptCommand);
				return;
			}

			string ps1 = GetPromptString();
			console.WriteText(ps1);
		}

		private DateTime GetCurrentTime()
		{
			var system = SystemModule.GetSystemModule();
			IWorld world = system.Context.WorldManager.World;

			return world.GlobalWorldState.Value.Now;
		}
		
		private string GetPromptString()
		{
			DateTime date = GetCurrentTime();
			string ps1 = GetVariableValue("PS1");

			string workingDirectory = process.WorkingDirectory.StartsWith(process.User.Home)
				? "~" + process.WorkingDirectory.Substring(process.User.Home.Length)
				: process.WorkingDirectory;
			
			var values = new Dictionary<string, string>();
			values.Add("%u", this.process.User.UserName);
			values.Add("%h", process.User.Computer.Name);
			values.Add("%w", workingDirectory);
			values.Add("%d", FormattedDay(date));
			values.Add("%t", FormattedTime(date));
			
			values.Add("%$", process.User.PrivilegeLevel == PrivilegeLevel.Root ? "#" : "$");
			
			foreach (string key in values.Keys)
			{
				if (!ps1.Contains(key))
					continue;

				ps1 = ps1.Replace(key, values[key]);
			}

			return ps1;
		}

		private string FormattedTime(DateTime date)
		{
			return date.ToShortTimeString();
		}
		
		private string FormattedDay(DateTime date)
		{
			return date.DayOfWeek switch
			{
				DayOfWeek.Friday => "Friday",
				DayOfWeek.Monday => "Monday",
				DayOfWeek.Saturday => "Saturday",
				DayOfWeek.Sunday => "Sunday",
				DayOfWeek.Thursday => "Thursday",
				DayOfWeek.Tuesday => "Tuesday",
				DayOfWeek.Wednesday => "Wednesday",
				_ => "Glitchday"
			};
		}

		private async Task<ISystemProcess?> FindProgram(ISystemProcess shellProcess, ITextConsole console, string name, string[] args)
		{
			// if the program name starts with a /, use the absolute path.
			if (name.StartsWith("/"))
				return await shellProcess.User.Computer.ExecuteProgram(shellProcess, console, name, args);

			// if the name starts with "./", try the current directory instead
			if (name.StartsWith("./"))
			{
				string filename = name.Substring(2);
				string fullPath = PathUtility.Combine(shellProcess.WorkingDirectory, filename);
				return await shellProcess.User.Computer.ExecuteProgram(shellProcess, console, fullPath, args);
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
					return await shellProcess.User.Computer.ExecuteProgram(shellProcess, console, fullPath, args);
			}

			// Found nothing... :(
			return null;
		}
	}
}