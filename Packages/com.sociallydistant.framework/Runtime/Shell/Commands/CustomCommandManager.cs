using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement;
using Modules;
using System.Reflection;

namespace Shell.Commands
{
	public class CustomCommandManager : IContentGenerator
	{
		private readonly IModuleManager moduleManager;

		public CustomCommandManager(IModuleManager moduleManager)
		{
			this.moduleManager = moduleManager;
		}

		private IEnumerable<IGameContent> GetCommandsInAssembly(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (!typeof(CustomCommand).IsAssignableFrom(type))
					continue;

				if (type.IsAbstract)
					continue;

				CustomCommandAttribute? attribute = type.GetCustomAttributes(false)
					.OfType<CustomCommandAttribute>()
					.FirstOrDefault();
				if (attribute == null)
					continue;

				if (type.GetConstructor(Type.EmptyTypes) == null)
					continue;

				yield return new CustomCommandAsset(type, attribute);
			}
		}
		
		/// <inheritdoc />
		public IEnumerable<IGameContent> CreateContent()
		{
			foreach (IGameContent command in GetCommandsInAssembly(this.GetType().Assembly))
			{
				yield return command;
			}
			
			foreach (GameModule module in moduleManager.Modules)
			{
				foreach (IGameContent command in GetCommandsInAssembly(module.GetType().Assembly))
					yield return command;
			}
		}
	}
}