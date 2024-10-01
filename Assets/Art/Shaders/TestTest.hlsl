#ifndef TEST
#define TEST
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"


void MyFcn_float(Texture2D MainTex, SamplerState ss, float2 uv, out float3 Out)
{
    float4 color = SAMPLE_TEXTURE2D(MainTex, ss, uv);
    Out = color.rgb;
}

#endif