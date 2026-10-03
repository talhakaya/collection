// In the collection: stands in for Unity 4's built-in "Diffuse" and "Transparent/Diffuse"
// shaders, which do not draw under URP (everything using them came out magenta).
//
// It is not a URP lit shader. The game was lit by the built-in forward renderer of 2014:
// Lambert, in Gamma space, with a light's contribution doubled and a falloff that dies
// at the light's range. URP's own lights are physical and in Linear space, and the three
// lit scenes looked nothing like themselves through URP/Lit. So the old arithmetic is
// done here, on light data that LegacyLighting.cs uploads each frame.
Shader "OdeToCactus/LegacyDiffuse"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		[Toggle] _Transparent ("Transparent", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
		[Enum(Off,0,On,1)] _ZWrite ("ZWrite", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
		Pass
		{
			Tags { "LightMode"="UniversalForward" }
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define OTC_MAX_LIGHTS 24

			TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				half4 _Color;
				half _Transparent;
			CBUFFER_END

			// xyz: world position, w: 1 / range squared
			float4 _OtcLightPos[OTC_MAX_LIGHTS];
			// rgb: the light's colour times its Unity 4 intensity, as gamma values
			float4 _OtcLightColor[OTC_MAX_LIGHTS];
			float _OtcLightCount;
			float4 _OtcAmbient;

			struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; float2 uv : TEXCOORD0; };
			struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 positionWS : TEXCOORD1; float3 normalWS : TEXCOORD2; };

			Varyings vert (Attributes v)
			{
				Varyings o;
				o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
				o.positionCS = TransformWorldToHClip(o.positionWS);
				o.normalWS = TransformObjectToWorldNormal(v.normalOS);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			half4 frag (Varyings i) : SV_Target
			{
				half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				// Everything below is in gamma values, as the game computed it.
				half3 albedo = LinearToSRGB(tex.rgb) * LinearToSRGB(_Color.rgb);
				float3 n = normalize(i.normalWS);

				// Unity 4's Lambert: ambient and every light were doubled.
				half3 light = _OtcAmbient.rgb * 2.0;
				int count = (int)_OtcLightCount;
				for (int k = 0; k < count; k++)
				{
					float3 toLight = _OtcLightPos[k].xyz - i.positionWS;
					float distSqr = dot(toLight, toLight);
					float q = distSqr * _OtcLightPos[k].w;          // (distance / range) squared
					float atten = 1.0 / (1.0 + 25.0 * q);           // the built-in falloff curve
					atten *= saturate((1.0 - q) * 5.0);             // ...which ended at the range
					float ndl = saturate(dot(n, toLight * rsqrt(max(distSqr, 1e-6))));
					light += _OtcLightColor[k].rgb * (ndl * atten * 2.0);
				}

				half3 c = saturate(albedo * light);
				half alpha = lerp(1.0, tex.a * _Color.a, _Transparent);
				return half4(SRGBToLinear(c), alpha);
			}
			ENDHLSL
		}
	}
}
