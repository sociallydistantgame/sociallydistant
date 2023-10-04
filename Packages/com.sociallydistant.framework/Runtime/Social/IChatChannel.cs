using System.Collections.Generic;

namespace Social
{
	public interface IChatChannel
	{
		string Name { get; }
		string Description { get; }
		
		IEnumerable<IUserMessage> Messages { get; }
	}
}