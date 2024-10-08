#ifndef TEST
#define TEST
#include "Assets\Art\ShadersTestTest.hlsl"


void Test_float(Texture2D MainTex, SamplerState ss, float2 uv, out float3 Out)
{
    float4 color = SAMPLE_TEXTURE2D(MainTex, ss, uv);
    Out = color.rgb;
}

#endif