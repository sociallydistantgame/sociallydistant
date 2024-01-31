using System;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement;
using Modules;
using System.Reflection;

namespace Shell.Commands
{
	public class CustomCommandManager : IGameContentSource
	{
		private readonly IModuleManager moduleManager;

		public CustomCommandManager(IModuleManager moduleManager)
		{
			this.moduleManager = moduleManager;
		}

		private void GetCommandsInAssembly(Assembly assembly, ContentCollectionBuilder builder)
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

				var commandAsset = new CustomCommandAsset(type, attribute);
				builder.AddContent(commandAsset);
			}
		}

		/// <inheritdoc />
		public Task LoadAllContent(ContentCollectionBuilder builder)
		{
			GetCommandsInAssembly(this.GetType().Assembly, builder);
			
			foreach (GameModule module in moduleManager.Modules)
			{
				GetCommandsInAssembly(module.GetType().Assembly, builder);
			}

			return Task.CompletedTask;
		}

	}
}