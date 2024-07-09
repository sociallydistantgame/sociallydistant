/**
 * GaussianBlur.fx
 *
 * This shader is used by the BackgroundBlurEffect widget effect. It is a two-pass Gaussian blur
 * implementation that's used to render a blurred texture. 
 *
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

// Texture that's to be blurred.
sampler2D MainTexture : register(s0);

float2 TexelSize;
float BlurAmount;

#define BLURX(uv, weight, kernel) tex2D(MainTexture, float2(uv.x + TexelSize.x * kernel * BlurAmount, uv.y)) * weight
#define BLURY(uv, weight, kernel) tex2D(MainTexture, float2(uv.x, uv.y + TexelSize.y * kernel * BlurAmount)) * weight

VSOut VS(float4 position : SV_Position, float4 color : COLOR0, float2 texcoord : TEXCOORD0)
{
    VSOut vsout;
    vsout.position = position;
    vsout.color = color;
    vsout.texcoord = texcoord;   
    return vsout;
}

float4 HorizontalBlur(VSOut data) : COLOR0
{
    float4 sum = float4(0,0,0,0);

    sum += BLURX(data.texcoord, 0.05, -4.0);
    sum += BLURX(data.texcoord, 0.09, -3.0);
    sum += BLURX(data.texcoord, 0.12, -2.0);
    sum += BLURX(data.texcoord, 0.15, -1.0);
    sum += BLURX(data.texcoord, 0.18, 0.0);
    sum += BLURX(data.texcoord, 0.15, +1.0);
    sum += BLURX(data.texcoord, 0.12, +2.0);
    sum += BLURX(data.texcoord, 0.09, +3.0);
    sum += BLURX(data.texcoord, 0.05, +4.0);
    
    return sum;
}

float4 VerticalBlur(VSOut data) : COLOR0
{
    float4 sum = float4(0,0,0,0);

    sum += BLURY(data.texcoord, 0.05, -4.0);
    sum += BLURY(data.texcoord, 0.09, -3.0);
    sum += BLURY(data.texcoord, 0.12, -2.0);
    sum += BLURY(data.texcoord, 0.15, -1.0);
    sum += BLURY(data.texcoord, 0.18, 0.0);
    sum += BLURY(data.texcoord, 0.15, +1.0);
    sum += BLURY(data.texcoord, 0.12, +2.0);
    sum += BLURY(data.texcoord, 0.09, +3.0);
    sum += BLURY(data.texcoord, 0.05, +4.0);
    
    return sum;
}


technique GaussianBlurTwoPass
{
    pass HorizontalPass 
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL HorizontalBlur();
    }
    
    pass VerticalPass 
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL VerticalBlur();
    }
}
