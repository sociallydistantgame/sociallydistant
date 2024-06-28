#nullable enable
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.GameplaySystems.Hacking.Assets;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class NetworkLocator : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (NetworkAsset asset in await finder.FindContentOfType<NetworkAsset>())
			{
				await asset.RebuildScriptTree();
				builder.AddContent(asset);
			}
		}
	}
}