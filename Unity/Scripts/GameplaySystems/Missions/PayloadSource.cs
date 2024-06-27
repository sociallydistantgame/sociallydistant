#nullable enable
using System.Threading.Tasks;
using ContentManagement;
using GameplaySystems.Hacking;

namespace GameplaySystems.Missions
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