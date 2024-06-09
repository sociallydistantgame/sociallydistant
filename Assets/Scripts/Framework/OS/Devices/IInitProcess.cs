#nullable enable
using System.Threading.Tasks;
namespace OS.Devices
{
	public interface IInitProcess : ISystemProcess
	{
		Task<ISystemProcess> CreateLoginProcess(IUser user);
	}
}