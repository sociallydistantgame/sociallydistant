#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class CommandSource : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (CommandAsset asset in await finder.FindContentOfType<CommandAsset>())
				builder.AddContent(asset);
		}
	}
}