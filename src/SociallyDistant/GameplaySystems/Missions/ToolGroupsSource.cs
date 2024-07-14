#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.UI.FloatingTools.FileManager;
using SociallyDistant.UI.Tools.Chat;
using SociallyDistant.UI.Tools.Email;
using SociallyDistant.UI.Tools.Terminal;
using SociallyDistant.UI.Tools.WebBrowser;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class ToolGroupsSource : IGameContentSource
	{
		private readonly TerminalGroupProvider  terminal   = new TerminalGroupProvider();
		private readonly ChatGroupProvider      chat       = new();
		private readonly WebBrowserToolProvider webBrowser = new();
		private readonly EmailGroupProvider     email      = new();
		
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			builder.AddContent(terminal.CreateToolDefinition());
			builder.AddContent(webBrowser.CreateToolDefinition());
			builder.AddContent(chat.CreateToolDefinition());
			builder.AddContent(email.CreateToolDefinition());

			builder.AddContent(new FileManagerProgram());
			
			foreach (MainToolGroup group in await finder.FindContentOfType<MainToolGroup>())
			{
				builder.AddContent(group);
			}
		}
	}
}