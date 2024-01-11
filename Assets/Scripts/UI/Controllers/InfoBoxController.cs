#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using AcidicGui.Widgets;
using Shell;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UI.Widgets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityExtensions;
using ButtonWidget = UI.Widgets.ButtonWidget;

namespace UI.Controllers
{
	public class InfoBoxController : 
		UIBehaviour,
		IList<InfoBoxController.ButtonDefinition>
	{
		[Header("Settings")]
		[SerializeField]
		private string title = string.Empty;

		[TextArea]
		[SerializeField]
		private string text = string.Empty;
		
		[SerializeField]
		private CommonColor widgetColor;

		[SerializeField]
		private List<ButtonDefinition> buttons = new List<ButtonDefinition>();
		
		[Header("UI")]
		[SerializeField]
		private Rectangle indicator = null!;

		[SerializeField]
		private TextMeshProUGUI titleComponent = null!;

		[SerializeField]
		private TextMeshProUGUI messageTextComponent = null!;

		[SerializeField]
		private StaticWidgetList widgets = null!;

		[SerializeField]
		private RectTransform buttonsArea = null!;

		[SerializeField]
		private ButtonWidget buttonWidgetTemplate = null!;

		private List<ButtonWidget> buttonInstances = new List<ButtonWidget>();
		
		public string Title
		{
			get => title;
			set
			{
				title = value;
				titleComponent.SetText(title);
			}
		}

		public string Text
		{
			get => text;
			set
			{
				text = value;
				messageTextComponent.SetText(text);
			}
		}

		public CommonColor Color
		{
			get => widgetColor;
			set
			{
				widgetColor = value;
				UpdateColors();
			}
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(InfoBoxController));
			base.Awake();

			UpdateButtons();
		}

		private void UpdateColors()
		{
			Color color = this.Color.GetColor();
			
			var translucentColor = new Color(color.r, color.g, color.b, 0.25f);

			this.titleComponent.color = color;
			this.indicator.ShapeProperties.OutlineColor = color;
			this.indicator.ShapeProperties.FillColor = translucentColor;
			this.indicator.ForceMeshUpdate();
		}

		private void UpdateButtons()
		{
			while (buttons.Count < buttonInstances.Count)
			{
				ButtonWidget instance = buttonInstances[^1];

				if (instance == null)
					continue;
				
				Destroy(instance.gameObject);
				buttonInstances.RemoveAt(buttonInstances.Count - 1);
			}

			while (buttons.Count > buttonInstances.Count)
			{
				ButtonWidget instance = Instantiate(buttonWidgetTemplate, buttonsArea);
				buttonInstances.Add(instance);
			}

			for (var i = 0; i < buttons.Count; i++)
			{
				ButtonDefinition definition = buttons[i];
				ButtonWidget instance = buttonInstances[i];

				if (!instance.gameObject.activeSelf)
					instance.gameObject.SetActive(true);

				instance.Text = definition.label;
				instance.Clicked = _ => definition.clickHandler?.Invoke();
			}
		}
		
		/// <inheritdoc />
		protected override void OnValidate()
		{
			base.OnValidate();

			if (titleComponent != null)
				titleComponent.SetText(title);

			if (messageTextComponent != null)
				messageTextComponent.SetText(text);

			UpdateColors();
		}

		/// <inheritdoc />
		public IEnumerator<ButtonDefinition> GetEnumerator()
		{
			return buttons.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(ButtonDefinition item)
		{
			buttons.Add(item);
			UpdateButtons();
		}

		/// <inheritdoc />
		public void Clear()
		{
			buttons.Clear();
			UpdateButtons();
		}

		/// <inheritdoc />
		public bool Contains(ButtonDefinition item)
		{
			return buttons.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(ButtonDefinition[] array, int arrayIndex)
		{
			buttons.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(ButtonDefinition item)
		{
			if (!buttons.Remove(item))
				return false;

			UpdateButtons();
			return true;
		}

		/// <inheritdoc />
		public int Count => buttons.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public int IndexOf(ButtonDefinition item)
		{
			return buttons.IndexOf(item);
		}

		/// <inheritdoc />
		public void Insert(int index, ButtonDefinition item)
		{
			buttons.Insert(index, item);
			UpdateButtons();
		}

		/// <inheritdoc />
		public void RemoveAt(int index)
		{
			buttons.RemoveAt(index);
			UpdateButtons();
		}

		/// <inheritdoc />
		public ButtonDefinition this[int index]
		{
			get => buttons[index];
			set
			{
				buttons[index] = value;
				UpdateButtons();
			}
		}
		
		[Serializable]
		public struct ButtonDefinition
		{
			public string label;
			public Action? clickHandler;
		}
	}
}