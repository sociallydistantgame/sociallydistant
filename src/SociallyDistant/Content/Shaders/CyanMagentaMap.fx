/**
 * CyanMagentaMap.fx
 *
 * This shader takes a map texture containing Cyan and Magenta colors, and uses
 * it to interpolate between a user-defined "foreground" and "background" color.
 *
 * This shader is used by the Avatar widget to render default avatars.
 */

/* standard MG boilerplate */
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VSOut {
    float4 position : SV_Position;
    float4 color : COLOR0;
    float2 texcoord : TEXCOORD0;
};

// Parameters from the UI system
matrix TransformMatrix;
sampler2D MainTexture : register(s0);
float Opacity;

// Avatar parameters
float4 Foreground;
float4 Background;

VSOut VS(float4 position : SV_Position, float4 color : COLOR0, float2 texcoord : TEXCOORD0)
{
    VSOut vsout;
    vsout.position = mul(position, TransformMatrix);
    vsout.color = color * Opacity;
    vsout.texcoord = texcoord;   
    return vsout;
}

float4 BasicPixelShader(VSOut data) : COLOR0
{
    float4 mapColor = tex2D(MainTexture, data.texcoord);
    return lerp(Background, Foreground, mapColor.g) * data.color;
}

technique BasicRender
{
    pass BlitThyPixels 
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL BasicPixelShader();
    }
}
