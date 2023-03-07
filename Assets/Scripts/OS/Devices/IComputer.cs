#nullable enable
using GameplaySystems.Networld;
using OS.FileSystems;
using OS.Network;

namespace OS.Devices
{
	public interface IComputer
	{
		string Name { get; }
		bool FindUserById(int id, out IUser? user);
		bool FindUserByName(string username, out IUser? user);

		/// <summary>
		///		Forks the given process and executes the specified program with the given
		///		command-line arguments on the forked process.
		/// </summary>
		/// <param name="parentProcess">The parent process to fork</param>
		/// <param name="console">A console device for the child process to use as its standard console.</param>
		/// <param name="programName">The name of the program to run</param>
		/// <param name="arguments">Command-line arguments to pass to the program to run</param>
		/// <returns>The forked child process for the program, or null if the program doesn't exist.</returns>
		ISystemProcess? ExecuteProgram(ISystemProcess parentProcess, ITextConsole console, string programName, string[] arguments);

		/// <summary>
		///		Gets an instance of the <see cref="VirtualFileSystem" /> class that
		///		interacts with this computer's root filesystem with the permissions
		///		of the given user.
		/// </summary>
		/// <param name="user">The user to operate the filesystem with. Must belong to this computer.</param>
		/// <returns>A view into the computer's file system from the perspective of the given user.</returns>
		VirtualFileSystem GetFileSystem(IUser user);
		
		NetworkConnection? Network { get; }
	}
}