Shader "Game/RTS/Faction Color"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset] _FactionMask("Faction Mask (Red)", 2D) = "black" {}
        _FactionColor("Faction Color", Color) = (1, 0, 0, 1)
        [KeywordEnum(Replace, Multiply, Add, Overlay)] _FactionBlend("Faction Blend", Float) = 0
        [Range(0, 1)] _FactionStrength("Faction Strength", Float) = 1
        [Range(0, 1)] _MaskStrength("Mask Strength", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma shader_feature_local _FACTIONBLEND_REPLACE _FACTIONBLEND_MULTIPLY _FACTIONBLEND_ADD _FACTIONBLEND_OVERLAY
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_FactionMask);
            SAMPLER(sampler_FactionMask);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _FactionColor;
                half _FactionStrength;
                half _MaskStrength;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                output.positionCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = normalInputs.normalWS;
                return output;
            }

            half3 GetBlendedFactionColor(half3 baseColor)
            {
                #if defined(_FACTIONBLEND_MULTIPLY)
                    return baseColor * _FactionColor.rgb;
                #elif defined(_FACTIONBLEND_ADD)
                    return saturate(baseColor + _FactionColor.rgb);
                #elif defined(_FACTIONBLEND_OVERLAY)
                    half3 lower = 2.0h * baseColor * _FactionColor.rgb;
                    half3 upper = 1.0h - 2.0h * (1.0h - baseColor) * (1.0h - _FactionColor.rgb);
                    return lerp(lower, upper, step(0.5h, baseColor));
                #else
                    return _FactionColor.rgb;
                #endif
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 baseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                half mask = SAMPLE_TEXTURE2D(_FactionMask, sampler_FactionMask, input.uv).r;
                half factionWeight = saturate(mask * _MaskStrength * _FactionStrength);
                half3 albedo = lerp(baseSample.rgb, GetBlendedFactionColor(baseSample.rgb), factionWeight);

                half3 normalWS = normalize(input.normalWS);
                Light mainLight = GetMainLight();
                half diffuse = saturate(dot(normalWS, mainLight.direction));
                half3 lighting = SampleSH(normalWS) + mainLight.color * diffuse * mainLight.distanceAttenuation;
                return half4(albedo * lighting, baseSample.a);
            }
            ENDHLSL
        }
    }
}
