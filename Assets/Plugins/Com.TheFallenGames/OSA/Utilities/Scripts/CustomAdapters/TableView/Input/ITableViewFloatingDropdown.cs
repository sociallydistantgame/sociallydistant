using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Input
{
	public interface ITableViewFloatingDropdown
	{
		event Action Closed;
		UnityEvent<int> onValueChanged { get; }
		int value { get; set; }
		int OptionsCount { get; }
		void ClearOptions();
		void AddOptions(List<string> options);
		void Show();
		void Hide();
	}
}
