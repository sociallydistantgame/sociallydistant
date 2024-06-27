#nullable enable
using System;
using System.Threading.Tasks;
using ContentManagement;
using Core;
using UnityEngine;

namespace GameplaySystems.Chat
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
					Debug.LogException(ex);
				}
			}
			
		}
	}
}