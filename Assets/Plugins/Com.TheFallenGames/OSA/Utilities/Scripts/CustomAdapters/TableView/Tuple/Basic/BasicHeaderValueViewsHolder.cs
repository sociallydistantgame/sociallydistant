using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Input;
using frame8.Logic.Misc.Other.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple.Basic
{
	/// <summary>
	/// A views holder for a value inside the header.
	/// See <see cref="TupleValueViewsHolder"/>
	/// </summary>
	public class BasicHeaderValueViewsHolder : TupleValueViewsHolder
	{
		RectTransform _ArrowRT;


		public override void CollectViews()
		{
			base.CollectViews();

			_ArrowRT = root.GetComponentAtPath<RectTransform>("SortArrow");
		}

		public override void UpdateViews(object value, ITableColumns columnsProvider)
		{
			string asStr = (string)value;
			if (columnsProvider.GetColumnState(ItemIndex).CurrentlyReadOnly)
			{
				asStr += "\n<color=#00000030><size=10>Read-only</size></color>";
			}

			TextComponent.text = asStr;

			var sortType = columnsProvider.GetColumnState(ItemIndex).CurrentSortingType;
			UpdateArrowFromSortType(sortType);
		}

		void UpdateArrowFromSortType(TableValueSortType type)
		{
			if (!_ArrowRT)
				return;

			if (_ArrowRT)
			{
				bool valid = type != TableValueSortType.NONE;
				float scale;
				float zRotation;
				if (valid)
				{
					scale = 1f;
					zRotation = 90f * (type == TableValueSortType.ASCENDING ? 1f : -1f);
				}
				else
				{
					scale = .5f;
					zRotation = 0f;
				}
				_ArrowRT.localScale = Vector3.one * scale;
				var euler = _ArrowRT.localRotation.eulerAngles;
				euler.z = zRotation;
				_ArrowRT.localRotation = Quaternion.Euler(euler);
			}
		}
	}
}
