#nullable enable
using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;

namespace SociallyDistant.GameplaySystems.Chat
{
	public sealed class ConversationLocator : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (ChatConversationAsset asset in await finder.FindContentOfType<ChatConversationAsset>())
			{
				try
				{
					if (asset is ICachedScript script)
						await script.RebuildScriptTree();
					
					builder.AddContent(asset);
				}
				catch (Exception ex)
				{
					Log.Error(ex.ToString());
				}
			}
			
		}
	}
}