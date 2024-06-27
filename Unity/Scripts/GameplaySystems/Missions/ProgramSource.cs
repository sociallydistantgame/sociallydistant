using System.Threading.Tasks;
using Architecture;
using ContentManagement;

namespace GameplaySystems.Missions
{
	public sealed class ProgramSource : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (UguiProgram program in await finder.FindContentOfType<UguiProgram>())
			{
				builder.AddContent(program);
			}
		}
	}
}