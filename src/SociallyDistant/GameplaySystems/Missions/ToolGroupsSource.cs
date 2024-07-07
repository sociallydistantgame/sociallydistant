#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.UI.Tools.Terminal;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class ToolGroupsSource : IGameContentSource
	{
		private readonly TerminalGroupProvider terminal = new TerminalGroupProvider();
		
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			builder.AddContent(terminal.CreateToolDefinition());
			
			foreach (MainToolGroup group in await finder.FindContentOfType<MainToolGroup>())
			{
				builder.AddContent(group);
			}
		}
	}
}