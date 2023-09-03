using UnityEngine;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.Demos.Common.CommandPanels
{
	public class LabelWithToggle : MonoBehaviour
	{
		public Text labelText;
		public Toggle toggle;


		public LabelWithToggle Init(string text = "")
		{
			labelText.text = text;

			return this;
		}
	}
}
