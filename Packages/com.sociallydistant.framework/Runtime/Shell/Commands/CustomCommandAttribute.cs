using System;

namespace Shell.Commands
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CustomCommandAttribute : Attribute
	{
		public string Name { get; private set; }

		public CustomCommandAttribute(string name)
		{
			this.Name = name;
		}
		
		public bool RequiresAdmin { get; set; }
		public bool PlayerOnly { get; set; }
	}
}