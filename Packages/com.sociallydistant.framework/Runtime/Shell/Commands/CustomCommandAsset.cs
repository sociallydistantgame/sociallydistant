using System;
using Core;

namespace Shell.Commands
{
	public sealed class CustomCommandAsset : INamedAsset
	{
		private readonly Type commandType;
		private readonly CustomCommandAttribute attribute;

		public CustomCommandAsset(Type type, CustomCommandAttribute attribute)
		{
			this.commandType = type;
			this.attribute = attribute;
		}

		/// <inheritdoc />
		public string Name => attribute.Name;

		public bool IsPlayerOnly => attribute.PlayerOnly;
		public bool AdminRequired => attribute.RequiresAdmin;

		public CustomCommand CreateInstance()
		{
			return (CustomCommand) Activator.CreateInstance(commandType, null);
		}
	}
}