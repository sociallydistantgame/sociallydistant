#nullable enable
using System;
using TMPro;
using UnityEngine;
using UnityExtensions;

namespace AcidicGui.Forms
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class ErrorLabel : MonoBehaviour
	{
		private TextMeshProUGUI text = null!;

		private void Awake()
		{
			this.MustGetComponent(out text);
		}

		public void SetError(string? error)
		{
			text.SetText(error);
		}
		
		#if UNITY_EDITOR

		private void OnValidate()
		{
			if (!TryGetComponent(out text))
				return;
			
			SetError(null);
		}

#endif
	}
}