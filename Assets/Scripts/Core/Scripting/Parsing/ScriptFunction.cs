#nullable enable
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Parsing
{
	public class ScriptFunction
	{
		public async Task<bool> ExecuteAsync(string name, string[] args, ITextConsole console)
		{
			return false;
		}
	}
}