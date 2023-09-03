using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.Demos.Hierarchy
{
	public interface IHierarchyParams
	{
		IHierarchyNodeModel HierarchyRootNode { get; set; }

		/// <summary>Doesn't include descendants of non-expanded folders</summary>
		List<IHierarchyNodeModel> FlattenedVisibleHierarchy { get; set; }
	}
}
