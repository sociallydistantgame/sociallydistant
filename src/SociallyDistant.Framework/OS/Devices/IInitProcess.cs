#nullable enable
namespace SociallyDistant.Core.OS.Devices
{
	public interface IInitProcess : ISystemProcess
	{
		Task<ISystemProcess> CreateLoginProcess(IUser user);
	}
}