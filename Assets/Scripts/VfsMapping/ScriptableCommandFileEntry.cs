#nullable enable
using Architecture;
using OS.Devices;
using OS.FileSystems;

namespace VfsMapping
{
	public sealed class ScriptableCommandFileEntry : AssetFileEntry<ScriptableCommandBase>
	{
		/// <inheritdoc />
		public override bool CanExecute => true;
		
		/// <inheritdoc />
		public ScriptableCommandFileEntry(IDirectoryEntry parent, ScriptableCommandBase asset) : base(parent, asset)
		{ }

		/// <inheritdoc />
		public override bool TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			Asset.Main(process, console, arguments);
			return true;
		}
	}
}