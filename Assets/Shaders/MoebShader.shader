Shader "Custom/URP/MoebiusEdgeDetection"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Albedo ("Albedo Color", Color) = (1, 1, 1, 1)
        _EdgeThreshold ("Edge Threshold", Range(0, 1)) = 0.1
        _Transparency ("Transparency", Range(0, 1)) = 1
        _WiggleStrength ("Wiggle Strength", Range(0.0, 1.0)) = 0.1
        _NoiseScale ("Noise Scale", Range(0.1, 10.0)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "EdgeDetectionPass"
            Tags
            {
                "LightMode"="UniversalForward"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_CameraNormalsTexture);
            SAMPLER(sampler_CameraNormalsTexture);

            float4 _Albedo;
            float _EdgeThreshold;
            float _Transparency;
            float _WiggleStrength;
            float _NoiseScale;

            struct attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                float4 lightmapUV : TEXCOORD1;
            };

            struct varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float2 uv : TEXCOORD0;
                float2 staticLightmapUV : TEXCOORD4;
            };

            varyings vert(attributes v)
            {
                varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.positionWS = TransformObjectToWorld(v.positionOS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.tangentWS = TransformObjectToWorldNormal(v.tangentOS.xyz); // World-space tangent
                o.uv = v.uv;
                o.staticLightmapUV = v.lightmapUV.xy;
                return o;
            }

            // Simple sine-based noise for UV distortion
            float2 GenerateNoiseUV(float2 uv)
            {
                float noiseX = sin(uv.y * _NoiseScale) * cos(uv.x * _NoiseScale);
                float noiseY = cos(uv.x * _NoiseScale) * sin(uv.y * _NoiseScale);
                return float2(noiseX, noiseY) * _WiggleStrength;
            }

            // Function to unpack normals from the normal map
            float3 UnpackNormalMap(float2 uv, float3 normalWS, float3 tangentWS)
            {
                // Sample the normal map
                float3 tangentNormal = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv).rgb;
                tangentNormal = normalize(tangentNormal * 2.0 - 1.0); // Transform from [0, 1] to [-1, 1]

                // Construct the tangent space matrix
                float3 bitangentWS = normalize(cross(normalWS, tangentWS)); // Bitangent
                float3x3 tangentToWorld = float3x3(tangentWS, bitangentWS, normalWS);

                // Transform tangent-space normal to world space
                return normalize(mul(tangentToWorld, tangentNormal));
            }


            float4 frag(varyings i) : SV_Target
            {
                // Screen-space texel size
                float2 texelSize = 1.0 / _ScreenParams.xy;

                // Apply noise to UVs for wiggly lines
                float2 wigglyUV = i.uv + GenerateNoiseUV(i.uv);
                // wigglyUV = i.uv;

                // Sobel filter gradients for albedo
                float gradientX_Albedo = 0.0, gradientY_Albedo = 0.0;

                // Sobel filter gradients for normals
                float gradientX_Normal = 0.0, gradientY_Normal = 0.0;

                // Compute normal map-modified normals
                float3 modifiedNormalWS = UnpackNormalMap(wigglyUV, normalize(i.normalWS), normalize(i.tangentWS));

                // Sobel edge detection on main texture and modified normals
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float sobelX = (x == -1 ? -1.0 : (x == 1 ? 1.0 : 0.0)) * (y == 0 ? 2.0 : 1.0);
                        float sobelY = (y == -1 ? -1.0 : (y == 1 ? 1.0 : 0.0)) * (x == 0 ? 2.0 : 1.0);
                        float2 offset = texelSize * float2(x, y);

                        // Sample main texture and normal map
                        float3 albedoSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, wigglyUV + offset).rgb;
                        float3 normalSample = UnpackNormalMap(wigglyUV + offset, normalize(i.normalWS),
                                                              normalize(i.tangentWS));

                        // Grayscale intensity from albedo
                        float intensity = dot(albedoSample, float3(0.333, 0.333, 0.333));

                        // Accumulate gradients
                        gradientX_Albedo += intensity * sobelX;
                        gradientY_Albedo += intensity * sobelY;

                        // Accumulate normal differences
                        gradientX_Normal += normalSample.x * sobelX;
                        gradientY_Normal += normalSample.y * sobelY;
                    }
                }

                // Edge intensity calculations
                float edgeIntensity_Albedo = sqrt(
                    gradientX_Albedo * gradientX_Albedo + gradientY_Albedo * gradientY_Albedo);
                float edgeIntensity_Normal = sqrt(
                    gradientX_Normal * gradientX_Normal + gradientY_Normal * gradientY_Normal);

                // Combine edge intensities
                float edgeIntensity = max(edgeIntensity_Albedo, edgeIntensity_Normal);
                float edge = step(_EdgeThreshold, edgeIntensity);

                // Lighting calculations (same as before)
                float3 bakedLight = SampleLightmap(i.staticLightmapUV, modifiedNormalWS);
                float3 ambientLight = SampleSH(modifiedNormalWS);

                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float lightIntensity = max(0.0, dot(modifiedNormalWS, -lightDir));
                float3 directionalLight = lightIntensity * mainLight.color.rgb;

                // Combine lighting
                float3 lighting = bakedLight + ambientLight + directionalLight;

                // Final color
                float4 baseColor = float4(_Albedo.rgb * lighting, _Albedo.a);
                return float4(edge * baseColor.rgb, baseColor.a * _Transparency);
            }
            ENDHLSL
        }
    }
}