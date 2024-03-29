#nullable enable

using System.Threading.Tasks;

namespace Shell
{
	public interface IShellContext
	{
		Task ShowInfoDialog(string title, string message);
	}
}