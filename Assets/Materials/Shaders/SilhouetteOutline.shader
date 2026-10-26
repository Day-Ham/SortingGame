Shader "TVEC/SilhouetteOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width (pixels)", Range(1, 20)) = 6
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Name "SilhouetteOutline"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Back
            ZWrite Off
            ZTest Always

            HLSLPROGRAM
            #pragma target 4.0
            #pragma require geometry
            #pragma vertex Vert
            #pragma geometry Geom
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings Vert(float4 positionOS : POSITION)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(positionOS.xyz);
                return output;
            }

            // Copies of the mesh, shifted by the same pixel amount in every direction.
            // The real mesh is drawn on top, so only the even rim is left.
            #define DIRECTION_COUNT 16

            [maxvertexcount(48)]
            void Geom(triangle Varyings input[3], inout TriangleStream<Varyings> stream)
            {
                [unroll]
                for (int i = 0; i < DIRECTION_COUNT; i++)
                {
                    float angle = i * (TWO_PI / DIRECTION_COUNT);
                    float2 direction = float2(cos(angle), sin(angle));
                    float2 pixelOffset = direction * _OutlineWidth * 2.0 / _ScreenParams.xy;

                    [unroll]
                    for (int v = 0; v < 3; v++)
                    {
                        Varyings output = input[v];
                        output.positionHCS.xy += pixelOffset * output.positionHCS.w;
                        stream.Append(output);
                    }

                    stream.RestartStrip();
                }
            }

            half4 Frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
