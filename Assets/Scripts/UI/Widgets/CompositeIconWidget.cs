#nullable enable

using System;
using System.Text.RegularExpressions;
using Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace UI.Widgets
{
	[ExecuteInEditMode]
	public class CompositeIconWidget : MonoBehaviour
	{
		[Header("Appearance")]
		[SerializeField]
		private CompositeIcon icon;

		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI? textIcon;

		[SerializeField]
		private Image? spriteIcon;

		public CompositeIcon Icon
		{
			get => icon;
			set
			{
				icon = value;
				UpdateIcon();
			}
		}
		
		private void Update()
		{
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
				UpdateIcon();
#endif
		}

		private void UpdateIcon()
		{
			// prefer sprites over text
			if (textIcon != null && (icon.spriteIcon != null || string.IsNullOrEmpty(icon.textIcon)))
			{
				textIcon.color = default;

				if (spriteIcon != null)
				{
					spriteIcon.color = icon.iconColor;
					spriteIcon.sprite = icon.spriteIcon;
				}
			}
			else
			{
				if (textIcon != null)
				{
					textIcon.color = icon.iconColor;
					textIcon.SetText(Regex.Unescape(icon.textIcon));
				}

				if (spriteIcon != null)
				{
					spriteIcon.color = default;
					spriteIcon.sprite = null;
				}
			}
		}
	}
}