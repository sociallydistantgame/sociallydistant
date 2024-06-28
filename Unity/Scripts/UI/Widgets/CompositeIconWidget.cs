#nullable enable

using System;
using System.Text.RegularExpressions;
using Shell.Common;
using TMPro;
using UI.PlayerUI;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Shell;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace UI.Widgets
{
	[ExecuteInEditMode]
	public class CompositeIconWidget : MonoBehaviour
	{
		[Header("Appearance")]
		
		private CompositeIcon icon;

		[Header("UI")]
		
		private TextMeshProUGUI? textIcon;

		
		private Image? spriteIcon;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(CompositeIconWidget));
		}

		private void Start()
		{
			UpdateIcon();
		}

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
			bool shouldBeActive = this.icon.spriteIcon != null || !string.IsNullOrWhiteSpace(this.icon.textIcon);
			
			if (this.gameObject.activeSelf != shouldBeActive)
				this.gameObject.SetActive(shouldBeActive);
			
			// prefer sprites over text
			if (textIcon != null && (icon.spriteIcon != null || string.IsNullOrEmpty(icon.textIcon)))
			{
				textIcon.color = default;

				if (spriteIcon != null)
				{
					spriteIcon.color = icon.iconColor.AsUnityColor();
					spriteIcon.sprite = icon.spriteIcon;
				}
			}
			else
			{
				if (textIcon != null)
				{
					textIcon.color = icon.iconColor.AsUnityColor();
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