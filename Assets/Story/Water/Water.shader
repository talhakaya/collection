// The story's water, the grassy field's ponds and the desert's sea: clear where it is shallow and coloured where it is deep, so the
// shore and anything standing in the water go down into it and fade out, rather than stopping at a hard line.
//
// It does what the Simple Water Shader (IgniteCoders) does for the desert's sea - small waves moving the surface,
// two ripple maps going different ways, and the picture of the world above it taken by WaterMirror - and to that
// adds how deep the water is under each point of the surface: the scene's depth (the camera's depth texture) at
// that point of the screen, turned back into a place in the world, and how far that place is under the surface.
// That depth decides:
//
//   - how see-through the water is: `_ShallowAlpha` near the shore, `_DeepColor`'s alpha where it is deep, and
//     nothing at all right at the edge (over `_EdgeFade`), so the land meets the water without a line;
//   - its colour, from `_ShallowColor` to `_DeepColor` over `_DepthFade`;
//   - a faint pale band at the water's edge (`_FoamColor`, `_FoamWidth`), broken up by the ripples.
//
// The ripple maps are the asset's, which are imported as colour pictures: they are turned back into the values
// drawn in them before being read as normals. The reflection is read with its left and right swapped back
// (WaterMirror's picture is flipped that way).
Shader "Collection/Story/Water"
{
	Properties
	{
		_DeepColor ("Deep colour (alpha: how see-through deep water is)", Color) = (0.05, 0.3, 0.42, 0.95)
		_ShallowColor ("Shallow colour", Color) = (0.16, 0.66, 0.64, 1)
		_ShallowAlpha ("How see-through shallow water is", Range(0, 1)) = 0.35
		_DepthFade ("Depth to the deep colour (m)", Float) = 2.5
		_EdgeFade ("Clear at the very edge, over (m)", Float) = 0.25
		_FoamColor ("Edge colour (alpha: strength)", Color) = (0.9, 0.95, 0.9, 0.35)
		_FoamWidth ("Edge band width (m)", Float) = 0.18

		_MainNormal ("Ripples", 2D) = "grey" {}
		_SecondNormal ("Second ripples", 2D) = "grey" {}
		_NormalTiling ("Ripple tiling (repeats per texture unit)", Vector) = (7, 7, 0, 0)
		_NormalStrength ("Ripple strength", Range(0, 1)) = 0.18
		_RippleSpeed ("Ripple speed", Float) = 0.02

		Texture2D_28de85506601443d82b6148f21ccc69c ("Reflection (set by WaterMirror)", 2D) = "black" {}
		_ReflectionPower ("Reflection strength", Range(0, 1)) = 0.6
		_Distortion ("How much the ripples bend the reflection", Range(0, 1)) = 0.18
		_Smoothness ("Smoothness", Range(0, 1)) = 0.8

		_WaveHeight ("Wave height (m)", Float) = 0.06
		_WaveLength ("Wave length (m)", Float) = 3
		_WaveSpeed ("Wave speed", Float) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

		Pass
		{
			Name "Forward"
			Tags { "LightMode" = "UniversalForward" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Back

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
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _DeepColor;
				float4 _ShallowColor;
				float _ShallowAlpha;
				float _DepthFade;
				float _EdgeFade;
				float4 _FoamColor;
				float _FoamWidth;
				float4 _MainNormal_ST;
				float4 _SecondNormal_ST;
				float4 _NormalTiling;
				float _NormalStrength;
				float _RippleSpeed;
				float _ReflectionPower;
				float _Distortion;
				float _Smoothness;
				float _WaveHeight;
				float _WaveLength;
				float _WaveSpeed;
			CBUFFER_END

			TEXTURE2D(_MainNormal);
			SAMPLER(sampler_MainNormal);
			TEXTURE2D(_SecondNormal);
			SAMPLER(sampler_SecondNormal);
			TEXTURE2D(Texture2D_28de85506601443d82b6148f21ccc69c);
			SAMPLER(samplerTexture2D_28de85506601443d82b6148f21ccc69c);

			struct Attributes
			{
				float3 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float fogFactor : TEXCOORD2;
			};

			// The surface's rise and fall: three long, slow waves going different ways, in metres.
			float Wave(float2 xz)
			{
				float2 p = xz / max(_WaveLength, 0.01);
				float t = _Time.y * _WaveSpeed;
				return (sin(p.x * 1.3 + t) + sin(p.y * 1.7 - t * 0.8) + sin((p.x + p.y) * 0.9 + t * 1.3)) / 3.0;
			}

			Varyings Vertex(Attributes input)
			{
				Varyings output;
				float3 positionWS = TransformObjectToWorld(input.positionOS);
				positionWS.y += Wave(positionWS.xz) * _WaveHeight;
				output.positionWS = positionWS;
				output.positionCS = TransformWorldToHClip(positionWS);
				output.uv = input.uv;
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				return output;
			}

			// A ripple map's normal, from the picture's red and green as drawn (it is imported as a colour picture,
			// so the sampled value is first turned back from linear).
			half2 Ripple(TEXTURE2D_PARAM(map, mapSampler), float2 uv)
			{
				half3 drawn = LinearToSRGB(SAMPLE_TEXTURE2D(map, mapSampler, uv).rgb);
				return drawn.rg * 2 - 1;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				// The ripples: the two maps moving different ways, added, as a world-space normal (the surface faces
				// up; the maps' x and y go along the world's x and z).
				float2 uv = input.uv * _NormalTiling.xy;
				float t = _Time.y * _RippleSpeed;
				half2 ripple = Ripple(TEXTURE2D_ARGS(_MainNormal, sampler_MainNormal), uv + float2(t, t * 0.6))
					+ Ripple(TEXTURE2D_ARGS(_SecondNormal, sampler_SecondNormal), uv * 0.77 - float2(t * 0.7, t));
				ripple *= _NormalStrength;
				float3 normalWS = normalize(float3(ripple.x, 1, ripple.y));

				// How deep the water is here: the ground seen through this point, back in the world, and how far
				// under the surface it is.
				float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);
				float rawDepth = SampleSceneDepth(screenUV);
				float3 groundWS = ComputeWorldSpacePosition(screenUV, rawDepth, UNITY_MATRIX_I_VP);
				float depth = max(0, input.positionWS.y - groundWS.y);

				float deep = saturate(depth / max(_DepthFade, 0.01));
				half3 colour = lerp(_ShallowColor.rgb, _DeepColor.rgb, deep);
				half alpha = lerp(_ShallowAlpha, _DeepColor.a, deep) * saturate(depth / max(_EdgeFade, 0.001));

				// Lit as a smooth surface, the sky and lights reflecting in it.
				InputData inputData = (InputData)0;
				inputData.positionWS = input.positionWS;
				inputData.positionCS = input.positionCS;
				inputData.normalWS = normalWS;
				inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
				inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				inputData.fogCoord = input.fogFactor;
				inputData.bakedGI = SampleSH(normalWS);
				inputData.normalizedScreenSpaceUV = screenUV;
				inputData.shadowMask = half4(1, 1, 1, 1);

				SurfaceData surface = (SurfaceData)0;
				surface.albedo = colour;
				surface.smoothness = _Smoothness;
				surface.occlusion = 1;
				surface.alpha = alpha;
				surface.normalTS = half3(0, 0, 1);
				half4 lit = UniversalFragmentPBR(inputData, surface);

				// The world above, mirrored in it, bent by the ripples; more of it the flatter it is seen.
				float2 mirrorUV = float2(1 - screenUV.x, screenUV.y) + ripple * _Distortion * 0.1;
				half3 mirrored = SAMPLE_TEXTURE2D(Texture2D_28de85506601443d82b6148f21ccc69c, samplerTexture2D_28de85506601443d82b6148f21ccc69c, mirrorUV).rgb;
				half facing = saturate(dot(normalWS, inputData.viewDirectionWS));
				half fresnel = pow(1 - facing, 5);
				half reflected = _ReflectionPower * lerp(0.5, 1, fresnel) * deep;
				lit.rgb = lerp(lit.rgb, mirrored, reflected);
				// Where it reflects, it is not seen through.
				alpha = lerp(alpha, 1, reflected * 0.5);

				// The pale band where the water meets the land, broken up by the ripples.
				// Lit like the land it lies on (the sun and the sky's light), so it does not glow at night.
				half edge = 1 - saturate(depth / max(_FoamWidth, 0.001));
				edge *= saturate(0.6 + ripple.x * 4);
				Light sun = GetMainLight(inputData.shadowCoord);
				half3 light = sun.color * sun.shadowAttenuation * saturate(sun.direction.y) + inputData.bakedGI;
				lit.rgb = lerp(lit.rgb, _FoamColor.rgb * light, edge * _FoamColor.a);
				alpha = max(alpha, edge * _FoamColor.a * saturate(depth / max(_EdgeFade * 0.5, 0.001)));

				lit.rgb = MixFog(lit.rgb, inputData.fogCoord);
				return half4(lit.rgb, alpha);
			}
			ENDHLSL
		}
	}
}
