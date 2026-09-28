Shader "TVEC/Mineral"
{
    Properties
    {
        [Header(Main)]
        _BaseColor("Color", Color) = (1,1,1,1)
        _InternalGlow("Internal Glow", Color) = (1,1,1,1)
        _Alpha("Alpha (For Transparent Only)", Range(0.0, 1.0)) = 1.0
        
        [Header(Rendering)]
        [Toggle] _OpaqueMode("Opaque Mode", Float) = 0.0
        [Toggle] _EnableRefraction("Enable Refraction", Float) = 1
        [Toggle] _EnableSSS("Enable SSS", Float) = 0
        [Toggle] _SparklesEnabled("Enable Sparkles", Float) = 0
        [Toggle] _EnableGlow("Enable Glow", Float) = 0
        [Toggle] _EnableRainbow("Enable Rainbow", Float) = 0
        
        [Header(Textures)]
        _NoiseTexture("Noise Texture (Internal)", 2D) = "white" {}
        _NoiseStrength("Noise Strength", Range(0.0, 10.0)) = 1.0
        _NoiseTiling("Noise Tiling", Float) = 8.0
        _NoiseOffset("Noise Offset", Float) = 0.0
        
        [Header(Surface)]
        _RimPower("Rim Power", Range(1.0, 12.0)) = 12.0 
        _RimIntensity("Rim Intensity", Range(0.0, 15.0)) = 15.0
        _Specular("Specular", Range(0.0, 1.0)) = 1.0
        
        [Header(Refraction)]
        _RefStrength("Reflection Strength", Range(0.0, 1.0)) = 1.0
        _ChromaticDispersion("Chromatic Dispersion", Range(0.0, 1.0)) = 1.0
        
        [Header(Iridescence)]
        _IridescenceStrength("Iridescence Strength", Range(0.0, 2.0)) = 2.0
        _IridescenceSpeed("Iridescence Speed", Range(0.0, 2.0)) = 2.0
        
        [Header(Glow)]
        [HDR]_RimGlowColor("Rim Glow Color", Color) = (1,1,1,1)
        _RimGlowIntensity("Rim Glow Intensity", Range(0.0, 10.0)) = 10.0
        _GlowPulseSpeed("Glow Pulse Speed", Range(0.0, 4.0)) = 4.0
        _GlowPulseStrength("Glow Pulse Strength", Range(0.0, 1.0)) = 1.0
        
        [Header(Sparkles)]
        [NoScaleOffset]_SparkleNormal("Sparkle Normal", 2D) = "bump" {}
        _SparkleScale("Sparkle Scale", Range(0.0, 200.00)) = 50.0
        _SparkleStrength("Sparkle Strength", Range(0.0, 1.0)) = 1.0
        _SparkleThreshold("Sparkle Threshold", Range(0.0, 1.0)) =1.0
        _SparkleSize("Sparkle Size", Range(0.0, 30.0)) = 30.0
        _SparkleColor("Sparkle Color", Color) = (1,1,1,1)
        
        [Header(Subsurface Scattering)]
        _SSSStrength("SSS Strength", Range(0.0, 3.0)) = 3.0
        _SSSRadius("SSS Radius", Range(1.0, 32.0)) = 32.0
        _SSSDistortion("SSS Distortion", Range(0.0, 1.0)) = 1.0
        _SSSPower("SSS Power", Range(1.0, 16.0)) = 4.0
        _SSSAmbient("SSS Ambient", Range(0.0, 1.0)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        Pass
        {
            Name "Internal"
            Tags {"LightMode" = "SRPDefaultUnlit"}
            Cull Front
            Blend One OneMinusSrcAlpha
            ZWrite [_OpaqueMode]
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma shader_feature _OPAQUEMODE_ON
            #pragma shader_feature _ENABLEREFRACTION_ON
            #pragma shader_feature _ENABLEGLOW_ON
            #pragma shader_feature _ENABLERAINBOW_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes 
            {
                float4 positionOS: POSITION;
                float3 normalOS: NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings 
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 noiseUV : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor, _InternalGlow, _RimGlowColor;
                float _RefStrength, _RimPower, _ChromaticDispersion, _Alpha;
                float _NoiseStrength, _NoiseTiling, _NoiseOffset;
                float _RimGlowIntensity, _GlowPulseSpeed, _GlowPulseStrength; 
            CBUFFER_END
            
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            
            TEXTURE2D(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);
            
            Varyings vert (Attributes input)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, o);
                
                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs norm = GetVertexNormalInputs(input.normalOS);
                
                o.positionCS = pos.positionCS;
                o.normalWS = norm.normalWS;
                o.viewDirWS = GetWorldSpaceNormalizeViewDir(pos.positionWS);
                o.screenPos = ComputeScreenPos(o.positionCS);
                
                o.noiseUV = input.positionOS.xyz * _NoiseTiling + _NoiseOffset;
                
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 n = normalize(i.normalWS);
                float3 v = normalize(i.viewDirWS);
                float fresnel = pow(1.0 - saturate(dot(-n, v)), _RimPower); 
                
                Light mainLight = GetMainLight();
                #if _ENABLERAINBOW_ON
                    float iridescence = sin(dot(n, v) * 10.0) * 0.5 + 0.5;
                    half3 rainbow = saturate(0.5 + 0.5 * cos(float3(0.0, 2.2, 4.2) + iridescence * 7.0));
                #else
                    half3 rainbow = mainLight.color;
                #endif
                
                float3 noiseUV = i.noiseUV;
                half noiseX = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, noiseUV.yz).r;
                half noiseY = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, noiseUV.xz).r;
                half noiseZ = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, noiseUV.xy).r;
                
                float3 blend = abs(i.normalWS);
                blend = pow(blend, 2.0);
                blend /= (blend.x + blend.y + blend.z);
                
                half noise = noiseX * blend.x + noiseY * blend.y + noiseZ * blend.z;
                
                
                half3 baseColor = _BaseColor.rgb;
                float pulse = 1.0 + _GlowPulseStrength * sin(_Time.y * _GlowPulseSpeed);
                #if _ENABLEGLOW_ON
                    baseColor = lerp(_BaseColor.rgb, _InternalGlow.rgb * pulse, 0.65);
                #else
                    baseColor = lerp(_BaseColor.rgb, _InternalGlow.rgb, 0.65);
                #endif
                
                #if _OPAQUEMODE_ON
                    float3 reflectDir = reflect(-v, -n);
                    half4 reflProbe = SAMPLE_TEXTURECUBE(unity_SpecCube0, samplerunity_SpecCube0, reflectDir);
                    half3 envReflection = DecodeHDREnvironment(reflProbe, unity_SpecCube0_HDR);
                
                    half3 color = baseColor * (1.0 + noise * _NoiseStrength);
                    color = lerp(color, rainbow, fresnel * 0.3);
                    color += envReflection * 0.3 * fresnel;
                    color += fresnel * _RimGlowColor.rgb * _RimGlowIntensity * pulse;
                
                    return half4(color, 1.0);
                #else
                    half3 refraction = half3(1,1,1);
                       #if _ENABLEREFRACTION_ON
                        float2 uv = i.screenPos.xy / i.screenPos.w;
                        float3 refractDir = refract(-v, n, 0.75);
                        float2 offset = refractDir.xy * _RefStrength * 1.5;
                
                        half r = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture,uv + offset * (1 + _ChromaticDispersion)).r;
                        half g = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture,uv + offset).g;
                        half b = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture,uv + offset * (1 - _ChromaticDispersion)).b;
                
                        refraction = half3(r,g,b);
                        #endif
                
                    half3 color = baseColor * refraction * (1.0 + noise * _NoiseStrength);
                    #if _ENABLEGLOW_ON
                        color += fresnel * _RimGlowColor.rgb * _RimGlowIntensity * pulse;
                    #endif
                
                    half alpha = _Alpha * 0.92;
                    return half4(color * alpha, alpha);
                #endif
            }
            ENDHLSL
        }
        Pass
        {
            Name "CrystalFront"
            Tags {"LightMode" = "UniversalForward"}
            
            Cull Back
            Blend One OneMinusSrcAlpha
            ZWrite [_OpaqueMode] 
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma shader_feature _OPAQUEMODE_ON
            #pragma shader_feature _ENABLEREFRACTION_ON
            #pragma shader_feature _SPARKLESENABLED_ON
            #pragma shader_feature _ENABLEGLOW_ON
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma shader_feature _ENABLESSS_ON
            #pragma shader_feature _ENABLERAINBOW_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 noiseUV : TEXCOORD4;
                #if _SPARKLESENABLED_ON
                    float2 sparkleUV : TEXCOORD5;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor, _SparkleColor, _RimGlowColor, _InternalGlow;
                float _RimPower, _RimIntensity, _Specular;
                float _RefStrength, _ChromaticDispersion;
                float _IridescenceStrength, _IridescenceSpeed;
                float _SparkleScale, _SparkleStrength, _SparkleThreshold, _SparkleSize;
                float _Alpha, _OpaqueMode;
                float _NoiseStrength, _NoiseTiling, _NoiseOffset;
                float _RimGlowIntensity, _GlowPulseSpeed, _GlowPulseStrength;
                float _SSSStrength, _SSSPower, _SSSDistortion, _SSSRadius, _SSSAmbient;
            CBUFFER_END
            
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            
            TEXTURE2D(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);
            
            #if _SPARKLESENABLED_ON
                TEXTURE2D(_SparkleNormal);
                SAMPLER(sampler_SparkleNormal);
            #endif
            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                
                return frac(p.x * p.y * p.z *(p.x + p.y + p.z));
            }
            
            // helper function - specular + rim for any light source
            half3 LightContribution(float3 n, float3 v, float3 l, half3 lightColor, float fresnel, half3 rainbow, float specPow)
            {  
                float3 h = normalize(l+v);
                float spec = pow(max(0.0, dot(n,h)), 140.0 * specPow + 1.0);
                half3 rim = pow(1.0 - saturate(dot(n,l)), 3.0) * fresnel;
                return (spec * specPow * 1.5 + rim * 0.5) * lightColor * rainbow;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, o);
                
                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs norm = GetVertexNormalInputs(input.normalOS);
                
                o.positionCS = pos.positionCS;
                o.positionWS = pos.positionWS;
                o.normalWS = norm.normalWS;
                o.viewDirWS = GetWorldSpaceNormalizeViewDir(pos.positionWS);
                o.screenPos = ComputeScreenPos(o.positionCS);
                o.noiseUV = input.positionOS.xyz * _NoiseTiling + _NoiseOffset;
                
                #if _SPARKLESENABLED_ON
                    o.sparkleUV = input.positionOS.xz * _SparkleScale;
                #endif
                
                return o;
            }
            
            half4 frag(Varyings i) : SV_Target
            {
                float3 n= normalize(i.normalWS);
                float3 v= normalize(i.viewDirWS);
                float fresnel = pow(1.0 - saturate(dot(n, v)), _RimPower);
                
                //Rim + Specular
                Light mainLight = GetMainLight();
                float3 l = normalize(mainLight.direction);
                float3 h = normalize(l + v);
                
                float specular = pow(max(0, dot(n, h)), 140.0 * _Specular + 1.0); 
                //Iridescence
                #if _ENABLERAINBOW_ON
                    float iridescence = sin(dot(n, v) * 10.0 - _Time.y * _IridescenceSpeed) * 0.5 + 0.5;
                    half3 rainbow = saturate(0.5 + 0.5 * cos(float3(0.0, 2.2, 4.2) + iridescence * 7.0));
                #else
                    half3 rainbow = mainLight.color;
                #endif
                
                float3 noiseUV = i.noiseUV;
                half noiseX = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, noiseUV.yz).r;
                half noiseY = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, noiseUV.xz).r;
                half noiseZ = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, noiseUV.xy).r;
                
                float3 blend = pow(abs(i.normalWS), 2.0);
                blend /= (blend.x + blend.y + blend.z + 0.001);
                half noise = noiseX * blend.x + noiseY * blend.y + noiseZ * blend.z;
                
                #if _ENABLEGLOW_ON
                    float pulse = 1.0 + _GlowPulseStrength * sin(_Time.y * _GlowPulseSpeed);
                #endif
                
                //Sparkle
                half sparkle = 0;
                #if _SPARKLESENABLED_ON
                    half4 sparkleSample = SAMPLE_TEXTURE2D(_SparkleNormal, sampler_SparkleNormal, i.sparkleUV);
                    half3 sparkleNormalTS = UnpackNormal(sparkleSample);
                    
                    half sparkleDot = saturate(dot(sparkleNormalTS, float3 (0,0,1)));
                    sparkle = pow(sparkleDot, _SparkleSize) * _SparkleStrength;
                    
                    sparkle *= step(_SparkleThreshold, hash(float3(i.sparkleUV, 0) * 12.3));
                #endif
                
                float thickness = saturate(1.0 - fresnel * 0.35);
                half3 sss = 0.0;
                #if _ENABLESSS_ON
                    float3 sssLightDir = normalize(mainLight.direction + n * _SSSDistortion);
                    float backScatter = pow(max(0.0, dot(-sssLightDir, v)), _SSSPower);
                    half3 bakedGI = SampleSHVertex(n);
                    sss += _SSSAmbient * thickness * bakedGI * _BaseColor.rgb * 0.5;
                    half3 sssColor = mainLight.color * _BaseColor.rgb;
                
                    #if _OPAQUEMODE_ON
                        sss += (backScatter * _SSSStrength + _SSSAmbient) * thickness * sssColor * 3.0;
                        
                        #if _ENABLEGLOW_ON
                            sss += _InternalGlow.rgb *fresnel * pulse * 1.2;
                        #endif
                    #else
                        float2 screenUV = i.screenPos.xy / i.screenPos.w;
                        half3 refractionBlur = 0.0;
                        const int samples = 5;
                        float radius = _SSSRadius / _ScreenParams.x;
                        for (int s = -samples; s <=samples; ++s)
                        {
                            float2 offset = float2(s * radius * (1.0 + thickness), 0.0);
                            refractionBlur += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offset).rgb;
                            refractionBlur += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV - offset).rgb;
                            refractionBlur += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offset.yx).rgb;
                            refractionBlur += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV - offset.yx).rgb;
                        }
                        refractionBlur /= float((samples * 2 + 1) * 2);
                        sss = lerp(sssColor * backScatter * _SSSStrength * thickness * 2.0, refractionBlur * _BaseColor.rgb * _SSSStrength * 1.8, 0.65);
                        sss += backScatter * _SSSStrength * thickness * 0.8;
                    #endif
                #endif
                
                #if _OPAQUEMODE_ON
                    half3 color = _BaseColor.rgb * (1.0 + noise * _NoiseStrength * 0.6);
                    color = lerp(color, rainbow, fresnel * _IridescenceStrength);
                    color += fresnel * _RimIntensity * rainbow;
                    color += specular * _Specular * 1.5 * _BaseColor.rgb;
                    color += sparkle * _SparkleColor.rgb * rainbow * 5.0;
                
                    color += sss;
                
                    #if _ENABLEGLOW_ON
                        color += fresnel * _RimGlowColor.rgb * _RimGlowIntensity * pulse;
                    #endif
                #else
                    half3 refraction = half3(1,1,1);
                    #if _ENABLEREFRACTION_ON
                        float2 uv = i.screenPos.xy / i.screenPos.w;
                        float2 offset = refract(-v, n, 0.85).xy * _RefStrength;
                        refraction = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + offset).rgb;
                    #endif
                    
                    half3 color = _BaseColor.rgb * refraction * (1.0 + noise * _NoiseStrength * 0.6);
                    color = lerp(color, rainbow, fresnel * _IridescenceStrength);
                    color += fresnel * _RimIntensity * rainbow;
                    color += specular * _Specular * 1.5 * _BaseColor.rgb;
                    color += sparkle * _SparkleColor.rgb * rainbow * 5.0;
                    color += sss;
                    
                    #if _ENABLEGLOW_ON
                        color += fresnel * _RimGlowColor.rgb * _RimGlowIntensity * pulse;
                    #endif
                #endif
                
                #if _ADDITIONAL_LIGHTS && _ENABLESSS_ON
                        InputData inputData = (InputData)0;
                        inputData.positionWS = i.positionWS;
                        inputData.normalWS = n;
                        inputData.viewDirectionWS = v;
                        inputData.shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                        inputData.positionCS = i.positionCS;
                        inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
                        uint lightCount = GetAdditionalLightsCount();
                        LIGHT_LOOP_BEGIN(lightCount)
                            Light al = GetAdditionalLight(lightIndex, i.positionWS);
                            float3 al_l = normalize(al.direction);
                            half3 al_c = al.color * al.distanceAttenuation * al.shadowAttenuation;
                            color += LightContribution(n, v, al_l, al_c, fresnel, rainbow, _Specular);
                        LIGHT_LOOP_END
                
                        #if _ENABLESSS_ON
                            LIGHT_LOOP_BEGIN(lightCount)
                            Light al_sss = GetAdditionalLight(lightIndex, i.positionWS);
                            float3 al_l_sss= normalize(al_sss.direction);
                            half3 al_c_sss = al_sss.color * al_sss.distanceAttenuation * al_sss.shadowAttenuation;
                            float3 al_sssDir = normalize(al_l_sss + n * _SSSDistortion);
                            float al_scatter = pow(max(0.0, dot(-al_sssDir, v)), _SSSPower);
                                color += (al_scatter * _SSSStrength + _SSSAmbient) * thickness * _BaseColor.rgb * al_c_sss;
                        LIGHT_LOOP_END
                        #endif
                    #endif
                
                    #if _OPAQUEMODE_ON
                        return half4(color, 1.0);
                    #else
                        half alpha = saturate(0.55 + fresnel * 0.75) * _Alpha;
                        half4 result = half4(color * alpha, alpha);
                        result.rgb = MixFog(result.rgb, ComputeFogFactor(i.positionCS.z));
                        return result;
                #endif
            }
            ENDHLSL
        }
    }
    CustomEditor "CrystalMaterialEditor"
    FallBack "Universal Render pipeline/Transparent"
}
