#nullable enable
using System;
using System.Threading.Tasks;
using OS.Devices;
using UnityEngine.Assertions;

namespace Modding
{
	public sealed class ReflectionNetworkServiceProvider : INetworkServiceProvider
	{
		private readonly Type type;
		
		/// <inheritdoc />
		public string Id { get; }

		public ReflectionNetworkServiceProvider(Type type, string id)
		{
			this.type = type;
			this.Id = id;
		}
		
		/// <inheritdoc />
		public INetworkService CreateService(ISystemProcess initProcess)
		{
			// work around the async
			ISystemProcess? fork = null;

			Task.Run(async () =>
			{
				fork = await initProcess.Fork();
			}).Wait();
			
			Assert.IsNotNull(fork);

			INetworkService service = (INetworkService) Activator.CreateInstance(type, fork!);

			return service;
		}
	}
}