#nullable enable

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AcidicGui.Forms
{
	public class Form : MonoBehaviour
	{
		public class FormSubmittedEvent : UnityEvent {}
		
		[Tooltip("Button that acts as the form's submit button. When pressed, this button will validate the form and fire the form's OnSubmit event.")]
		
		private Button? submitButton;

		[Tooltip("Whether to trigger form submission when a text entry commits edited text to its value. Note: Multi-line text entries are ignored.")]
		
		private bool submitOnTextEntry = true;

		
		private FormSubmittedEvent onSubmit = new FormSubmittedEvent();

		public FormSubmittedEvent OnSubmit => onSubmit;

		public bool TrySubmit()
		{
			// Find any child IFormValidator components
			IFormValidator[] validators = this.GetComponentsInChildren<IFormValidator>();

			var valid = true;

			foreach (IFormValidator validator in validators)
			{
				if (validator.Validate())
					continue;

				valid = false;
				break;
			}
			
			if (valid)
				onSubmit.Invoke();
			
			return valid;
		}

		public void SetError(string? error)
		{
			
		}
	}
}