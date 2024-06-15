#nullable enable
using System.Threading.Tasks;
using ContentManagement;
using Core;
using Core.WorldData.Data;
using Modules;
using UnityEngine;

namespace DevTools
{
	public interface IArticleAsset
	{
		string NarrativeId { get; }
		string HostName { get; }
		string Title { get; }
		string NarrativeAuthorId { get; }
		string Excerpt { get; }

		DocumentElement[] Body { get; }

		Task<Texture2D?> GetFeaturedImage();
	}
	
	public interface ICharacterGenerator : IGameContent
	{
		Task GenerateNpcs(IWorldManager world);
	}
}