#nullable enable
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.Shell;
using SociallyDistant.UI.Shell;

namespace SociallyDistant.VfsMapping
{
	public sealed class UguiProgramBinaryFile : AssetFileEntry<IProgram>
	{
		/// <inheritdoc />
		public override bool CanExecute => true;
		
		/// <inheritdoc />
		public UguiProgramBinaryFile(IDirectoryEntry parent, IProgram asset) : base(parent, asset)
		{ }

		/// <inheritdoc />
		public override async Task<bool> TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			process.Name = Name;

			var desktop = Application.Instance.Context.Shell;
			
			desktop.OpenProgram(Asset, arguments, process, console);
			return true;
		}
	}
}