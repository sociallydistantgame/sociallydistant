#nullable enable
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.GameplaySystems.Hacking;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class PayloadSource : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (PayloadAsset asset in await finder.FindContentOfType<PayloadAsset>())
				builder.AddContent(asset);
		}
	}
}