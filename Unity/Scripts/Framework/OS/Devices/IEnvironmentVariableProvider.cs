#nullable enable
using System.Collections.Generic;

namespace OS.Devices
{
	public interface IEnvironmentVariableProvider
	{
		IEnumerable<string> Keys { get; }

		string this[string key] { get; set; }

		bool IsSet(string variable);
		
		IEnvironmentVariableProvider DeepClone();
	}
}