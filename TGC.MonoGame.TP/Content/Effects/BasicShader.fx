#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Custom Effects - https://docs.monogame.net/articles/content/custom_effects.html
// High-level shader language (HLSL) - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl
// Programming guide for HLSL - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-pguide
// Reference for HLSL - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-reference
// HLSL Semantics - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InverseTransposeWorld;

// Light parameters
float3 ambientColor;  // Light's Ambient Color
float3 diffuseColor;  // Light's Diffuse Color
float3 specularColor; // Light's Specular Color
float KAmbient;
float KDiffuse;
float KSpecular;
float shininess;
float3 lightPosition;
float3 eyePosition; // Camera position

sampler2D ColormapSampler = sampler_state
{
	Texture = (Colormap);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = WRAP;
	AddressV = WRAP;
};

float3 DiffuseColor;

float Time = 0;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float4 Normal : TEXCOORD2;
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

	output.Normal = mul(input.Normal, InverseTransposeWorld);
	output.TexCoord = input.TexCoord;

	return output;
}
float4 getLight(float4 worldPosition, float3 normal, float4 texelColor)
{
	// Base vectors
	float3 lightDirection = normalize(lightPosition - worldPosition.xyz);
	float3 viewDirection = normalize(eyePosition - worldPosition.xyz);
	float3 halfVector = normalize(lightDirection + viewDirection);

	// Calculate the diffuse light
	float NdotL = saturate(dot(normal.xyz, lightDirection));
	float3 diffuseLight = KDiffuse * diffuseColor * NdotL;

	// Calculate the specular light
	float NdotH = dot(normal.xyz, halfVector);
	float3 specularLight = sign(NdotL) * KSpecular * specularColor * pow(saturate(NdotH), shininess);

	// Final calculation
	float4 finalColor = float4(saturate(ambientColor * KAmbient + diffuseLight) * texelColor.rgb + specularLight, texelColor.a);

	return finalColor;
}
float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 textureColor = tex2D(ColormapSampler, input.TexCoord);
	float4 finalColor = getLight(input.WorldPosition, input.Normal.xyz, textureColor);
	return finalColor;
}


VertexShaderOutput PlaneVS(in VertexShaderInput input)
{
	// Clear the output
	VertexShaderOutput output = (VertexShaderOutput)0;

	input.Position.y = cos(input.Position.x * 0.1) + sin(input.Position.z * 0.5 + Time) * 6;
	float3 tanget = float3(1, -0.1 * sin(input.Position.x * 0.1), 0);
	float3 bitanget = float3(0, 0.5 * cos(input.Position.z * 0.5 + Time), 1);
	input.Normal = normalize(float4((tanget, bitanget), 0));
	// Model space to World space
	output.WorldPosition = mul(input.Position, World);
	// World space to View space
	float4 viewPosition = mul(output.WorldPosition, View);
	// View space to Projection space
	output.Position = mul(viewPosition, Projection);

	output.Normal = mul(input.Normal, InverseTransposeWorld);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 PlanePS(VertexShaderOutput input) : COLOR
{
	// float3 normal = cross(ddx(input.WorldPosition.xyz), ddy(input.WorldPosition.xyz));
	// normal = normalize(normal);
	float4 finalColor = getLight(input.WorldPosition, input.Normal.xyz, float4(DiffuseColor, 1));
	return finalColor;
}

technique BlinnPhong{
	pass P0{
		VertexShader = compile VS_SHADERMODEL MainVS();
PixelShader = compile PS_SHADERMODEL MainPS();
}
};

technique Plane {
	pass P0{
		VertexShader = compile VS_SHADERMODEL PlaneVS();
		PixelShader = compile PS_SHADERMODEL PlanePS();
	}
};