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
		IProgram<RectTransform>,
		INamedAsset
	{
		[SerializeField]
		private string binaryName = string.Empty;

		[SerializeField]
		private string windowTitle = string.Empty;

		[SerializeField]
		private RectTransform programGuiPrefab = null!;

		/// <inheritdoc />
		public string Name => binaryName;
		
		/// <inheritdoc />
		public string BinaryName => this.binaryName;
		
		/// <inheritdoc />
		public string WindowTitle => this.windowTitle;
		
		/// <inheritdoc />
		public void InstantiateIntoWindow(ISystemProcess process, IWindowWithClient<RectTransform> window)
		{
			// Delay execution of Awake until we're ready.
			programGuiPrefab.gameObject.SetActive(false);
			RectTransform programRect = Instantiate(this.programGuiPrefab);
			programGuiPrefab.gameObject.SetActive(true);

			// Set the name of the process
			process.Name = binaryName;
			
			// Set window attributes
			window.Title = this.WindowTitle;
			
			// Find a ProcessKillHandler component on the spawned program. If
			// we don't find one, we must add one.
			ProcessKillHandler? killHandler = programRect.gameObject.GetComponentInChildren<ProcessKillHandler>(true);
			if (killHandler == null)
				killHandler = programRect.gameObject.AddComponent<ProcessKillHandler>();
			
			// Associate the program with its window
			window.SetClient(programRect);
			
			// Find all components implementing IProgramOpenHandler and fire off the event.
			IProgramOpenHandler[] openHandlers = programRect.gameObject.GetComponentsInChildren<IProgramOpenHandler>(true);
			foreach (IProgramOpenHandler handler in openHandlers)
			{
				handler.OnProgramOpen(process, window);
			}

			// Enable the program rect so Awake gets called
			programRect.gameObject.SetActive(true);
		}
	}
}