#nullable enable
namespace OS.Devices
{
	public interface IComputer
	{
		string Name { get; }
		bool FindUserById(int id, out IUser? user);
		bool FindUserByName(string username, out IUser? user);
	}
}