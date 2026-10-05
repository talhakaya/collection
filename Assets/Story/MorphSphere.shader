// A sphere whose six sides can each be flattened into the side of a cube: the cube that
// just fits inside the sphere, its eight corners on the sphere's surface.
//
// Each side has a value from 0 (the flat cube side) to 1 (the sphere). Every vertex is
// moved along the line from the sphere's centre through it, between where that line meets
// the sphere and where it meets the cube, so the corners - which are on both - never move.
//
// The mesh is a unit sphere (MorphSphere makes one, as six grids); only the direction of
// each vertex from the centre is used.
//
// The texture is an unwrapped cube (see MorphSphere for the layout). Where it is see-through
// the screen texture shows instead, spread over the whole of that side and not lit: the
// hole in the front is a screen. The screen's picture is given big pixels, scanlines
// and static on the way (Screen, in the forward pass).
Shader "Collection/Story/Morph Sphere"
{
	Properties
	{
		_BaseColor ("Colour", Color) = (1, 1, 1, 1)
		_BaseMap ("Unwrapped cube", 2D) = "white" {}
		_ScreenMap ("Screen (shows through the holes)", 2D) = "black" {}

		[Header(The screen as an old television)]
		_ScreenPixels ("Chunky pixels across (0 is off)", Range(0, 256)) = 56
		_Scanlines ("Scanlines", Range(0, 1)) = 0.45
		_ScanlineCount ("Scanlines down the screen", Range(8, 400)) = 56
		_ScanlineSpeed ("Scanlines rolling (lines a second)", Range(-20, 20)) = 1.5
		_Static ("Static", Range(0, 1)) = 0.18
		_StaticSize ("Static grains across", Range(8, 400)) = 112

		// The six sides, 0 the cube and 1 the sphere. Not shown on the material: the
		// MorphSphere component on each object sets them for that object, so sliders here
		// would do nothing.
		[HideInInspector] _Right ("Right (+X)", Range(0, 1)) = 1
		[HideInInspector] _Left ("Left (-X)", Range(0, 1)) = 1
		[HideInInspector] _Up ("Up (+Y)", Range(0, 1)) = 1
		[HideInInspector] _Down ("Down (-Y)", Range(0, 1)) = 1
		[HideInInspector] _Front ("Front (+Z)", Range(0, 1)) = 1
		[HideInInspector] _Back ("Back (-Z)", Range(0, 1)) = 1

		[Header(Where two sides of different values meet)]
		_Seam ("Seam width", Range(0.001, 0.5)) = 0.02
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _BaseColor;
			float4 _BaseMap_ST;
			float _Right;
			float _Left;
			float _Up;
			float _Down;
			float _Front;
			float _Back;
			float _Seam;
			float _ScreenPixels;
			float _Scanlines;
			float _ScanlineCount;
			float _ScanlineSpeed;
			float _Static;
			float _StaticSize;
		CBUFFER_END

		// Half the edge of the cube inside a sphere of radius 1.
		#define CUBE_HALF 0.57735027

		// Where the surface is in the direction d from the centre (d of length 1).
		float3 Morph(float3 d)
		{
			float3 a = abs(d);
			float largest = max(a.x, max(a.y, a.z));

			// The value of the side this direction looks at on each axis.
			float3 side = float3(d.x >= 0 ? _Right : _Left, d.y >= 0 ? _Up : _Down, d.z >= 0 ? _Front : _Back);

			// A point belongs to the side of its largest axis. Two sides of different values
			// cannot both keep their own shape along the edge they share - a straight edge and
			// an arc are different lines - so along an edge the lower value wins: a cube side
			// stays flat right up to its straight edges, and the rounder side next to it
			// comes down to meet it over the width of the seam. Each axis pulls the value down
			// to its own side's as the point nears that side; the largest axis pulls fully.
			float3 closeness = smoothstep(1 - _Seam, 1, a / largest);
			float3 pulled = lerp(1, side, closeness);
			float roundness = min(pulled.x, min(pulled.y, pulled.z));

			float3 onCube = d * (CUBE_HALF / largest);
			return lerp(onCube, d, roundness);
		}

		// The surface and its normal. The normal is measured: the surface is found again a
		// little way off in two directions, and the normal is square to both.
		void MorphWithNormal(float3 d, out float3 position, out float3 normal)
		{
			float3 other = abs(d.y) < 0.9 ? float3(0, 1, 0) : float3(1, 0, 0);
			float3 across = normalize(cross(other, d));
			float3 along = cross(d, across);

			const float offset = 0.004;
			position = Morph(d);
			float3 a = Morph(normalize(d + across * offset)) - position;
			float3 b = Morph(normalize(d + along * offset)) - position;
			normal = normalize(cross(a, b));
		}
		ENDHLSL

		Pass
		{
			Name "Forward"
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);
			TEXTURE2D(_ScreenMap);
			SAMPLER(sampler_ScreenMap);

			// A number from 0 to 1 that looks random, the same for the same grain.
			float Grain(float2 cell)
			{
				return frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453);
			}

			// What the screen shows at a point of its side (0 to 1 across and up): the screen
			// texture as an old television would show it.
			half3 Screen(float2 uv)
			{
				// Chunky pixels: every point of a block shows the middle of the block.
				float2 at = uv;
				if (_ScreenPixels >= 1)
				{
					at = (floor(uv * _ScreenPixels) + 0.5) / _ScreenPixels;
				}

				half3 picture = SAMPLE_TEXTURE2D(_ScreenMap, sampler_ScreenMap, at).rgb;

				// Scanlines: dark bands down the screen, drifting.
				float band = 0.5 + 0.5 * cos((uv.y * _ScanlineCount + _Time.y * _ScanlineSpeed) * 6.2831853);
				picture *= 1 - _Scanlines * (1 - band);

				// Static: grains of grey that change 24 times a second.
				float2 grain = floor(uv * _StaticSize) + floor(_Time.y * 24) * float2(17, 31);
				picture = lerp(picture, Grain(grain), _Static);

				return picture;
			}

			struct Attributes
			{
				float3 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				float2 sideUv : TEXCOORD1;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float2 sideUv : TEXCOORD2;
			};

			Varyings Vertex(Attributes input)
			{
				float3 position;
				float3 normal;
				MorphWithNormal(normalize(input.positionOS), position, normal);

				Varyings output;
				output.positionCS = TransformObjectToHClip(position);
				output.normalWS = TransformObjectToWorldNormal(normal);
				output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
				output.sideUv = input.sideUv;
				return output;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				half4 colour = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
				half3 normal = normalize(input.normalWS);

				Light light = GetMainLight();
				half3 lit = light.color * saturate(dot(normal, light.direction)) + SampleSH(normal);

				half3 screen = Screen(input.sideUv);
				return half4(lerp(screen, colour.rgb * lit, colour.a), 1);
			}
			ENDHLSL
		}

		// The shape casts a shadow.
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
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			float3 _LightDirection;

			float4 Vertex(float3 positionOS : POSITION) : SV_POSITION
			{
				float3 position;
				float3 normal;
				MorphWithNormal(normalize(positionOS), position, normal);

				float3 positionWS = TransformObjectToWorld(position);
				float3 normalWS = TransformObjectToWorldNormal(normal);
				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

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

		// So the shape, not the sphere it is made from, is what other passes see.
		Pass
		{
			Name "DepthOnly"
			Tags { "LightMode" = "DepthOnly" }
			ColorMask 0

			HLSLPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment

			float4 Vertex(float3 positionOS : POSITION) : SV_POSITION
			{
				return TransformObjectToHClip(Morph(normalize(positionOS)));
			}

			half4 Fragment() : SV_Target
			{
				return 0;
			}
			ENDHLSL
		}
	}
}
