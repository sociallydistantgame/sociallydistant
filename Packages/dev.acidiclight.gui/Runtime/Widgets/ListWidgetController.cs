using UnityEngine;
using UnityEngine.UI;

namespace AcidicGui.Widgets
{
	public class ListWidgetController : WidgetController
	{
		[SerializeField]
		private ToggleGroup toggleGroup = null!;

		public ToggleGroup ToggleGroup => toggleGroup;
		
		public bool AllowSelectNone { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			this.ToggleGroup.allowSwitchOff = this.AllowSelectNone;
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
		}
	}
}