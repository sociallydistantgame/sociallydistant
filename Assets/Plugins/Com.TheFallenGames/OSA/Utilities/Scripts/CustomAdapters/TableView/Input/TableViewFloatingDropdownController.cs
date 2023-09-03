using System;
using System.Collections.Generic;
using frame8.Logic.Misc.Other.Extensions;
using UnityEngine;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Input
{
	public class TableViewFloatingDropdownController : MonoBehaviour
	{
		ITableViewFloatingDropdown _Dropdown;
		Action<int> _CurrentCallback;
		int _ValueToReturn = -1;
		//Vector3 _PosToReset;
		List<object> _Values = new List<object>();
		// Excluding the Close option
		List<string> _ValueNames = new List<string>();


		protected void Awake()
		{
			(transform as RectTransform).pivot = new Vector2(0f, 1f);
			_Dropdown = GetComponent(typeof(ITableViewFloatingDropdown)) as ITableViewFloatingDropdown;
			_Dropdown.Closed += OnDropdownClosed;
		}


		public void InitWithEnum(Type enumType, bool sortByEnumNameInsteadOfValue = false, Action<List<object>> valuesFilter = null)
		{
			if (enumType == null || !enumType.IsEnum)
			{
				enumType = null;
				ClearOptionsAndTypes();
				return;
			}

			var values = Enum.GetValues(enumType);
			var list = new List<object>();
			var names = new List<string>();
			for (int i = 0; i < values.Length; i++)
			{
				var value = (int)values.GetValue(i);
				list.Add(value);
				names.Add(Enum.GetName(enumType, value));
			}

			if (valuesFilter != null)
				valuesFilter(list);

			if (sortByEnumNameInsteadOfValue)
				list.Sort((a, b) => Enum.GetName(enumType, a).CompareTo(Enum.GetName(enumType, b)));
			else
				list.Sort();

			InitWithValues(list, names);
		}

		public void ShowFloating(RectTransform atParent, Action<object> onValueSelected, object invalidValue)
		{
			Action<int> onSelected = i =>
			{
				if (onValueSelected == null || _Values == null || _Values.Count <= i)
					return;

				var val = i == -1 ? invalidValue : _Values[i];
				onValueSelected(val);
			};

			ShowFloating(atParent, onSelected);
		}

		public void ClearOptionsAndTypes()
		{
			_Values.Clear();
			_ValueNames.Clear();
			_Dropdown.ClearOptions();
		}

		public void Hide()
		{
			_Dropdown.Hide();
		}

		void InitWithValues(IList<object> values, IList<string> names)
		{
			ClearOptionsAndTypes();
			_Values.AddRange(values);
			_ValueNames.AddRange(names);
		}

		void ShowFloating(RectTransform atParent, Action<int> onSelected)
		{
			_CurrentCallback = onSelected;
			_ValueToReturn = -1;

			_Dropdown.onValueChanged.RemoveListener(OnValueChanged);

			gameObject.SetActive(true);
			transform.position = atParent.position;

			_Dropdown.ClearOptions();
			var options = new List<string>(_ValueNames); // modifying a copy

			options.Add("<Close>");
			_Dropdown.AddOptions(options);
			_Dropdown.value = options.Count - 1; // selecting an invalid value by default
			//RefreshShownValue();
			_Dropdown.Show();

			_Dropdown.onValueChanged.AddListener(OnValueChanged);

			//_PosToReset = transform.localPosition;
			var asRT = (transform as RectTransform);
			asRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, atParent.rect.width);
			asRT.TryClampPositionToParentBoundary();
		}

		void OnValueChanged(int value)
		{
			// The invalid value is returned as -1
			_ValueToReturn = value == _Dropdown.OptionsCount - 1 ? -1 : value;

			_Dropdown.onValueChanged.RemoveListener(OnValueChanged);
			_Dropdown.Hide();
		}

		void OnDropdownClosed()
		{
			gameObject.SetActive(false);
			if (_CurrentCallback != null)
			{
				var callback = _CurrentCallback;
				_CurrentCallback = null;
				callback(_ValueToReturn);
			}
		}
	}
}
