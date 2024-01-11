#nullable enable
using Shell;
using UI.Shell;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Assets/Main Tool Group")]
	public class MainToolGroup : 
		ScriptableObject, 
		ITabbedToolDefinition
	{
		[SerializeField]
		private UguiProgram program = null!;

		[SerializeField]
		private bool allowUserTabs;

		public IProgram Program => program;
		public bool AllowUserTabs => allowUserTabs;
	}
	
	
}