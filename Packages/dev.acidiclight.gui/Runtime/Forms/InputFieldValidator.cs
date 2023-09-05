#nullable enable
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityExtensions;

namespace AcidicGui.Forms
{
	public class InputFieldValidator : FormValidator
	{
		[Header("Requirements")]
		[SerializeField]
		private bool mustBeNonEmpty;

		[SerializeField]
		private bool mustNotBeWhitespace;
		
		[SerializeField]
		private int minCharacters;
		
		[SerializeField]
		private int maxCharacters;
		
		private TMP_InputField inputField;

		/// <inheritdoc />
		protected override void OnAwake()
		{
			this.MustGetComponent(out inputField);
			base.OnAwake();
		}

		private void OnEnable()
		{
			inputField.onSubmit.AddListener(OnSubmit);
		}

		private void OnDisable()
		{
			inputField.onSubmit.RemoveListener(OnSubmit);
		}

		private void OnSubmit(string value)
		{
			Form.TrySubmit();
		}
		
		/// <inheritdoc />
		public override bool Validate()
		{
			if (mustBeNonEmpty && string.IsNullOrEmpty(inputField.text))
				return false;

			if (mustNotBeWhitespace && string.IsNullOrWhiteSpace(inputField.text))
				return false;
			
			if (inputField.text.Length > maxCharacters)
				return false;

			if (inputField.text.Length < minCharacters)
				return false;

			return true;
		}
	}
}