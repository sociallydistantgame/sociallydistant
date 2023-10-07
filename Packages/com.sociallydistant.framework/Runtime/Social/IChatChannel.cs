using System;
using System.Collections.Generic;
using Core;
using Core.WorldData.Data;

namespace Social
{
	public interface IChatChannel
	{
		ObjectId Id { get; }
		string Name { get; }
		string Description { get; }
		
		IObservable<IUserMessage> SendObservable { get; }
		IObservable<IUserMessage> EditObservable { get; }
		IObservable<IUserMessage> DeleteObservable { get; }
		
		
		MessageChannelType ChannelType { get; }
		ObjectId? GuildId { get; }
		
		IEnumerable<IUserMessage> Messages { get; }
	}
}