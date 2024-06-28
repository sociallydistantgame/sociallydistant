#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Hacking;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.GameplaySystems.Hacking
{
	public class PayloadAsset : IPayload
	{
		
		private string payloadName;
		
		/// <inheritdoc />
		public string Name => payloadName;

		/// <inheritdoc />
		public bool IsUnlocked(ISkillTree skills)
		{
			return true;
		}

		/// <inheritdoc />
		public bool CanUnlock(ISkillTree skills)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public bool Unlock(ISkillTree skills)
		{
			throw new NotImplementedException();
		}

		public void Run(ISystemProcess process, ConsoleWrapper console)
		{
			System.Diagnostics.Process.Start("https://youtu.be/K7Hn1rPQouU");
		}
	}
}