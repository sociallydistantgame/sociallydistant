using SociallyDistant.Architecture;
using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class ProgramSource : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
		}
	}
}