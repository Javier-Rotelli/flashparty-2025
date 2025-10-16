#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//#include "hlslNoise.inc.fx"


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
    float2 coords = mapToCircle(uv,0.);
    float l = length(uv);
    if(l < minValue || l > maxValue)
        discard;

    float opacity = 1;smoothstep(minValue,0.4,l) - smoothstep(0.5,maxValue,l);
    
    //float ring = floor(coords.y * 10 + frac(Time));
    float ring = floor(coords.y * 10);
    coords.y = frac(coords.y * 10);
    
    coords.x = frac(coords.x * 5);
    float3 n = float3(noise(float2(coords.x, i * 20)),noise(float2(coords.y, i * 10)), ring );
    return float4(n,opacity);
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

float hash(float2 p) { return frac(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x)))); }
float noise(float2 x) {
	float2 i = floor(x);
	float2 f = frac(x);

	// Four corners in 2D of a tile
	float a = hash(i);
	float b = hash(i + float2(1.0, 0.0));
	float c = hash(i + float2(0.0, 1.0));
	float d = hash(i + float2(1.0, 1.0));

	// Simple 2D lerp using smoothstep envelope between the values.
	// return float3(lerp(lerp(a, b, smoothstep(0.0, 1.0, f.x)),
	//			lerp(c, d, smoothstep(0.0, 1.0, f.x)),
	//			smoothstep(0.0, 1.0, f.y)));

	// Same code, with the clamps in smoothstep and common subexpressions
	// optimized away.
	float2 u = f * f * (3.0 - 2.0 * f);
	return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}