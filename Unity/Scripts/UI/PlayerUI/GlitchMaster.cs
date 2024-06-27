#nullable enable
using System;
using DesktopEffects.XorgGlitch;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityExtensions;

namespace UI.PlayerUI
{
	public sealed class GlitchMaster : MonoBehaviour
	{
		private bool gameRequestsGlitchBands;
		private bool allowGlitchBands;
		private PostProcessVolume volume = null!;

		public bool AllowGlitchBands
		{
			get => allowGlitchBands;
			set
			{
				if (allowGlitchBands == value)
					return;

				allowGlitchBands = value;
				UpdateEffects();
			}
		}

		private void Awake()
		{
			this.MustGetComponent(out volume);
			UpdateEffects();
		}

		private void UpdateEffects()
		{
			if (volume.profile.TryGetSettings(out XorgGlitchEffectSettings glitchBands))
			{
				glitchBands.enabled.value = gameRequestsGlitchBands && allowGlitchBands;
			}
		}
	}
}