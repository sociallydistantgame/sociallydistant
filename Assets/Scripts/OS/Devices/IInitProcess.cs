#nullable enable
namespace OS.Devices
{
	public interface IInitProcess : ISystemProcess
	{
		ISystemProcess CreateLoginProcess(IUser user);
	}
}