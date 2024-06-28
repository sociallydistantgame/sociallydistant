using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.Core.Social
{
	public interface IUserMessage
	{
		ObjectId ChannelId { get; }
		ObjectId Id { get; }
		IProfile Author { get; }
		DateTime Date { get; }

		DocumentElement[] GetDocumentData();
	}
}