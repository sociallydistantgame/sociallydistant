#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Hacking
{
	public interface IPayload : IUnlockableAsset
	{
		void Run(ISystemProcess process, ConsoleWrapper console);
	}
}