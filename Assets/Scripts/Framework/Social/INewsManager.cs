#nullable enable
using System;
using System.Collections.Generic;

namespace Social
{
	public interface INewsManager : IDisposable
	{
		INewsArticle? LatestNews { get; }
		
		IEnumerable<INewsArticle> AllArticles { get; }

		IEnumerable<INewsArticle> GetArticlesForHost(string hostname);
	}
}