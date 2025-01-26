Shader "Custom/URP/MoebiusEdgeDetection"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _EdgeThreshold ("Edge Threshold", Range(0, 1)) = 0.1
        _WiggleStrength ("Wiggle Strength", Range(0.0, 1.0)) = 0.1
        _NoiseScale ("Noise Scale", Range(0.1, 10.0)) = 1.0
        _TransparencyBase ("Base Transparency", Range(0.0, 1.0)) = 1.0
        _TransparencyOutline ("Outline Transparency", Range(0.0, 1.0)) = 1.0
        _Albedo ("Albedo Color", Color) = (1, 1, 1, 1)
        _UseMainTex ("Use Main Texture", Float) = 1.0
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

            float _EdgeThreshold;
            float _WiggleStrength;
            float _NoiseScale;
            float _TransparencyBase;
            float _TransparencyOutline;
            float4 _Albedo;
            float _UseMainTex;

            struct attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            varyings vert(attributes v)
            {
                varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            // Simple sine-based noise for UV distortion
            float2 GenerateNoiseUV(float2 uv)
            {
                float noiseX = sin(uv.y * _NoiseScale) * cos(uv.x * _NoiseScale);
                float noiseY = cos(uv.x * _NoiseScale) * sin(uv.y * _NoiseScale);
                return float2(noiseX, noiseY) * _WiggleStrength;
            }

            // Encode texture data into the "normal" buffer
            float3 EncodeTextureToNormal(float2 uv)
            {
                float3 textureSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
                return normalize(float3(textureSample.rg, 0.5)); // Encoding into RG channels
            }

            float4 frag(varyings i) : SV_Target
            {
                // Screen-space texel size
                float2 texelSize = 1.0 / _ScreenParams.xy;

                // Apply noise to UVs for wiggly lines
                float2 wigglyUV = i.uv + GenerateNoiseUV(i.uv);

                // Sobel filter gradients for encoded texture data
                float gradientX = 0.0, gradientY = 0.0;

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float sobelX = (x == -1 ? -1.0 : (x == 1 ? 1.0 : 0.0)) * (y == 0 ? 2.0 : 1.0);
                        float sobelY = (y == -1 ? -1.0 : (y == 1 ? 1.0 : 0.0)) * (x == 0 ? 2.0 : 1.0);
                        float2 offset = texelSize * float2(x, y);

                        // Encode texture information into the "normal buffer"
                        float3 encodedNormal = EncodeTextureToNormal(wigglyUV + offset);

                        // Accumulate gradients
                        gradientX += encodedNormal.r * sobelX;
                        gradientY += encodedNormal.g * sobelY;
                    }
                }

                // Edge intensity calculations
                float edgeIntensity = sqrt(gradientX * gradientX + gradientY * gradientY);
                float edge = step(_EdgeThreshold, edgeIntensity);

                // Sample the main texture or use the albedo color
                float3 baseColor = (_UseMainTex > 0.5)
                                       ? SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb
                                       : _Albedo.rgb;

                // Combine edge detection and base texture/albedo
                float outlineAlpha = edge * _TransparencyOutline; // Outline transparency
                float baseAlpha = (1.0 - edge) * _TransparencyBase; // Base transparency
                float finalAlpha = outlineAlpha + baseAlpha; // Combine alpha values

                float3 finalColor = edge * float3(1.0, 1.0, 1.0) * _TransparencyOutline // Outline as white
                    + (1.0 - edge) * baseColor; // Base render

                // Return the final color with combined transparency
                return float4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}