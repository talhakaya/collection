// A flat colour drawn over everything that has been drawn before it, whatever is nearer or farther. Late in the
// see-through queue, so only what is put after it (queue 3901 and up) shows on top.
//
// The story's intro uses it as the black the character stands in: the world and the water are under it, the
// character is drawn after it, and fading its alpha brings the world in behind the character.
Shader "Collection/Story/Blackout"
{
	Properties
	{
		_Color ("Colour", Color) = (0, 0, 0, 1)
	}

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent+900" "RenderPipeline" = "UniversalPipeline" }

		Pass
		{
			Name "Blackout"
			Tags { "LightMode" = "UniversalForward" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Always
			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _Color;
			CBUFFER_END

			// The mesh is ignored but for its corners: the quad covers the screen wherever it is.
			float4 Vertex(float3 positionOS : POSITION) : SV_POSITION
			{
				return float4(positionOS.xy * 2, UNITY_NEAR_CLIP_VALUE, 1);
			}

			half4 Fragment() : SV_Target
			{
				return _Color;
			}
			ENDHLSL
		}
	}
}
