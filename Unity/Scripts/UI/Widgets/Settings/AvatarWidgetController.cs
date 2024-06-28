#nullable enable
using System;
using AcidicGui.Widgets;
using Social;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets.Settings
{
	public sealed class AvatarWidgetController : WidgetController
	{
		
		private DefaultAvatarRenderer defaultAvatarRenderer = null!;
		
		
		private RawImage customAvatar = null!;

		
		private Texture2D? customAvatarTexture = null;

		
		private Color defaultColor = Color.yellow;

		
		private AvatarSize size = AvatarSize.Icon;
        
		private LayoutElement layout;

		public AvatarSize AvatarSize
		{
			get => size;
			set => size = value;
		}

		public Color DefaultAvatarColor
		{
			get => defaultColor;
			set => defaultColor = value;
		}

		public Texture2D? AvatarTexture
		{
			get => customAvatarTexture;
			set => customAvatarTexture = value;
		}

		private void Awake()
		{
			this.MustGetComponent(out layout);
		}

		private void OnEnable()
		{
			UpdateUI();
		}

		private void OnDisable()
		{
			this.customAvatar.gameObject.SetActive(false);
			this.defaultAvatarRenderer.gameObject.SetActive(false);
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			customAvatar.texture = AvatarTexture;
			
			defaultAvatarRenderer.gameObject.SetActive(AvatarTexture == null);
			customAvatar.gameObject.SetActive(this.AvatarTexture != null);
			defaultAvatarRenderer.HumanColor = DefaultAvatarColor;

			layout.minWidth = (int) AvatarSize;
			layout.minHeight = (int) AvatarSize;
			layout.preferredWidth = (int) AvatarSize;
			layout.preferredHeight = (int) AvatarSize;
			
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			customAvatar.texture = null;
		}
	}
}