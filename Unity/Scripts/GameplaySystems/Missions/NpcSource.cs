#nullable enable
using System.Threading.Tasks;
using ContentManagement;
using DevTools;
using GameplaySystems.Social;

namespace GameplaySystems.Missions
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