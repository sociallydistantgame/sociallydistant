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
		[SerializeField]
		private DockIcon iconPrefab = null!;

		private readonly List<DockIcon> icons = new List<DockIcon>();
		private readonly List<IconDefinition> definitions = new List<IconDefinition>();

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DockGroup));
			base.Awake();
		}

		private void UpdateDefinition(int index, IconDefinition data)
		{
			DockIcon iconInstance = icons[index];

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
		
		/// <inheritdoc />
		public IEnumerator<IconDefinition> GetEnumerator()
		{
			return definitions.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(IconDefinition item)
		{
			definitions.Add(item);
			UpdateDefinitions();
		}

		/// <inheritdoc />
		public void Clear()
		{
			definitions.Clear();
			UpdateDefinitions();
		}

		/// <inheritdoc />
		public bool Contains(IconDefinition item)
		{
			return definitions.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(IconDefinition[] array, int arrayIndex)
		{
			definitions.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(IconDefinition item)
		{
			if (!definitions.Remove(item))
				return false;

			UpdateDefinitions();
			return true;
		}

		/// <inheritdoc />
		public int Count => definitions.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public int IndexOf(IconDefinition item)
		{
			return definitions.IndexOf(item);
		}

		/// <inheritdoc />
		public void Insert(int index, IconDefinition item)
		{
			definitions.Insert(index, item);
			UpdateDefinitions();
		}

		/// <inheritdoc />
		public void RemoveAt(int index)
		{
			definitions.RemoveAt(index);
			UpdateDefinitions();
		}

		/// <inheritdoc />
		public IconDefinition this[int index]
		{
			get => definitions[index];
			set
			{
				definitions[index] = value;
				UpdateDefinition(index, value);
			}
		}
		
		public struct IconDefinition
		{
			public CompositeIcon Icon;
			public string Label;
			public bool IsActive;
			public Action? ClickHandler;
		}
	}
}