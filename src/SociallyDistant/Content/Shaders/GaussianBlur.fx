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

// These constants define the array length of the bell curve.
// If you change them, you must also change the corresponding values in
// BackgroundBlurWidgetEffect.cs.
#define MAXIMUM_BLURRINESS 16
#define MAX_DISTANCE 3

struct VSOut {
    float4 position : SV_Position;
    float4 color : COLOR0;
    float2 texcoord : TEXCOORD0;
};

// Texture that's to be blurred.
sampler2D MainTexture : register(s0);

float Curve[MAXIMUM_BLURRINESS * MAX_DISTANCE];

float2 TexelSize;
float Blurriness;

#define BLURX(uv, weight, kernel) tex2D(MainTexture, float2(uv.x + TexelSize.x * kernel, uv.y)) * weight
#define BLURY(uv, weight, kernel) tex2D(MainTexture, float2(uv.x, uv.y + TexelSize.y * kernel)) * weight

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
    float sampleCount = ceil(min(MAXIMUM_BLURRINESS, Blurriness) * MAX_DISTANCE);
    float4 sum = float4(0,0,0,0);
    
    sum += BLURX(data.texcoord, Curve[0], 0);
    
    for (float i = 1; i <= sampleCount; i++)
    {
        sum += BLURX(data.texcoord, Curve[i], -i);
        sum += BLURX(data.texcoord, Curve[i], i);
    }
    
    return sum;
}

float4 VerticalBlur(VSOut data) : COLOR0
{
    float sampleCount = ceil(min(MAXIMUM_BLURRINESS, Blurriness) * MAX_DISTANCE);
    float4 sum = float4(0,0,0,0);
    
    sum += BLURY(data.texcoord, Curve[0], 0);
    
    for (float i = 1; i <= sampleCount; i++)
    {
        sum += BLURY(data.texcoord, Curve[i], -i);
        sum += BLURY(data.texcoord, Curve[i], i);
    }
    
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
