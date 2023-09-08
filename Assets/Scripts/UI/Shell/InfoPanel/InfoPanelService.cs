#nullable enable

using System;
using System.Collections.Generic;
using Architecture;
using Core.Systems;
using Shell;
using Shell.InfoPanel;
using UniRx;
using UnityEngine;

namespace UI.Shell.InfoPanel
{
	[CreateAssetMenu(menuName = "ScriptableObject/Services/Info Panel Service")]
	public class InfoPanelService : 
		ScriptableObject,
		IInfoPanelService
	{
		private readonly UniqueIntGenerator idGenerator = new UniqueIntGenerator();
		private readonly ReactiveCollection<InfoWidgetData> widgets = new ReactiveCollection<InfoWidgetData>();

		public IReadOnlyReactiveCollection<InfoWidgetData> WidgetsObservable => widgets;

		private void Awake()
		{
			this.widgets.Clear();
		}

		public void ClearAllWidgets()
		{
			this.widgets.Clear();
		}

		public void CloseWidget(int widgetId)
		{
			for (var i = 0; i < this.widgets.Count; i++)
			{
				if (widgets[i].Id != widgetId) 
					continue;
				
				widgets.RemoveAt(i);
			}
		}
		
		public int CreateCloseableInfoWidget(string icon, string title, string message)
		{
			return AddWidgetInternal(new InfoWidgetCreationData()
			{
				Title = title,
				Icon = icon,
				Text = message,
				Closeable = true
			});
		}
		
		public int CreateStickyInfoWidget(string icon, string title, string message)
		{
			return AddWidgetInternal(new InfoWidgetCreationData()
			{
				Title = title,
				Icon = icon,
				Text = message,
				Closeable = false
			}, true);
		}
        
		private int AddWidgetInternal(InfoWidgetCreationData creationData, bool sticky = false)
		{
			int id = this.idGenerator.GetNextValue();

			var widgetData = new InfoWidgetData()
			{
				Id = id,
				CreationData = creationData
			};

			if (sticky)
				this.widgets.Insert(0, widgetData);
			else
				this.widgets.Add(widgetData);
            
            return id;
		}
	}
}