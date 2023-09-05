#nullable enable
using System;
using UnityEditor;
using UnityEngine;
using UnityExtensions;

namespace AcidicGui.Forms
{
	public abstract class FormValidator :
		MonoBehaviour,
		IFormValidator
	{
		private Form form = null!;
		private ErrorLabel? errorLabel;

		protected Form Form => form;
		
		private void Awake()
		{
			this.MustGetComponentInParent(out form);

			errorLabel = GetComponentInChildren<ErrorLabel>();
			
			OnAwake();
		}

		protected void SetError(string? error)
		{
			if (errorLabel != null)
				errorLabel.SetError(error);
			else if (form != null)
				form.SetError(error);
				
		}

		/// <inheritdoc />
		public abstract bool Validate();
		
		protected virtual void OnAwake() {}
		
#if UNITY_EDITOR
		private void OnValidate()
		{
			form = this.GetComponentInParent<Form>();
		}
#endif
	}
}