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
			foreach (ScriptableCommandBase asset in await finder.FindContentOfType<ScriptableCommandBase>())
				builder.AddContent(asset);
		}
	}
}