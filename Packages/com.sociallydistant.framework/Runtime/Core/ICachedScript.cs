#nullable enable
using System.Threading.Tasks;

namespace Core
{
	public interface ICachedScript
	{
		Task RebuildScriptTree();
	}
}