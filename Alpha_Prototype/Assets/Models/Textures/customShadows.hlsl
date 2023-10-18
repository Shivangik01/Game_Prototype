#ifndef CUSTOM_SHADOWS
#define CUSTOM_SHADOWS

#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
    #if (SHADERPASS != SHADERPASS_FORWARD)
        #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
    #endif
#endif

struct CustomLightingData {
    // Surface attributes
    float3 positionWS;
    float4 shadowCoord;
};

float CalculateCustomLighting(CustomLightingData d)
{
    #ifdef SHADERGRAPH_PREVIEW
    return 0;
    #else
    Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, 1);

    return mainLight.shadowAttenuation;
    #endif
}

void CalculateCustomLighting_float(float3 Position, out float shadow)
{
    CustomLightingData d;
    d.positionWS = Position;
#ifdef SHADERGRAPH_PREVIEW
    d.shadowCoord = 0;
#else

    float4 positionCS = TransformWorldToHClip(Position);
    #if SHADOWS_SCREEN
        d.shadowCoord = ComputeScreenPos(positionCS);
    #else
        d.shadowCoord = TransformWorldToShadowCoord(Position);
    #endif

#endif

    shadow = CalculateCustomLighting(d);
}

#endif