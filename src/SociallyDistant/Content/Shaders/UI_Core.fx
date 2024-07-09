/**
 * SOCIALLY DISTANT - CORE USER INTERFACE SHADER
 *
 * This effect file contains all necessary shader code needed to render
 * the game's user interface. Do not modify it unless you seriously know
 * what you're doing, as it is very tightly coupled with the UI renderer
 * code. If you've never seen that code before, you may wanna go look at it
 * for a few hours before fuckin' around in here. Don't break Ritchie's
 * exquisitly-programmed UI render code D:
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

matrix TransformMatrix;

sampler2D MainTexture : register(s0);

float Opacity;


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
    return tex2D(MainTexture, data.texcoord) * data.color;
}

technique BasicRender
{
    pass BlitThyPixels 
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL BasicPixelShader();
    }
}
