using System.Collections.Generic;
using Core;

namespace Social
{
	public interface IGuild : IChatGroup
	{
		ObjectId Id { get; }
		
		string Name { get; }
		
		IEnumerable<IChatChannel> Channels { get; }
	}
}