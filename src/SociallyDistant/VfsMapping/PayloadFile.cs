#nullable enable
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.GameplaySystems.Hacking;

namespace SociallyDistant.VfsMapping
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