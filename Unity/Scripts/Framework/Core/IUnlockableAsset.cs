#nullable enable
namespace Core
{
	public interface IUnlockableAsset : INamedAsset
	{
		bool IsUnlocked(ISkillTree skills);
		bool CanUnlock(ISkillTree skills);
		bool Unlock(ISkillTree skills);
	}
}