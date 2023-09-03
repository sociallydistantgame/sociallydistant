using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.DateTimePicker;
using Com.TheFallenGames.OSA.Demos.Common.Drawer;

namespace Com.TheFallenGames.OSA.Demos.DateTimePicker
{
	/// <summary>Implementing multiple adapters to get a generic picker which returns a <see cref="DateTimePickerDialog"/> object</summary>
	public class DateTimePickerSimpleSceneEntry : MonoBehaviour
	{
		void Start()
        {
			var drawer = GameObject.Find(typeof(DrawerCommandPanel).Name).GetComponent<DrawerCommandPanel>();

			drawer.Init(new IOSA[0], false, false, false, false, false, false);
			drawer.AddButtonsWithOptionalInputPanel("Show another").button1.onClick.AddListener(Show);
			drawer.galleryEffectSetting.gameObject.SetActive(false);
			Show();
		}

		public void Show() { DateTimePickerDialog.Show(null); }
	}
}
