#nullable enable
using System;
using Core.WorldData.Data;

namespace Social
{
	public interface INewsArticle
	{
		string? HostName { get; }
		IProfile Author { get; }
		string Headline { get; }
		DateTime Date { get; }

		DocumentElement[] GetBody();
	}
}