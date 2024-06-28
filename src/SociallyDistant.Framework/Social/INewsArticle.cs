#nullable enable
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.Core.Social
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