#nullable enable

using System;

namespace Core.Config
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SystemSettingsAttribute : Attribute
	{
		public SystemSettingsAttribute(string categoryName)
		{
			MetaCategoryName = categoryName;
		}
		
		public string MetaCategoryName { get; }
	}
}