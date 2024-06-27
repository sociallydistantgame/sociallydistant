using System.Collections.Generic;

namespace Social
{
	public interface IMailThread
	{
		int Count { get; }

		IEnumerable<IMailMessage> GetMessagesInThread();
	}
}