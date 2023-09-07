#nullable enable
using System.Collections.Generic;

namespace OS.Devices
{
	public interface IIterableEnvironmentVariableProvider : 
		IEnvironmentVariableProvider, 
		IEnumerable<KeyValuePair<string, string>>
	{
	}
}