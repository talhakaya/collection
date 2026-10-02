Shader "Custom/PassThru" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader
{
	Pass
	{
		ZTest Off Cull Off ZWrite Off
		Fog { Mode off }

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform float2 _MainTex_TexelSize;

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert (appdata_img v)
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.vertex.xy;

	return o;
}

float4 frag (v2f i) : COLOR
{
	return tex2D(_MainTex, i.uv);
}
ENDCG

	}
}

Fallback off

}