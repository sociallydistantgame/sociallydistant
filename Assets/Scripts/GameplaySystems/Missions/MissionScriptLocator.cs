#nullable enable
using System;
using System.Threading.Tasks;
using ContentManagement;
using Core;
using UnityEngine;

namespace GameplaySystems.Missions
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
					Debug.LogException(ex);
				}
			}
		}
	}
}