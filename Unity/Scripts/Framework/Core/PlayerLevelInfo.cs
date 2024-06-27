#nullable enable
namespace Core
{
	public struct PlayerLevelInfo
	{
		public readonly ulong Experience;
		public readonly ulong NextStep;
		public readonly float Progress;
		public readonly int Level;
		
		public PlayerLevelInfo(ulong experience, int level, ulong nextLevel, float progress)
		{
			Experience = experience;
			Level = level;
			NextStep = nextLevel;
			Progress = progress;
		}
	}
}