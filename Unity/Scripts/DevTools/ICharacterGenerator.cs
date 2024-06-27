#nullable enable
using System.Threading.Tasks;
using ContentManagement;
using Core;
using Modules;

namespace DevTools
{
	public interface ICharacterGenerator : IGameContent
	{
		Task GenerateNpcs(IWorldManager world);
	}
}