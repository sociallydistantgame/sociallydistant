#nullable enable
using System;

namespace Core.Scripting
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class FunctionAttribute : Attribute
	{
		private readonly string name;

		public string Name => name;

		public FunctionAttribute(string name)
		{
			this.name = name;
		}
	}
}