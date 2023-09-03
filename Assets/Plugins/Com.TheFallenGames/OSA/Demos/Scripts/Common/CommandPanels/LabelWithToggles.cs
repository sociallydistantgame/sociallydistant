using System;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other;

namespace Com.TheFallenGames.OSA.Demos.Common.CommandPanels
{
	public class LabelWithToggles : MonoBehaviour
	{
		[SerializeField]
		LabelWithToggle _SubItemPrefab = null;

		public event Action<int, bool> ToggleChanged;

		public Text mainLabel;
		public ToggleGroup toggleGroup;

		[NonSerialized]
		public LabelWithToggle[] subItems;

		public bool Interactable { set { foreach (var subItem in subItems) if (subItem) subItem.toggle.interactable = value; } }


		//int _ToggleIndexForCurrentEvent;
		

		public LabelWithToggles Init(string mainLabelStr, params string[] names)
		{
			if (subItems != null)
				DestroySubItems();

			mainLabel.text = mainLabelStr;

			int i = 0;
			var subItemsList =
				DotNETCoreCompat.ConvertAll(
					names,
					n =>
					{
						if (string.IsNullOrEmpty(n))
							return null;

						var lwt = (Instantiate(_SubItemPrefab.gameObject) as GameObject).GetComponent<LabelWithToggle>();
						lwt.gameObject.SetActive(true);
						lwt.transform.SetParent(toggleGroup.transform, false);
						lwt.Init(n);
						toggleGroup.RegisterToggle(lwt.toggle);

						var copyOfI = i++;

						lwt.toggle.onValueChanged.AddListener(isOn =>
						{
							if (ToggleChanged != null)
								ToggleChanged(copyOfI, isOn);
						});

						return lwt;
					}
				);

			// Only keep the non-null ones (the ones with names)
			subItems = subItemsList.FindAll(si => si != null).ToArray();

			return this;
		}

		void DestroySubItems()
		{
			foreach (var subItem in subItems)
			{
				if (subItem)
				{
					if (subItem.toggle)
						subItem.toggle.onValueChanged.RemoveAllListeners();
					Destroy(subItem.gameObject);
				}
			}
		}

		void OnDestroy()
		{
			ToggleChanged = null;
			if (subItems != null)
				DestroySubItems();
		}
	}
}
