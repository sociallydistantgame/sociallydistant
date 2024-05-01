#nullable enable
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets
{
	[ExecuteInEditMode]
	public class ButtonBackgroundUpdater : UIBehaviour
	{
		[Header("Settings")]
		[SerializeField]
		private ButtonColor buttonColort;

		[SerializeField]
		private ButtonTextureSet blueInactive;
		
		[SerializeField]
		private ButtonTextureSet blueActive;
		
		[SerializeField]
		private ButtonTextureSet redInactive;
		
		[SerializeField]
		private ButtonTextureSet redActive;
		
		[Header("Components")]
		[SerializeField]
		private Button button = null!;

		[SerializeField]
		private Image image = null!;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ButtonBackgroundUpdater));
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			UpdateBackground();
		}

		private void UpdateBackground()
		{
			if (button == null)
				return;

			if (image == null)
				return;
			
			var buttonActive = false;
			
			ButtonTextureSet textureSet = this.buttonColort switch
			{
				ButtonColor.Blue when buttonActive => blueActive,
				ButtonColor.Blue when !buttonActive => blueInactive,
				ButtonColor.Red when buttonActive => redActive,
				ButtonColor.Red when !buttonActive => redInactive,
				_ => blueInactive
			};

			image.sprite = textureSet.defaultTexture;
			button.spriteState = textureSet.spriteState;
		}

		#if UNITY_EDITOR
		/// <inheritdoc />
		protected override void OnValidate()
		{
			UpdateBackground();
			base.OnValidate();
		}
		#endif

		public enum ButtonColor
		{
			Blue,
			Red
		}

		[Serializable]
		private struct ButtonTextureSet
		{
			public Sprite defaultTexture;
			public SpriteState spriteState;
		}
	}
}