using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.TableView;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Input;

namespace Com.TheFallenGames.OSA.Editor.OSAWizard.CustomAdapterConfigurators
{
	[CustomAdapterConfigurator(typeof(ITableAdapter))]
	public class TableAdapterConfigurator : ICustomAdapterConfigurator
	{
		public void ConfigureNewAdapter(IOSA adapter)
		{
			var tableAdapter = adapter as ITableAdapter;
			var p = tableAdapter.TableParameters;
			var transform = tableAdapter.AsMonoBehaviour.transform;
			p.Viewport = transform.Find("Viewport") as RectTransform;
			p.Content = p.Viewport.Find("Content") as RectTransform;

			var pTable = p.Table;
			var prefabsParent = transform.Find("Prefabs-Hidden");
			var tuplePrefabsParent = prefabsParent.Find("Tuple") as RectTransform;
			var columnsTuplePrefabsParent = prefabsParent.Find("ColumnsTuple") as RectTransform;
			var paths = CWiz.TV.Paths;

			// Some recommended spacing values
			p.ContentSpacing = 2;
			if (!p.IsHorizontal)
				p.ContentPadding.bottom = 320;
			pTable.ColumnsTupleSize = 60f;
			pTable.ColumnsTupleSpacing = 5f;

			// Only children scroll on X axis
			p.ScrollSensivityOnXAxis = 0f;

			// Scrollbar
			var sbTR = transform.Find("Scrollbar");
			if (sbTR)
			{
				p.Scrollbar = sbTR.GetComponent<Scrollbar>();
				OSAUtil.ConfigureDinamicallyCreatedScrollbar(p.Scrollbar, adapter, p.Viewport);
			}

			// Columns Scrollbar
			var csbTR = transform.Find("ColumnsScrollbar");
			if (csbTR)
				pTable.ColumnsScrollbar = csbTR.GetComponent<Scrollbar>();

			// Instantiate the tuple prefab & its value prefab
			pTable.TuplePrefab = InstantiateFromRes(paths.TUPLE_PREFAB, tuplePrefabsParent);
			var tupleAdapter = pTable.TuplePrefab.GetComponent(typeof(ITupleAdapter)) as ITupleAdapter;
			tupleAdapter.TupleParameters.ItemPrefab = InstantiateFromRes(paths.TUPLE_VALUE_PREFAB, tuplePrefabsParent);

			// Allowing resizing of individual tuples manually, by default
			tupleAdapter.TupleParameters.ResizingMode = TableResizingMode.MANUAL_TUPLES;

			// Instantiate the columns tuple prefab & its value prefab
			pTable.ColumnsTuplePrefab = InstantiateFromRes(paths.COLUMNS_TUPLE_PREFAB, columnsTuplePrefabsParent);
			var hTupleAdapter = pTable.ColumnsTuplePrefab.GetComponent(typeof(ITupleAdapter)) as ITupleAdapter;
			var hTupleValuePrefab = hTupleAdapter.TupleParameters.ItemPrefab = InstantiateFromRes(paths.COLUMNS_TUPLE_VALUE_PREFAB, columnsTuplePrefabsParent);
			DisableLayoutComponentsInTupleValuePrefab(hTupleValuePrefab);

			// Assign the floating text input and dropdown
			pTable.FloatingDropdownPrefab = Resources.Load<GameObject>(paths.INPUT__FLOATING_DROPDOWN).transform as RectTransform;
			pTable.TextInputControllerPrefab = Resources.Load<GameObject>(paths.INPUT__FLOATING_TEXT).GetComponent<TableViewTextInputController>();

			// Assign the options panel
			pTable.OptionsPanel = transform.Find("OptionsPanel") as RectTransform;

			Debug.Log("TableView instantiated. Resize mode was set to "+ tupleAdapter.TupleParameters.ResizingMode + 
				". If you don't plan to have individual rows resizeable, " +
				"go over the tuple & column prefabs and their associated value prefabs and remove " +
				"all Layout components, as they won't be needed. And set ResizeMode to NONE in inspector. " +
				"This will boost scrolling performance.",
				adapter.AsMonoBehaviour.gameObject
			);
		}

		RectTransform InstantiateFromRes(string respath, RectTransform inParent)
		{
			var pref = Resources.Load<GameObject>(respath);
			var inst = (GameObject.Instantiate(pref, inParent, false) as GameObject).transform as RectTransform;
			inst.name = inst.name.Replace("(Clone)", "");
			Debug.Log("Instantiated \"" + respath + "\"", inst.gameObject);

			return inst;
		}

		// This is to avoid the notification from TupleParams where it finds enabled layout components while resize mode is not NONE
		void DisableLayoutComponentsInTupleValuePrefab(RectTransform tupleValuePrefab)
		{
			var layoutGroup = tupleValuePrefab.GetComponent<LayoutGroup>();
			if (layoutGroup && layoutGroup.enabled)
				layoutGroup.enabled = false;

			foreach (RectTransform rt in tupleValuePrefab)
			{
				var l = rt.GetComponent<LayoutGroup>();
				if (l)
					l.enabled = false;

				var le = rt.GetComponent<LayoutElement>();
				if (le)
					le.enabled = false;
			}
		}
	}
}
