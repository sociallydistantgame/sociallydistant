using System.Collections.Generic;

namespace Social
{
	public interface IGuild : IChatGroup
	{
		string Name { get; }
		
		IEnumerable<IChatChannel> Channels { get; }
	}
}