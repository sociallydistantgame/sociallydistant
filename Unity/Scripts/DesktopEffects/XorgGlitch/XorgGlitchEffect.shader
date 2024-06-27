Shader "Hidden/XorgGlitchEffect"
{
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

        float thicknessOscillation = (sin(_Time) + 1) / 2;
        float y = (sin(_Time*20) + 1) / 2;
        float timeWave = (cos(_Time * 60) + 1) / 2;
        float thickness = 0.05 * thicknessOscillation;

        float bandStart = y - (thickness/2);
        float bandEnd = y + (thickness/2);
        float tolerance = 0.0069;

        if (abs(y - timeWave) <= tolerance)
        {
            if (i.texcoord.y >= bandStart && i.texcoord.y <= bandEnd)
            {
                return float4(1 - color.r, 1 - color.g, 1 - color.b, 1);
            }
        }
        return color;
    }
    
    ENDHLSL

SubShader
  {
      Cull Off ZWrite Off ZTest Always
      Pass
      {
          HLSLPROGRAM
              #pragma vertex VertDefault
              #pragma fragment Frag
          ENDHLSL
      }
  }
}