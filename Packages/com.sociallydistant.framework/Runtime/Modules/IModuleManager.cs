#nullable enable
using System.Collections.Generic;

namespace Modules
{
	public interface IModuleManager
	{
		IEnumerable<GameModule> Modules { get; }
	}
}