#nullable enable
using System;
using System.Threading.Tasks;
using Core.Scripting.Consoles;
using OS.Devices;
using OS.FileSystems;
using OS.FileSystems.Host;

namespace Core.Scripting
{
	public class OperatingSystemExecutionContext : IScriptExecutionContext
	{
		private readonly ISystemProcess process;

		public OperatingSystemExecutionContext(ISystemProcess process)
		{
			this.process = process;
		}

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
		public Task<bool> TryExecuteCommandAsync(string name, string[] args, ITextConsole console)
		{
			ISystemProcess? commandProcess = FindProgram(this.process, console, name, args);
			if (commandProcess == null)
				return Task.FromResult(false);
			
			// special case for commands that kill the process IMMEDIATELY
			// on the same frame this was called
			if (commandProcess != null && !commandProcess.IsAlive)
				return Task.FromResult(true);

			var completionSource = new TaskCompletionSource<bool>();

			commandProcess.Killed += HandleKill;

			return completionSource.Task;
			
			void HandleKill(ISystemProcess killed)
			{
				killed.Killed -= HandleKill;
				completionSource.SetResult(true);
			}
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
		public void HandleCommandNotFound(string name, ITextConsole console)
		{
			console.WriteText($"sh: \x1b[1m{name}\x1b[0m: \x1b[91mCommand not found.\x1b[0m{Environment.NewLine}");
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
	}
}