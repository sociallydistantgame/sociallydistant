#nullable enable
using GameplaySystems.Hacking;
using OS.FileSystems;

namespace VfsMapping
{
	public class PayloadFile : UnlockableAssetFileEntry<PayloadAsset>
	{
		/// <inheritdoc />
		public PayloadFile(IDirectoryEntry parent, PayloadAsset asset) : base(parent, asset)
		{ }

		/// <inheritdoc />
		public override bool CanExecute => false;
	}
}