#nullable enable
using System.Diagnostics;
using Serilog;
using SociallyDistant.Architecture;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.VfsMapping
{
	internal sealed class ScriptableCommandFileEntry : AssetFileEntry<CommandAsset>
	{
		/// <inheritdoc />
		public override bool CanExecute => true;
		
		/// <inheritdoc />
		public ScriptableCommandFileEntry(IDirectoryEntry parent, CommandAsset asset) : base(parent, asset)
		{ }

		/// <inheritdoc />
		public override async Task<bool> TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			try
			{
				await Asset.Main(process, console, arguments);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				process.Kill();
			}

			return true;
		}
	}
}