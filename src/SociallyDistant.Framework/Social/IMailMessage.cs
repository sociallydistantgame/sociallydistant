using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.Core.Social
{
	public interface IMailMessage
	{
		IProfile From { get; }
		IProfile To { get; }
		IMailThread? Thread { get; }
		string Subject { get; }
		string NarrativeId { get; }
		MailTypeFlags MessageType { get; }
		
		IEnumerable<DocumentElement> Body { get; }
	}
}