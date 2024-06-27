using System;
using Core;
using Core.WorldData.Data;

namespace Social
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