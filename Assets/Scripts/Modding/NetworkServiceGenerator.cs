#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ContentManagement;
using Modules;
using OS.Devices;

namespace Modding
{
	public sealed class NetworkServiceGenerator : IContentGenerator
	{
		private readonly IModuleManager moduleManager;

		public NetworkServiceGenerator(IModuleManager moduleManager)
		{
			this.moduleManager = moduleManager;
		}
		
		/// <inheritdoc />
		public IEnumerable<IGameContent> CreateContent()
		{
			foreach (Assembly assembly in moduleManager.Modules.Select(x => x.GetType().Assembly).Distinct())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (!typeof(INetworkService).IsAssignableFrom(type))
						continue;

					NetworkServiceAttribute? attribute = type.GetCustomAttributes(false)
						.OfType<NetworkServiceAttribute>()
						.FirstOrDefault();

					if (attribute == null)
						continue;
					
					ConstructorInfo? constructor = type.GetConstructor(new[] { typeof(ISystemProcess) });
					if (constructor == null)
						continue;

					var provider = new ReflectionNetworkServiceProvider(type, attribute.Id);

					yield return provider;
				}
			}
		}
	}
}