using SociallyDistant.Core.Hacking;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS.Network
{
	public class ServerInfo
	{
		private readonly IComputer computer;
		private readonly ServerType serverType;
		private readonly SecurityLevel secLevel;
		private readonly ISystemProcess? daemon;

		public ServerType ServerType => serverType;
		public SecurityLevel SecurityLevel => secLevel;
		
		
		public ServerInfo(IComputer computer, ServerType serverType, SecurityLevel secLevel, ISystemProcess? daemon)
		{
			this.computer = computer;
			this.serverType = serverType;
			this.secLevel = secLevel;
			this.daemon = daemon;
		}

		public ISystemProcess? Breach(IExploit exploit)
		{
			if (daemon == null)
				return null;

			if (!exploit.CanAttack(serverType, SecurityLevel))
				return null;
			
			return daemon;
		}
	}
}