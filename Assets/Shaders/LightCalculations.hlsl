#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"

inline half3 CalculateLightAmount(half4 positionOS, half3 normalWS)
{
    // Get the VertexNormalInputs of the vertex, which contains the normal in world space
    VertexNormalInputs positions = GetVertexNormalInputs(positionOS);

    // Main light
    Light light = GetMainLight();
    half3 lightAmount = LightingLambert(light.color, light.direction, positions.normalWS.xyz);

    // Add additional lights
    uint additionalLightCount = GetAdditionalLightsCount();
    for (uint i = 0; i < additionalLightCount; i++)
    {
        Light additionalLight = GetAdditionalLight(i, TransformObjectToWorld(positionOS));
        lightAmount += LightingLambert(additionalLight.color, additionalLight.direction, normalWS) *
            additionalLight.distanceAttenuation * additionalLight.shadowAttenuation;
    }

    return saturate(lightAmount);
}
