using UnityEngine;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.Demos.Common.CommandPanels
{
	public class LabelWithSliderPanel : MonoBehaviour
	{
		public Text labelText, minLabelText, maxLabelText;
		public Slider slider;

		public bool Interactable { set { slider.interactable = value; } }


		public void Init(string label, string minLabel, string maxLabel)
		{
			labelText.text = label;
			minLabelText.text = minLabel;
			maxLabelText.text = maxLabel;
		}

		internal void Set(float min, float max, float val)
		{
			slider.minValue = min;
			slider.maxValue = max;
			slider.onValueChanged.Invoke(slider.value = val);
		}
	}
}
