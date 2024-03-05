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

		/// <inheritdoc />
		public bool CheckPassword(string password)
		{
			// Players don't have passwords.
			return false;
		}

		public PlayerUser(PlayerComputer computer, string username)
		{
			if (string.IsNullOrWhiteSpace(username))
				username = "user";
			
			Computer = computer;
			UserName = username;
			Home = $"/home/{username}";
		}

		internal void Rename(string newUsername)
		{
			if (string.IsNullOrWhiteSpace(newUsername))
				newUsername = "user";
			
			this.UserName = newUsername;
			this.Home = $"/home/{newUsername.Replace("/", "_")}";
		}
	}
}