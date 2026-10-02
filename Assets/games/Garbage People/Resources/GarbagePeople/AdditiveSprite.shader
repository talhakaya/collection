// Phaser's ADD blend mode for sprites: the colour, scaled by its alpha, added to what is
// behind it. Kept in Resources so that builds include it.
//
// Built on Unity's own sprite shader code, so the SpriteRenderer's colour (the tint and
// alpha), which Unity passes as _RendererColor rather than in the vertices, is applied.
Shader "GarbagePeople/AdditiveSprite"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha One

		Pass
		{
			CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			fixed4 frag(v2f i) : SV_Target
			{
				return SampleSpriteTexture(i.texcoord) * i.color;
			}
			ENDCG
		}
	}
}
