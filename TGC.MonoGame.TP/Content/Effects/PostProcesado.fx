#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

float aberrationAmount = 0.005;

texture baseTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// texture bloomTexture;
// sampler2D bloomTextureSampler = sampler_state
// {
//     Texture = (bloomTexture);
//     MagFilter = Linear;
//     MinFilter = Linear;
//     AddressU = Clamp;
//     AddressV = Clamp;
// };



VertexShaderOutput PostProcessVS(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
    float2 uv = input.TexCoord;
    // Chromatic aberration
    float red = tex2D(textureSampler, uv).r;
    float green = tex2D(textureSampler, uv + float2(aberrationAmount, 0)).g;
    float blue = tex2D(textureSampler, uv + float2(-aberrationAmount, 0)).b;
    float4 color = float4(red, green, blue, 1.0);
    // float4 bloom = tex2D(bloomTextureSampler, input.TexCoord);
    // color += bloom;
    return color;
}

technique Integrate{
	pass P0{
		VertexShader = compile VS_SHADERMODEL PostProcessVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
}
};