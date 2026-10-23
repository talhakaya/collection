// URP's lit look for the blocky characters, with a drawn pattern on each part: where the
// pattern is white the part shows its colour, where it is black its second colour, and
// greys mix the two.
//
// The pattern is laid on from the three sides of the part (whichever its surface faces), in
// the part's own space but measured in metres, so it's the same size on a torso and a finger,
// isn't stretched along long thin limbs, and moves with the body.
//
// CharacterAppearance sets the colours and pattern on each part; the material holds the
// defaults (no pattern: plain colour).
Shader "Collection/Story/Blocky Pattern"
{
	Properties
	{
		_BaseColor ("Colour (the pattern's white)", Color) = (1, 1, 1, 1)
		_ShadeColor ("Second colour (the pattern's black)", Color) = (0.5, 0.5, 0.5, 1)
		_PatternMap ("Pattern", 2D) = "white" {}
		_PatternSize ("Pattern size (metres a repeat)", Float) = 0.5
		_PatternStrength ("Pattern strength", Range(0, 1)) = 1
		_Smoothness ("Smoothness", Range(0, 1)) = 0.15
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _BaseColor;
			float4 _ShadeColor;
			float _PatternSize;
			float _PatternStrength;
			float _Smoothness;
		CBUFFER_END
		ENDHLSL

		Pass
		{
			Name "Forward"
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile _ _FORWARD_PLUS
			#pragma multi_compile_fragment _ _LIGHT_LAYERS
			#pragma multi_compile_fog
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_PatternMap);
			SAMPLER(sampler_PatternMap);

			struct Attributes
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float3 normalOS : TEXCOORD2;
				float3 patternPosition : TEXCOORD3;
				float fogFactor : TEXCOORD4;
			};

			Varyings Vertex(Attributes input)
			{
				// The part's size in the world along each of its own axes, so the pattern is
				// measured in metres however the part is scaled.
				float3 size = float3(length(unity_ObjectToWorld._m00_m10_m20),
					length(unity_ObjectToWorld._m01_m11_m21),
					length(unity_ObjectToWorld._m02_m12_m22));

				Varyings output;
				output.positionWS = TransformObjectToWorld(input.positionOS);
				output.positionCS = TransformWorldToHClip(output.positionWS);
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.normalOS = input.normalOS;
				output.patternPosition = input.positionOS * size;
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				return output;
			}

			// The pattern seen from the side the surface faces, blended where it faces between
			// sides (on a cube's flat faces only one side counts).
			half Pattern(float3 position, float3 normal)
			{
				float3 p = position / max(_PatternSize, 0.001);
				float3 w = pow(abs(normal), 4);
				w /= w.x + w.y + w.z;
				half x = SAMPLE_TEXTURE2D(_PatternMap, sampler_PatternMap, p.zy).r;
				half y = SAMPLE_TEXTURE2D(_PatternMap, sampler_PatternMap, p.xz).r;
				half z = SAMPLE_TEXTURE2D(_PatternMap, sampler_PatternMap, p.xy).r;
				return x * w.x + y * w.y + z * w.z;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				half white = lerp(1, Pattern(input.patternPosition, normalize(input.normalOS)), _PatternStrength);
				half3 albedo = lerp(_ShadeColor.rgb, _BaseColor.rgb, white);

				InputData inputData = (InputData)0;
				inputData.positionWS = input.positionWS;
				inputData.positionCS = input.positionCS;
				inputData.normalWS = normalize(input.normalWS);
				inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
				inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				inputData.fogCoord = input.fogFactor;
				inputData.bakedGI = SampleSH(inputData.normalWS);
				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
				inputData.shadowMask = half4(1, 1, 1, 1);

				SurfaceData surface = (SurfaceData)0;
				surface.albedo = albedo;
				surface.smoothness = _Smoothness;
				surface.occlusion = 1;
				surface.alpha = 1;
				surface.normalTS = half3(0, 0, 1);

				half4 colour = UniversalFragmentPBR(inputData, surface);
				colour.rgb = MixFog(colour.rgb, inputData.fogCoord);
				return half4(colour.rgb, 1);
			}
			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			ZWrite On
			ZTest LEqual
			ColorMask 0

			HLSLPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment
			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			float3 _LightDirection;
			float3 _LightPosition;

			float4 Vertex(float3 positionOS : POSITION, float3 normalOS : NORMAL) : SV_POSITION
			{
				float3 positionWS = TransformObjectToWorld(positionOS);
				float3 normalWS = TransformObjectToWorldNormal(normalOS);
				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 toLight = normalize(_LightPosition - positionWS);
				#else
					float3 toLight = _LightDirection;
				#endif
				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, toLight));

				// Nothing of the caster may fall in front of the light's near plane.
				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#endif
				return positionCS;
			}

			half4 Fragment() : SV_Target
			{
				return 0;
			}
			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags { "LightMode" = "DepthOnly" }
			ColorMask R

			HLSLPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment

			float4 Vertex(float3 positionOS : POSITION) : SV_POSITION
			{
				return TransformObjectToHClip(positionOS);
			}

			half4 Fragment() : SV_Target
			{
				return 0;
			}
			ENDHLSL
		}
	}
}
