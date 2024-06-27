using System.Collections.Generic;
using Core.WorldData.Data;

namespace Social
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