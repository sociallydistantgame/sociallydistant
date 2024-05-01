#nullable enable
using System.Threading.Tasks;
using Architecture;
using ContentManagement;

namespace GameplaySystems.Missions
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