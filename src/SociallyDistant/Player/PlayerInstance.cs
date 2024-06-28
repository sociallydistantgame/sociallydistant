#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.GameplaySystems.Networld;
using SociallyDistant.OS.Devices;

namespace SociallyDistant.Player
{
	public struct PlayerInstance
	{
		public IInitProcess OsInitProcess;
		public PlayerComputer Computer;

		//public UiManager UiManager;
		
		//public GameObject UiRoot;
		public PlayerFileOverrider FileOverrider;

		public LocalAreaNetwork PlayerLan;

		public ISkillTree SkillTree;
	}
}