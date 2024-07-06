#nullable enable
namespace SociallyDistant.Core.OS.Devices
{
	public interface IInitProcess : ISystemProcess
	{
		ISystemProcess CreateLoginProcess(IUser user);
	}
}