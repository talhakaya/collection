// In the collection: the original changed every pixel of the screen on the CPU
// (ReadPixels in OnPostRender, which URP never calls). The same arithmetic is done here,
// on the picture the camera renders into a texture: one screen row in four, picked at
// random each frame, is drawn grey and darkened by a random amount; the other rows are
// mixed with their grey by the ratio (a random ratio per pixel in mode 2). It works on
// gamma-space bytes, including the wrap-around of the byte cast for ratios above one.
Shader "Games/Oracle/PassThru" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Ratio ("Ratio", Float) = 0
	_Mode ("Mode", Float) = 0
	_Scanline ("Scanline", Float) = 0
	_Seed ("Seed", Float) = 0
	_Rows ("Rows", Float) = 1080
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
float _Mode;
float _Scanline;
float _Seed;
float _Rows;

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

float hash (float2 p)
{
	return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
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
	float row = floor(i.uv.y * _Rows);
	float gray = (c.r + c.g + c.b) / 3.0;
	if (_Mode != 0.0 && fmod(row, 4.0) == _Scanline)
	{
		float degree = 2.0 + hash(float2(row, _Seed));
		float g = floor(gray / degree);
		c = float3(g, g, g);
	}
	else
	{
		float ratio = _Ratio;
		if (_Mode == 2.0)
		{
			ratio = hash(i.uv * 1000.0 + _Seed);
		}
		float g = floor(gray);
		c = wrapByte(g * ratio + (1.0 - ratio) * c);
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
