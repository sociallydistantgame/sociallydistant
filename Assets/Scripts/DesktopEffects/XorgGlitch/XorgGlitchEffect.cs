#nullable enable

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace DesktopEffects.XorgGlitch
{
	public sealed class XorgGlitchEffect : PostProcessEffectRenderer<XorgGlitchEffectSettings>
	{
		/// <inheritdoc />
		public override void Render(PostProcessRenderContext context)
		{
			PropertySheet? sheet = context.propertySheets.Get(Shader.Find("Hidden/XorgGlitchEffect"));
			context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
		}
	}
}