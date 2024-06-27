using UnityEngine;

namespace AcidicGui.Widgets
{
	public abstract class AnimatedHighlightDriver : MonoBehaviour
	{
		public abstract Color CurrentColor { get; set; }
	}
}