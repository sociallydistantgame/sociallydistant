#nullable enable
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Social;
using SociallyDistant.GameplaySystems.Social;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class NpcSource : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (ICharacterGenerator generator in await finder.FindContentOfType<NpcGeneratorScript>())
			{
				builder.AddContent(generator);
			}
		}
	}
}