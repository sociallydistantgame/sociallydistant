#nullable enable
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.OS.Network;

namespace SociallyDistant.Core.OS.Devices
{
	public abstract class NetworkService : INetworkService
	{
		private readonly ISystemProcess process;


		public IUser User => process.User;
		public IComputer Computer => User.Computer;
		protected INetworkConnection? Network => Computer.Network;
		public IVirtualFileSystem FileSystem => Computer.GetFileSystem(User);

		public async Task<ISystemProcess> Fork(IUser? forkUser = null)
		{
			if (forkUser != null)
				return await process.ForkAsUser(forkUser);

			return await process.Fork();
		}
		
		public NetworkService(ISystemProcess process)
		{
			this.process = process;
		}

		/// <inheritdoc />
		public abstract void Start();

		/// <inheritdoc />
		public abstract void Update();

		/// <inheritdoc />
		public abstract void Stop();
	}
	
	public interface INetworkService
	{
		void Start();
		void Update();
		void Stop();
	}
}