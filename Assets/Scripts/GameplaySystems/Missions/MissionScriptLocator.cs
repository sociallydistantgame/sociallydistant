#nullable enable
using System.Threading.Tasks;
using ContentManagement;
using UnityEngine;

namespace GameplaySystems.Missions
{
	public sealed class MissionScriptLocator : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder)
		{
			MissionScriptAsset[]? assets = Resources.LoadAll<MissionScriptAsset>("Missions");

			foreach (MissionScriptAsset script in assets)
			{
				await Task.Yield();
				builder.AddContent(script);
			}
		}
	}
}