using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;

namespace Com.TheFallenGames.OSA.Demos.Common.CommandPanels
{
	public class ButtonsWithOptionalInputPanel : WithInputPanel
	{
		public Button button1, button2, button3, button4;

		public bool Interactable { set { button1.interactable = button2.interactable = button3.interactable = button4.interactable = value; } }


		internal void Init(string label1, string label2, string label3, string label4, string inputFieldLabel)
		{
			button1.transform.GetComponentAtPath<Text>("Text").text = label1;
			button2.transform.GetComponentAtPath<Text>("Text").text = label2;
			button3.transform.GetComponentAtPath<Text>("Text").text = label3;
			button4.transform.GetComponentAtPath<Text>("Text").text = label4;

			button1.gameObject.SetActive(!string.IsNullOrEmpty(label1));
			button2.gameObject.SetActive(!string.IsNullOrEmpty(label2));
			button3.gameObject.SetActive(!string.IsNullOrEmpty(label3));
			button4.gameObject.SetActive(!string.IsNullOrEmpty(label4));

			if (inputField)
			{
				(inputField.placeholder as Text).text = inputFieldLabel;
				inputField.gameObject.SetActive(!string.IsNullOrEmpty(inputFieldLabel));
			}
		}
	}
}
