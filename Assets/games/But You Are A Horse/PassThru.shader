// In the collection: the original drew the screen back over itself after RenderGrayScale
// had changed every pixel on the CPU (ReadPixels in OnPostRender, which URP never calls).
// The same arithmetic is done here instead, on the picture the camera renders into a
// texture. It works on gamma-space bytes, as the original did, including the wrap-around
// of the byte cast that gives the garbled colours for ratios below zero.
Shader "Games/ButYouAreAHorse/PassThru" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Ratio ("Ratio", Float) = 1
}

SubShader
{
	Tags { "Queue"="Overlay" "RenderType"="Opaque" }
	Pass
	{
		ZTest Always Cull Off ZWrite Off

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

sampler2D _MainTex;
float _Ratio;

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert (appdata_img v)
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
	return o;
}

float3 wrapByte (float3 v)
{
	v = trunc(v);
	return v - 256.0 * floor(v / 256.0);
}

float4 frag (v2f i) : SV_Target
{
	float3 c = tex2D(_MainTex, i.uv).rgb;
#ifndef UNITY_COLORSPACE_GAMMA
	c = LinearToGammaSpace(saturate(c));
#endif
	c = floor(c * 255.0 + 0.5);
	float gray = floor((c.r + c.g + c.b) / 3.0);
	if (_Ratio == 0.0)
	{
		c = float3(gray, gray, gray);
	}
	else if (_Ratio < 1.0)
	{
		c = wrapByte(_Ratio * c + (1.0 - _Ratio) * gray);
	}
	c = c / 255.0;
#ifndef UNITY_COLORSPACE_GAMMA
	c = GammaToLinearSpace(c);
#endif
	return float4(c, 1.0);
}
ENDCG

	}
}

Fallback off

}
