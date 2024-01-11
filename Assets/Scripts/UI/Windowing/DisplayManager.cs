#nullable enable
using Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace UI.Windowing
{
	public class DisplayManager : UIBehaviour
	{
		[SerializeField]
		private DockGroup iconGroup = null!;
		
		[SerializeField]
		private RectTransform networkViewer = null!;

		private readonly DockGroup.IconDefinition[] definitions = new DockGroup.IconDefinition[]
		{
			new DockGroup.IconDefinition()
			{
				Label = "Network viewer",
				Icon = MaterialIcons.Web
			}
		};
		
		public bool ShowNetworkViewer
		{
			get => networkViewer.gameObject.activeSelf;
			set
			{
				if (ShowNetworkViewer == value)
					return;
				
				networkViewer.gameObject.SetActive(value);
				definitions[0].IsActive = ShowNetworkViewer;
				UpdateDock();
			}
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(DisplayManager));
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			BuildDock();
			UpdateDock();
		}

		private void BuildDock()
		{
			definitions[0].IsActive = ShowNetworkViewer;
			definitions[0].ClickHandler = ToggleNetworkViewer;
			
			this.iconGroup.Clear();

			foreach (DockGroup.IconDefinition definition in definitions)
			{
				iconGroup.Add(definition);
			}
		}
		
		private void UpdateDock()
		{
			for (var i = 0; i < definitions.Length; i++)
			{
				DockGroup.IconDefinition source = definitions[i];
				DockGroup.IconDefinition destination = iconGroup[i];

				if (source.IsActive == destination.IsActive)
					return;

				iconGroup[i] = source;
			}
		}

		public void ToggleNetworkViewer()
		{
			ShowNetworkViewer = !ShowNetworkViewer;
		}
	}
}