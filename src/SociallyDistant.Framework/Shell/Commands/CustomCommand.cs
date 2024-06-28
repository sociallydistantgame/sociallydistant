#nullable enable

using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Shell.Commands
{
	public abstract class CustomCommand
	{
		public string Name { get; protected set; } = string.Empty;
		public bool RequiresAdmin { get; protected set; }
		public bool PlayerOnly { get; protected set; }

		public abstract Task<int> Run(string[] args, ISystemProcess process, ConsoleWrapper console);
	}
}