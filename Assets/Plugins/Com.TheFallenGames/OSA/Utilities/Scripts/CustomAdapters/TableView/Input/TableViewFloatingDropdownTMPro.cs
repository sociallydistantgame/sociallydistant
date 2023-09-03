using System;
using UnityEngine;
using UnityEngine.Events;


namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Input
{
#if OSA_TV_TMPRO
	public class TableViewFloatingDropdownTMPro : TMPro.TMP_Dropdown, ITableViewFloatingDropdown
	{
		public event Action Closed;
		public int OptionsCount { get { return options.Count; } }
		UnityEvent<int> ITableViewFloatingDropdown.onValueChanged { get { return base.onValueChanged; } }

		public new DropdownEvent onValueChanged
		{
			get { throw new InvalidOperationException("FloatingDropdown.onValueChanged: Not available for this class"); }
			set { throw new InvalidOperationException("FloatingDropdown.onValueChanged: Not available for this class"); }
		}
		public new void Show() { throw new InvalidOperationException("FloatingDropdown.Show() Not available for this class "); }

		void ITableViewFloatingDropdown.Show() { base.Show(); }

		protected override void DestroyDropdownList(GameObject dropdownList)
		{
			base.DestroyDropdownList(dropdownList);

			if (Closed != null)
				Closed();
		}
	}
#endif
}