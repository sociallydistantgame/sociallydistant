
namespace Com.TheFallenGames.OSA.Demos.Hierarchy
{
	public interface IHierarchyNodeModel
	{
		IHierarchyNodeModel Parent { get; set; }
		IHierarchyNodeModel[] Children { get; set; }
		int Depth { get; set;  }

		/// <summary>Directories can be expanded or not</summary>
		bool Expanded { get; set; }

		/// <summary>Convention: If children array is not null, this is a directory</summary>
		bool IsDirectory();
	}
}
