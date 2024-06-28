#nullable enable
using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class MissionScriptLocator : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (MissionScriptAsset script in await finder.FindContentOfType<MissionScriptAsset>())
			{
				try
				{
					if (script is ICachedScript cachedScript)
						await cachedScript.RebuildScriptTree();
					
					builder.AddContent(script);
				}
				catch (Exception ex)
				{
					Log.Error(ex.ToString());
				}
			}
		}
	}
}