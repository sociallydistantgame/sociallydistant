#nullable enable
using System.ComponentModel;
using Core;
using OS.Devices;
using Shell;
using Shell.Common;
using Shell.Windowing;
using UI.Shell;
using UI.Windowing;
using UnityEngine;
using UnityEngine.Serialization;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Assets/Program")]
	public class UguiProgram :
		ScriptableObject,
		IProgram,
		INamedAsset
	{
		[SerializeField]
		private string binaryName = string.Empty;

		[SerializeField]
		private string windowTitle = string.Empty;

		[SerializeField]
		private RectTransform programGuiPrefab = null!;
		
		[SerializeField]
		private CompositeIcon programIcon;
		
		/// <inheritdoc />
		public string Name => binaryName;
		
		/// <inheritdoc />
		public string BinaryName => this.binaryName;
		
		/// <inheritdoc />
		public string WindowTitle => this.windowTitle;

		public CompositeIcon Icon => this.programIcon;
		
		/// <inheritdoc />
		public void InstantiateIntoWindow(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
		{
			// Create a RectTransformContent to instantiate the program into.
			var windowContent = new RectTransformContent();
			
			RectTransform programRect = Instantiate(this.programGuiPrefab, windowContent.RectTransform);
			
			// Set the name of the process
			process.Name = binaryName;
			
			// Set window attributes
			window.Title = this.WindowTitle;
			window.Icon = this.Icon;
			
			// Find a ProcessKillHandler component on the spawned program. If
			// we don't find one, we must add one.
			ProcessKillHandler? killHandler = programRect.gameObject.GetComponentInChildren<ProcessKillHandler>(true);
			if (killHandler == null)
				killHandler = programRect.gameObject.AddComponent<ProcessKillHandler>();
			
			// Associate the program with its window
			window.Content = windowContent;
			
			// Find all components implementing IProgramOpenHandler and fire off the event.
			IProgramOpenHandler[] openHandlers = programRect.gameObject.GetComponentsInChildren<IProgramOpenHandler>(true);
			foreach (IProgramOpenHandler handler in openHandlers)
			{
				handler.OnProgramOpen(process, window, console, args);
			}

			// Enable the program rect so Awake gets called
			programRect.gameObject.SetActive(true);
		}
	}
}