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
// hole in the front is a screen.
Shader "Collection/Story/Morph Sphere"
{
	Properties
	{
		_BaseColor ("Colour", Color) = (1, 1, 1, 1)
		_BaseMap ("Unwrapped cube", 2D) = "white" {}
		_ScreenMap ("Screen (shows through the holes)", 2D) = "black" {}

		[Header(Sides. 0 is the cube and 1 is the sphere)]
		_Right ("Right (+X)", Range(0, 1)) = 1
		_Left ("Left (-X)", Range(0, 1)) = 1
		_Up ("Up (+Y)", Range(0, 1)) = 1
		_Down ("Down (-Y)", Range(0, 1)) = 1
		_Front ("Front (+Z)", Range(0, 1)) = 1
		_Back ("Back (-Z)", Range(0, 1)) = 1

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

				half3 screen = SAMPLE_TEXTURE2D(_ScreenMap, sampler_ScreenMap, input.sideUv).rgb;
				return half4(lerp(screen, colour.rgb * lit, colour.a), 1);
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
