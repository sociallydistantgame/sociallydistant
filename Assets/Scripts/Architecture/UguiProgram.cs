#nullable enable
using OS.Devices;
using UI.Shell;
using UI.Windowing;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Assets/Program")]
	public class UguiProgram :
		ScriptableObject,
		IProgram<RectTransform>
	{
		[SerializeField]
		private RectTransform programGuiPrefab = null!;
		
		/// <inheritdoc />
		public void InstantiateIntoWindow(ISystemProcess process, IWindowWithClient<RectTransform> window)
		{
			
		}
	}
}