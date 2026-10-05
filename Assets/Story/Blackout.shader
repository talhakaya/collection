// A flat colour drawn at the very back of the picture, late in the see-through queue: over whatever has been
// drawn that did not mark how far away it is (the sky, water, anything else see-through), and behind everything
// solid.
//
// The story's intro uses it as the black the character stands in: the sky and the sea are under it, the character
// (solid) is in front of it, and fading its alpha brings the sea in behind the character.
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
			ZTest LEqual
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
				return float4(positionOS.xy * 2, UNITY_RAW_FAR_CLIP_VALUE, 1);
			}

			half4 Fragment() : SV_Target
			{
				return _Color;
			}
			ENDHLSL
		}
	}
}
