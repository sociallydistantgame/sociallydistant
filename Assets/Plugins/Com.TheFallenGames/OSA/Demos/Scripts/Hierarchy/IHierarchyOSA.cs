using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.Demos.Hierarchy
{
	public interface IHierarchyOSA : IOSA
	{
		/// <summary>Returns whether the toggle could be done</summary>
		bool ToggleDirectoryFoldout(int itemIndex);
	}
}
