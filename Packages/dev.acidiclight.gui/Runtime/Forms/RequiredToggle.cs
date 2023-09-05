#nullable enable
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityExtensions;

namespace AcidicGui.Forms
{
	[RequireComponent(typeof(Toggle))]
	public class RequiredToggle : FormValidator
	{
		[SerializeField]
		private string errorText = "Field must be checked";
		
		private Toggle toggle = null!;
		
		/// <inheritdoc />
		protected override void OnAwake()
		{
			this.MustGetComponent(out toggle);
		}

		/// <inheritdoc />
		public override bool Validate()
		{
			return toggle.isOn;
		}
	}
}