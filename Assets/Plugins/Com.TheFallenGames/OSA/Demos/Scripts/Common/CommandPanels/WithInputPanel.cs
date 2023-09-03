using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.Demos.Common.CommandPanels
{
	public class WithInputPanel : MonoBehaviour
	{
		public InputField inputField;

		public float InputFieldValueAsFloat { get { try { return float.Parse(inputField.text); } catch { inputField.text = "0"; return 0f; } } }
		public int InputFieldValueAsInt {
			get
			{
				int final = 0;
				if (!int.TryParse(inputField.text, out final))
				{
					long v;
					if (long.TryParse(inputField.text, out v))
						final = OSAConst.MAX_ITEMS;
				}

				inputField.text = final + "";
				return final;
			}
		}
	}
}
