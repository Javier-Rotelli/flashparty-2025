#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#include "hlslNoise.inc.fx"


float TAU = 6.283185; 
float PI = 3.141592;
float bars = 16.;
float3 barColorA = float3(0.98,0.96,0.);
float3 barColorB = float3(0.1,0.2,0.9);

float4x4 World;
float4x4 View;
float4x4 Projection;

float Time = 0;
float i = 0;

float2 mapToCircle(float2 p, float r)
{
    return float2(atan2(p.y,p.x), length(p) - r);
}

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	// Clear the output
	VertexShaderOutput output = (VertexShaderOutput)0;
	// Model space to World space
	output.WorldPosition = mul(input.Position, World);
	// World space to View space
	float4 viewPosition = mul(output.WorldPosition, View);
	// View space to Projection space
	output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	return output;
}

static float minValue = 0.2;
static float maxValue = 0.8;
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TexCoord * 2 - 1;
    float2 coords = mapToCircle(uv,0.5);
    float l = length(uv);
    if(l < minValue || l > maxValue)
        discard;

    float opacity = smoothstep(minValue,0.4,l) - smoothstep(0.5,maxValue,l);
    
    float ring = floor(coords.y * 10);
	
    float noise = snoise(float3(uv + i,Time));
	
	float noise2 = snoise(float3(uv * 3.0,Time * 0.5 - i));

	float noise3 = snoise(float3(uv * cos(Time),ring));

	float3 albedo = float3(1.,0.,1.) * noise + float3(0.,1.,1.) * noise2;// + float3(1.,1.,0.) * noise3;
	albedo = saturate(albedo);
    return float4(albedo,opacity);
}

technique BlinnPhong{
	pass P0{
		VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
        
        // Enable alpha blending
        AlphaBlendEnable = TRUE;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
        BlendOp = Add;
}
};