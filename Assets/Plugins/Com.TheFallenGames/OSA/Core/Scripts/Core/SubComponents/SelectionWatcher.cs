using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.TheFallenGames.OSA.Core.SubComponents
{
	public class SelectionWatcher
	{
		public delegate void NewObjectSelectedDelegate(GameObject lastGO, GameObject newGO);

		public event NewObjectSelectedDelegate NewObjectSelected;

		public bool Enabled { get; set; }
		GameObject LastSelectedObject { get; set; }

		public void OnUpdate()
		{
			CheckNewObjectSelection();
		}

		bool CheckNewObjectSelection()
		{
			var last = LastSelectedObject;
			if (!Enabled)
			{
				LastSelectedObject = null;
				return last != LastSelectedObject;
			}

			var curSelected = GetCurrentlySelectedObject();
			if (!curSelected)
			{
				LastSelectedObject = null;
				return last != LastSelectedObject;
			}

			if (LastSelectedObject != curSelected)
			{
				if (curSelected)
					OnNewObjectSelected(curSelected);

				LastSelectedObject = curSelected;
				return true;
			}

			return false;
		}

		GameObject GetCurrentlySelectedObject()
		{
			if (!EventSystem.current)
				return null;

			return EventSystem.current.currentSelectedGameObject;
		}

		void OnNewObjectSelected(GameObject curSelected)
		{
			NewObjectSelected(LastSelectedObject, curSelected);
		}
	}
}
