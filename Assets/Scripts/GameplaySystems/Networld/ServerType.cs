using System;

namespace GameplaySystems.Networld
{
	public enum ServerType
	{
		Netcat,
		Shell,
		FileTransfer,
		Web,
		WebSecured,
		Database,
		Desktop,
		GameServer,
		Email,
		Chat
	}

	[Flags]
	public enum ServerTypeFlags
	{
		Nothing=0,
		Shell=1,
		FileTransfer=2,
		Web=4,
		WebSecured=8,
		Database=16,
		Desktop=32,
		GameServer=64,
		Email=128,
		Chat=256
	}
}