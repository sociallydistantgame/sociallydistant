using System.Collections.Generic;
using System.Text;

namespace OS.Devices
{
	public interface IAutoCompleteSource
	{
		IReadOnlyList<string> GetCompletions(StringBuilder text, out int insertionPoint);
	}
}