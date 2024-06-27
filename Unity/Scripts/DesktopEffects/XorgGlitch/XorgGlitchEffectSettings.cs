#nullable enable
using System;
using UnityEngine.Rendering.PostProcessing;

namespace DesktopEffects.XorgGlitch
{
	[Serializable]
	[PostProcess(typeof(XorgGlitchEffect), PostProcessEvent.BeforeStack, "Xorg screen glitches", false)]
	public sealed class XorgGlitchEffectSettings : PostProcessEffectSettings
	{
		
	}
}