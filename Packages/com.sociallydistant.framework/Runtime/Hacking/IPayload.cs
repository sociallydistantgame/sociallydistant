#nullable enable
using Core;
using OS.Devices;

namespace Hacking
{
	public interface IPayload : IUnlockableAsset
	{
		void Run(ISystemProcess process, ConsoleWrapper console);
	}
}