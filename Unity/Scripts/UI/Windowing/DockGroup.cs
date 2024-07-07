#nullable enable
using Shell.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Shell.Dock;
using UnityEditor;
using UnityExtensions;

namespace UI.Windowing
{
	public class DockGroup : 
		UIBehaviour,
		IList<DockGroup.IconDefinition>
	{
		
		private DockIcon iconPrefab = null!;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DockGroup));
			base.Awake();
		}

		private void UpdateDefinition(int index, IconDefinition data)
		{
			DockIcon iconInstance = icons[index];

			iconInstance.NotificationGroup = data.NotificationGroup;
			iconInstance.UpdateIcon(data);
		}
		
		private void UpdateDefinitions()
		{
			// Remove any unneeded UI
			while (icons.Count > definitions.Count)
			{
				DockIcon instance = icons[^1];
				icons.RemoveAt(icons.Count - 1);
				
				Destroy(instance.gameObject);
			}
			
			// Create any new ones
			while (icons.Count < definitions.Count)
			{
				DockIcon instance = Instantiate(iconPrefab, this.transform);
				icons.Add(instance);
			}
			
			// Annnd update data!
			for (var i = 0; i < definitions.Count; i++)
			{
				IconDefinition definition = definitions[i];
				UpdateDefinition(i, definition);
			}
		}
	}
}