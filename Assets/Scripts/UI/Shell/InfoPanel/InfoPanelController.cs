#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace UI.Shell.InfoPanel
{
	public class InfoPanelController : MonoBehaviour
	{
		private Dictionary<int, InfoWidgetController> widgetControllers = new Dictionary<int, InfoWidgetController>();

		[SerializeField]
		private InfoPanelService infoPanelService = null!;

		[SerializeField]
		private RectTransform widgetsArea = null!;
		
		[SerializeField]
		private InfoWidgetController widgetTemplate = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(InfoPanelController));

			widgetTemplate.gameObject.SetActive(false);
		}

		private void Start()
		{
			infoPanelService.WidgetList.ItemAdded += HandleWidgetAdded;
			infoPanelService.WidgetList.ItemRemoved += HandleWidgetRemoved;
		}

		private void HandleWidgetRemoved(InfoWidgetData data)
		{
			// Simply destroy and remove the widget
			if (widgetControllers.TryGetValue(data.Id, out InfoWidgetController controller))
			{
				Destroy(controller.gameObject);
				widgetControllers.Remove(data.Id);
			}
		}

		private void HandleWidgetAdded(InfoWidgetData data)
		{
			// Create a widget
			if (!widgetControllers.TryGetValue(data.Id, out InfoWidgetController widget))
			{
				widget = Instantiate(widgetTemplate, widgetsArea);
				widgetControllers.Add(data.Id, widget);
				widget.gameObject.SetActive(true);
			}

			widget.SetData(data);
		}
	}
}