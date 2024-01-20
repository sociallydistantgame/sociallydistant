#nullable enable
using OS.Devices;

namespace Core.Scripting
{
	public sealed class HypervisorUser : IUser
	{
		private readonly IComputer computer;
		
		/// <inheritdoc />
		public int Id => 0;

		/// <inheritdoc />
		public string UserName => "worker";

		/// <inheritdoc />
		public string Home => "/";

		/// <inheritdoc />
		public PrivilegeLevel PrivilegeLevel => PrivilegeLevel.Root;

		/// <inheritdoc />
		public IComputer Computer => computer;

		/// <inheritdoc />
		public bool CheckPassword(string password)
		{
			return false;
		}

		internal HypervisorUser()
		{
			computer = new HypervisorComputer(this);
		}
	}
}