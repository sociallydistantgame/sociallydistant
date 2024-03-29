using System;

namespace Core.WorldData.Data
{
	[Flags]
	public enum MailTypeFlags : byte
	{
		None = 0,
		Read = 1,
		Briefing = 2,
		CompletedMission = 4,
		Accessibility = 8,
		SystemMessage = 16
	}
}