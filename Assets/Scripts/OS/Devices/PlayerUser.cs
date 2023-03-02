#nullable enable
namespace OS.Devices
{
	public class PlayerUser : IUser
	{
		/// <inheritdoc />
		public int Id => 1000;

		/// <inheritdoc />
		public string UserName { get; set; }

		/// <inheritdoc />
		public string Home { get; private set; }

		/// <inheritdoc />
		public PrivilegeLevel PrivilegeLevel => PrivilegeLevel.Admin;

		/// <inheritdoc />
		public IComputer Computer { get; private set; }

		public PlayerUser(PlayerComputer computer, string username)
		{
			Computer = computer;
			UserName = username;
			Home = $"/home/{username}";
		}
	}
}