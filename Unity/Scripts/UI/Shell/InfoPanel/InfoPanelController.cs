#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Shell.InfoPanel;
using UniRx.Triggers;
using UnityEngine;
using Utility;
using UniRx;
using UnityExtensions;

namespace UI.Shell.InfoPanel
{
	public class InfoPanelController : MonoBehaviour
	{
		
		private InfoPanelService infoPanelService = null!;
		
		
		private InfoWidgetsController widgetsArea = null!;
		
		private readonly List<InfoWidgetData> widgetList = new List<InfoWidgetData>();
		private IDisposable? widgetsObserver;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(InfoPanelController));
		}

		private IEnumerator Start()
		{
			yield return null;
			this.widgetsArea.SetItems(this.widgetList);

			widgetsObserver = infoPanelService.WidgetsObservable.ObserveCountChanged(true)
				.Subscribe(OnWidgetCountChanged);
		}

		private void OnDestroy()
		{
			widgetsObserver?.Dispose();
			widgetsObserver = null;
		}

		private void OnWidgetCountChanged(int newCount)
		{
			this.widgetList.Clear();
			this.widgetList.AddRange(this.infoPanelService.WidgetsObservable);

			this.widgetsArea.SetItems(this.widgetList);
		}
	}
}