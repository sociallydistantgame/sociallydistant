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
		[SerializeField]
		private Toggle toggle = null!;

		[SerializeField]
		private Image targetGraphic = null!;

		[Header("Settings")]
		[SerializeField]
		private Sprite uncheckedIdle = null!;
		
		[SerializeField]
		private Sprite checkedIdle = null!;
		
		[Header("Unchecked")]
		[SerializeField]
		private SpriteState uncheckedState = new SpriteState();
		
		[Header("Checked")]
		[SerializeField]
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