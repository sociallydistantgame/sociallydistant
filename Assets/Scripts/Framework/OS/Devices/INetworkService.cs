#nullable enable
using OS.FileSystems;
using OS.Network;

namespace OS.Devices
{
	public abstract class NetworkService : INetworkService
	{
		private readonly ISystemProcess process;


		public IUser User => process.User;
		public IComputer Computer => User.Computer;
		protected INetworkConnection? Network => Computer.Network;
		public IVirtualFileSystem FileSystem => Computer.GetFileSystem(User);
		
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