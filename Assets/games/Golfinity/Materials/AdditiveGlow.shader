Shader "Custom/AdditiveGlow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
			"RenderPipeline"="UniversalPipeline"
			"IgnoreProjector"="True"
			"CanUseSpriteAtlas"="True"
			"PreviewType"="Plane"
		}

		Cull Off
		ZWrite Off
		ZTest LEqual
		Blend One One

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct Varyings
			{
				float4 positionHCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			float4 _MainTex_ST;
			float4 _Color;

			Varyings vert(Attributes IN)
			{
				Varyings OUT;
				OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				OUT.color = IN.color * _Color;
				return OUT;
			}

			float4 frag(Varyings IN) : SV_Target
			{
				float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
				float4 col = texColor * IN.color;
				// Additive (Soft): premultiply by alpha so the glow's intensity fades
				// with alpha (e.g. the reveal animation) under a straight One+One blend.
				col.rgb *= col.a;
				return col;
			}
			ENDHLSL
		}
	}
	FallBack Off
}
