#nullable enable
using Core.Scripting;
using GameplaySystems.Social;

namespace Editor.CustomImporters
{
	public sealed class NewsArticleCommands : ScriptModule
	{
		private readonly NewsArticleAsset asset;

		public NewsArticleCommands(NewsArticleAsset asset)
		{
			this.asset = asset;
		}

		[Function("title")]
		private void SetTitle(string[] args)
		{
			asset.Title = string.Join(" ", args);
		}

		[Function("author")]
		private void SetAuthor(string author)
		{
			asset.NarrativeAuthorId = author;
		}

		[Function("id")]
		private void SetId(string id)
		{
			asset.NarrativeId = id;
		}

		[Function("host")]
		private void SetHost(string host)
		{
			asset.HostName = host;
		}

		[Function("excerpt")]
		private void SetExcerpt(string[] text)
		{
			asset.Excerpt = string.Join(" ", text);
		}
	}
}