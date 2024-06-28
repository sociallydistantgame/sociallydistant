namespace SociallyDistant.Core.Social
{
	public interface IMailThread
	{
		int Count { get; }

		IEnumerable<IMailMessage> GetMessagesInThread();
	}
}