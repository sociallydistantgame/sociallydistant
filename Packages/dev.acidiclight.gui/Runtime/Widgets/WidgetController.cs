#nullable enable

using UnityEngine;

namespace AcidicGui.Widgets
{
	public abstract class WidgetController : MonoBehaviour
	{
		public abstract void UpdateUI();
		public abstract void OnRecycle();
	}
}