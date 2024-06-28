#nullable enable

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Controllers
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Toggle), typeof(Image))]
	public class CheckboxController : UIBehaviour
	{
		[Header("UI")]
		
		private Toggle toggle = null!;

		
		private Image targetGraphic = null!;

		[Header("Settings")]
		
		private Sprite uncheckedIdle = null!;
		
		
		private Sprite checkedIdle = null!;
		
		[Header("Unchecked")]
		
		private SpriteState uncheckedState = new SpriteState();
		
		[Header("Checked")]
		
		private SpriteState checkedState = new SpriteState();

		private bool? wasChecked;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(CheckboxController));
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			UpdateGraphics();
		}

		private void Update()
		{
			if (toggle == null)
				return;
			
			bool? previous = wasChecked;
			bool current = toggle.isOn;

			wasChecked = current;

			if (previous == current)
				return;

			UpdateGraphics();
		}

		private void UpdateGraphics()
		{
			if (targetGraphic == null)
				return;

			if (toggle == null)
				return;
			
			if (wasChecked == true)
			{
				targetGraphic.sprite = checkedIdle;
				toggle.spriteState = checkedState;
			}
			else
			{
				targetGraphic.sprite = uncheckedIdle;
				toggle.spriteState = uncheckedState;
			}
		}
	}
}