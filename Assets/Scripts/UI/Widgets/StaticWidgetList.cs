using System.Collections.Generic;
using AcidicGui.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets
{
	public class StaticWidgetList : MonoBehaviour
	{
		[SerializeField]
		private SystemWidgets widgetAssembler = null!;

		private readonly List<StaticWidgetHolder> widgetHolders = new List<StaticWidgetHolder>();
		
		public void UpdateWidgetList(IList<IWidget> widgets)
		{
			// Recycle all existing widget controllers, because we don't know which ones are going to stay in use.
			foreach (StaticWidgetHolder holder in widgetHolders)
			{
				widgetAssembler.RecycleBin.Recycle(holder.WidgetController);
				holder.WidgetController = null;
			}
			
			// Recycle any unneeded widgets
			while (this.widgetHolders.Count > widgets.Count)
			{
				Destroy(widgetHolders[^1].ViewRoot.gameObject);
				
				widgetHolders.RemoveAt(widgetHolders.Count - 1);
			}
			
			// Allocate any we need.
			while (widgetHolders.Count < widgets.Count)
			{
				var holder = new StaticWidgetHolder();

				var go = new GameObject("StaticWidgetHolder");
				holder.ViewRoot = go.AddComponent<RectTransform>();
				
				holder.ViewRoot.SetParent((RectTransform) this.transform, false);

				var layoutGroup = go.AddComponent<VerticalLayoutGroup>();

				layoutGroup.childForceExpandWidth = false;
				layoutGroup.childForceExpandHeight = false;
				layoutGroup.childScaleWidth = true;
				layoutGroup.childScaleHeight = true;
				layoutGroup.childControlWidth = true;
				layoutGroup.childControlHeight = true;
                
				this.widgetHolders.Add(holder);
			}
			
			// Update/spawn all widgets!
			for (var i = 0; i < widgets.Count; i++)
			{
				StaticWidgetHolder holder = widgetHolders[i];

				holder.WidgetController = widgets[i].Build(widgetAssembler, holder.ViewRoot);

				holder.WidgetController.UpdateUI();
			}
		}

		private class StaticWidgetHolder
		{
			public RectTransform ViewRoot { get; set; }
			public WidgetController WidgetController { get; set; }
		}
	}
}