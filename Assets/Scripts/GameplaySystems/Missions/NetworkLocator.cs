#nullable enable
using System.Threading.Tasks;
using ContentManagement;
using GameplaySystems.Hacking.Assets;

namespace GameplaySystems.Missions
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