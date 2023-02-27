#nullable enable

using System.Collections.Generic;
using Architecture;
using Core.Systems;
using UnityEngine;

namespace UI.Shell.InfoPanel
{
	[CreateAssetMenu(menuName = "ScriptableObject/Services/Info Panel Service")]
	public class InfoPanelService : ScriptableObject
	{
		private readonly UniqueIntGenerator idGenerator = new UniqueIntGenerator();
		private readonly ObservableList<InfoWidgetData> widgets = new ObservableList<InfoWidgetData>();
		private readonly Dictionary<int, int> widgetIdMap = new Dictionary<int, int>();

		public IReadOnlyObservableList<InfoWidgetData> WidgetList => widgets;

		public void DeleteWidget(int id)
		{
			if (!widgetIdMap.TryGetValue(id, out int index))
				return;

			widgets.Remove(widgets[index]);
			widgetIdMap.Remove(id);
			idGenerator.DeclareUnused(id);
		}

		public void ModifyWidget(int id, InfoWidgetCreationData creationData)
		{
			if (!widgetIdMap.TryGetValue(id, out int index))
				return;

			InfoWidgetData data = widgets[index];

			widgets.Remove(data);
			
			data.CreationData = creationData;

			foreach (int otherId in widgetIdMap.Keys)
			{
				if (otherId == id)
					widgetIdMap[otherId] = widgets.Count;
				else if (widgetIdMap[otherId] > index)
					widgetIdMap[otherId] --;
			}
			
			widgets.Add(data);
		}

		public bool TryGetCreationData(int id, out InfoWidgetCreationData creationData)
		{
			creationData = default;

			if (!widgetIdMap.TryGetValue(id, out int index))
				return false;

			creationData = widgets[index].CreationData;
			return true;
		}

		public int Create(InfoWidgetCreationData creationData)
		{
			int id = idGenerator.GetNextValue();
			widgetIdMap.Add(id, widgets.Count);

			var widget = new InfoWidgetData
			{
				Id = id,
				CreationData = creationData
			};

			widgets.Add(widget);
			return id;
		}
	}
}