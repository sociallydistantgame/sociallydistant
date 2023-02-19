#nullable enable

namespace OS.Devices
{
	public class LocalComputer : IComputer
	{
		/// <inheritdoc />
		public bool FindUserById(int id, out IUser? user)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public bool FindUserByName(string username, out IUser? user)
		{
			throw new System.NotImplementedException();
		}
	}
}