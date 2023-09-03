using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.CustomAdapters.DateTimePicker;

namespace Com.TheFallenGames.OSA.Demos.DateTimePicker
{
	public class ShowDateTimePickerButton : MonoBehaviour
	{
		void Start()
        {
			var b = GetComponent<Button>();
			if (!b)
				b = gameObject.AddComponent<Button>();
			b.onClick.AddListener(() => DateTimePickerDialog.Show(null));
		}
	}
}
