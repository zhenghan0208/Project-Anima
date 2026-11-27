Shader "Custom/URP_Stylized_HandDrawn"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (0.96, 0.92, 0.82, 1)
        [MainTexture] _BaseMap("Paper Texture (Albedo)", 2D) = "white" {}
        _RampTex("Ramp (Light/Shadow gradient)", 2D) = "white" {}
        _HatchTex("Hatching Lines Texture", 2D) = "white" {}
        _HatchTiling("Hatch Tiling XY", Vector) = (6.68, 0.48, 0, 0)
        _HatchOpacity("Hatching Opacity", Range(0, 1)) = 0.8
        _ShadowThreshold("Shadow Threshold", Range(0.01, 1)) = 0.65
        _ShadowStrength("Shadow Strength", Range(0, 1)) = 0.9
        _CustomLightBlend("Custom Light Blend", Range(0, 1)) = 1
        _CustomLightDirection("Custom Light Direction (XYZ)", Vector) = (35.3, 35.37, -3.9, 0)
        _OutlineWidth("Outline Width (world units)", Range(0, 0.1)) = 0.0193
        _OutlineColor("Outline Color", Color) = (0.11, 0.075, 0.14, 1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "HandDrawnForward"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_RampTex); SAMPLER(sampler_RampTex);
            TEXTURE2D(_HatchTex); SAMPLER(sampler_HatchTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                float4 _HatchTiling;
                half _HatchOpacity;
                half _ShadowThreshold;
                half _ShadowStrength;
                half _CustomLightBlend;
                float4 _CustomLightDirection;
                float _OutlineWidth;
                half4 _OutlineColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float2 hatchUV : TEXCOORD3;
            };
            Varyings vert(Attributes v)
            {
                Varyings o;
                VertexPositionInputs pos = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = pos.positionCS;
                o.positionWS = pos.positionWS;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.hatchUV = v.uv * _HatchTiling.xy;
                return o;
            }
            half4 frag(Varyings i) : SV_Target
            {
                half3 n = normalize(i.normalWS);
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                half3 realLightDir = normalize(mainLight.direction);
                half3 customDir = normalize(_CustomLightDirection.xyz + float3(0.0001, 0.0001, 0.0001));
                half3 lightDir = normalize(lerp(realLightDir, customDir, _CustomLightBlend));
                half lightAmount = saturate(dot(n, lightDir) * 0.5 + 0.5);
                // The built-in shadow map is applied when following Unity's light.
                half attenuation = lerp(mainLight.shadowAttenuation, 1.0h, _CustomLightBlend);
                lightAmount *= attenuation;
                // A supplied ramp maps darkness to brightness horizontally.
                half3 ramp = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(lightAmount, 0.5)).rgb;
                half3 paper = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).rgb;
                half3 color = paper * _BaseColor.rgb;
                half shadowMask = 1.0h - smoothstep(_ShadowThreshold - 0.12h, _ShadowThreshold + 0.12h, lightAmount);
                half3 shaded = lerp(color, color * ramp, _ShadowStrength * shadowMask);
                // Hatch texture: white paper and dark pencil lines.
                half3 hatchTex = SAMPLE_TEXTURE2D(_HatchTex, sampler_HatchTex, i.hatchUV).rgb;
                half hatchInk = 1.0h - dot(hatchTex, half3(0.299, 0.587, 0.114));
                half ink = hatchInk * shadowMask * _HatchOpacity;
                shaded = lerp(shaded, shaded * half3(0.32, 0.29, 0.34), saturate(ink));
                return half4(shaded, 1);
            }
            ENDHLSL
        }
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }
            Cull Front
            ZWrite On
            HLSLPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                float4 _HatchTiling;
                half _HatchOpacity;
                half _ShadowThreshold;
                half _ShadowStrength;
                half _CustomLightBlend;
                float4 _CustomLightDirection;
                float _OutlineWidth;
                half4 _OutlineColor;
            CBUFFER_END
            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; };
            Varyings vertOutline(Attributes v)
            {
                Varyings o;
                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);
                float3 worldN = normalize(TransformObjectToWorldNormal(v.normalOS));
                o.positionCS = TransformWorldToHClip(worldPos + worldN * _OutlineWidth);
                return o;
            }
            half4 fragOutline(Varyings i) : SV_Target { return _OutlineColor; }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
