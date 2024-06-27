#nullable enable
using System.Threading.Tasks;
using Architecture;
using ContentManagement;

namespace GameplaySystems.Missions
{
	public sealed class ToolGroupsSource : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (MainToolGroup group in await finder.FindContentOfType<MainToolGroup>())
			{
				builder.AddContent(group);
			}
		}
	}
}