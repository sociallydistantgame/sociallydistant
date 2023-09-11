#nullable enable
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AcidicGui.Widgets
{
	public class WidgetRecycleBin
	{
		private readonly Dictionary<Type, Stack<WidgetController>> recycleBin = new Dictionary<Type, Stack<WidgetController>>();
		private readonly GameObject recycleBinRoot = new GameObject("Widget Recycle Bin");

		public T? TryGetRecycled<T>(RectTransform destination)
			where T : WidgetController
		{
			if (!recycleBin.TryGetValue(typeof(T), out Stack<WidgetController>? stack))
				return null;

			if (stack.Count == 0)
				return null;

			WidgetController latest = stack.Peek();

			if (latest is not T desiredWidget)
				return null;

			stack.Pop();

			if (stack.Count == 0)
				recycleBin.Remove(typeof(T));
			
			Vector3 scale = desiredWidget.transform.localScale;
			
			desiredWidget.transform.SetParent(destination, false);
			desiredWidget.transform.localScale = scale;
			desiredWidget.gameObject.SetActive(true);
			return desiredWidget;
		}
		
		public void Recycle(WidgetController widget)
		{
			widget.OnRecycle();
			
			widget.gameObject.SetActive(false);

			Type type = widget.GetType();

			Vector3 scale = widget.transform.localScale;
			
			widget.transform.SetParent(recycleBinRoot.transform, false);

			widget.transform.localScale = scale;

			if (!recycleBin.TryGetValue(type, out Stack<WidgetController>? stack))
			{
				stack = new Stack<WidgetController>();
				recycleBin.Add(type, stack);
			}

			stack.Push(widget);
		}
	}
}