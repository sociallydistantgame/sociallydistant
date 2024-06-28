#nullable enable
namespace SociallyDistant.Core.Social
{
	public interface INewsManager : IDisposable
	{
		INewsArticle? LatestNews { get; }
		
		IEnumerable<INewsArticle> AllArticles { get; }

		IEnumerable<INewsArticle> GetArticlesForHost(string hostname);
	}
}