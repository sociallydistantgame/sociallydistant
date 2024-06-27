#nullable enable
using System.Collections.Generic;
using System.IO;
using OS.Devices;
using System.Threading.Tasks;

namespace OS.FileSystems
{
	public interface IVirtualFileSystem
	{
		bool DirectoryExists(string path);
		bool FileExists(string path);
		void CreateDirectory(string path);
		void DeleteFile(string path);
		void DeleteDirectory(string path);
		Stream OpenRead(string path);
		Stream OpenWrite(string path);
		Stream OpenWriteAppend(string path);

		bool IsExecutable(string path);

		Task<ISystemProcess> Execute(ISystemProcess parent, string path, ITextConsole console, string[] arguments);

		void WriteAllText(string path, string text);
		void WriteAllBytes(string path, byte[] bytes);

		byte[] ReadAllBytes(string path);
		string ReadAllText(string path);

		IEnumerable<string> GetDirectories(string path);
		IEnumerable<string> GetFiles(string path);

		void Mount(string path, IFileSystem filesystem);
		void Unmount(string path);
	}
}