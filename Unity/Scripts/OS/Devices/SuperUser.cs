#nullable enable
namespace OS.Devices
{
	public class SuperUser : IUser
	{
		/// <inheritdoc />
		public int Id => 0;

		/// <inheritdoc />
		public string UserName => "root";

		/// <inheritdoc />
		public string Home => "/root";

		/// <inheritdoc />
		public PrivilegeLevel PrivilegeLevel => PrivilegeLevel.Root;

		/// <inheritdoc />
		public IComputer Computer { get; private set; }

		/// <inheritdoc />
		public bool CheckPassword(string password)
		{
			// root doesn't have a password.
			return true;
		}

		public SuperUser(IComputer computer)
		{
			this.Computer = computer;
		}
	}
}