#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"

inline half3 CalculateAmbientLight(half3 normalWS)
{
    return SampleSH(normalWS);
}

inline half3 CalculateLightAmount(half4 positionOS, half3 normalWS)
{
    // Get the VertexNormalInputs of the vertex, which contains the normal in world space
    VertexNormalInputs positions = GetVertexNormalInputs(positionOS);

    // Main light
    Light light = GetMainLight();
    half3 lightAmount = LightingLambert(light.color, light.direction, positions.normalWS.xyz);
    half3 ambient = CalculateAmbientLight(normalWS);
    lightAmount += ambient;

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

// inline half3 SampleSpecular(half3 lightDirection, half3 viewDirWS)
// {
//     float3 halfwayDir = normalize(lightDirection + viewDirWS);
//     float spec = pow(saturate(dot(surfaceNormal, halfwayDir)), smoothness * 128.0);
//     return spec * lightColor;
// }

half3 SampleReflection(half4 positionCS, half3 positionWS, half3 worldNormal, half3 viewDir, half smoothness,
                       half occlusion = 1)
{
    // Compute the reflection direction
    half3 reflectionDir = reflect(-viewDir, worldNormal);

    // half perceptualRoughness = RoughnessToPerceptualRoughness(1.0 - smoothness);
    half perceptualRoughness = RoughnessToPerceptualRoughness(1.0 - smoothness);
    half2 screenUV = GetNormalizedScreenSpaceUV(positionCS);


    // Sample the reflection probe
    half3 reflection = GlossyEnvironmentReflection(
        reflectionDir,
        positionWS,
        perceptualRoughness,
        occlusion,
        screenUV
    );

    return reflection;
}
