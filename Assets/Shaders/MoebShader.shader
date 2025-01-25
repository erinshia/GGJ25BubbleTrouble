Shader "Custom/URP/MoebiusEdgeDetection"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _EdgeThreshold ("Edge Threshold", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Overlay"
        }
        Pass
        {
            Name "EdgeDetectionPass"
            Tags
            {
                "LightMode"="UniversalForward"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Include URP Shader Library
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Textures and Properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _EdgeThreshold;

            struct attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct varyings
            {
                float4 positionHCS : SV_POSITION; // Homogeneous Clip Space
                float2 uv : TEXCOORD0; // UV Coordinates
            };

            varyings vert(attributes v)
            {
                varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            // Sobel Filter Function
            float sobel_filter(float2 uv, float2 texelSize)
            {
                // Sobel Kernels
                float3 sobelX = float3(-1, 0, 1);
                float3 sobelY = float3(1, 2, 1);

                float3 gradientX = float3(0.0, 0.0, 0.0);
                float3 gradientY = float3(0.0, 0.0, 0.0);

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y) * texelSize;
                        float3 sample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset).rgb;

                        gradientX += sample * sobelX[x + 1];
                        gradientY += sample * sobelY[y + 1];
                    }
                }

                // Compute the gradient magnitude
                float intensity = length(gradientX * gradientX + gradientY * gradientY);
                return intensity;
            }

            float4 frag(varyings i) : SV_Target
            {
                // Screen-space texel size (inverse of resolution)
                float2 texelSize = 1.0 / _ScreenParams.xy;

                // Sobel filter gradients
                float gradientX = 0.0;
                float gradientY = 0.0;

                // Loop through a 3x3 neighborhood
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        // Sobel X kernel weights
                        float sobelX = (x == -1 ? -1.0 : (x == 1 ? 1.0 : 0.0)) * (y == 0 ? 2.0 : 1.0);

                        // Sobel Y kernel weights
                        float sobelY = (y == -1 ? -1.0 : (y == 1 ? 1.0 : 0.0)) * (x == 0 ? 2.0 : 1.0);

                        // Texture offset
                        float2 offset = float2(x, y) * texelSize;

                        // Sample texture and convert to grayscale
                        float intensity = dot(
                            SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offset).rgb,
                            float3(0.333, 0.333, 0.333));

                        // Accumulate gradients
                        gradientX += intensity * sobelX;
                        gradientY += intensity * sobelY;
                    }
                }

                // Calculate edge intensity
                float edgeIntensity = sqrt(gradientX * gradientX + gradientY * gradientY);

                // Apply threshold to highlight edges
                float edge = step(_EdgeThreshold, edgeIntensity);

                // Return edge-detected color
                return float4(edge, edge, edge, 1.0);
            }
            ENDHLSL
        }
    }
}