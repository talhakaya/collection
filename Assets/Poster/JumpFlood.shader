// For RadiatingOutline: finds, for every pixel of the screen, the nearest pixel of a silhouette ("jump flooding").
// Each pixel holds the position (in pixels) of the nearest silhouette pixel found so far.
//
// Pass 0, Seed: a silhouette pixel holds its own position; any other, a position far off.
// Pass 1, Jump: a pixel looks at the nearest found by itself and by the eight pixels `_Step` away around it, and
// keeps the nearest. Done with steps halving from half the screen down to one pixel, every pixel ends up holding
// (very nearly) the nearest silhouette pixel.
Shader "Hidden/Poster/Jump Flood"
{
    Properties
    {
        _MainTex ("Source", 2D) = "black" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    // Far enough off that any real pixel is nearer.
    #define NOWHERE float2(-100000, -100000)

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    struct v2f
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord;
        return o;
    }
    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        // Seed. _MainTex is the mask; _MaskRect is where on the screen it lies (x, y, width, height, as parts of
        // the screen). _KeyColour, when _UseKey is 1: the mask is everything that is not that colour (a camera's
        // picture over a background of it); otherwise the mask is the texture's alpha.
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _MaskRect;
            float4 _ScreenPixels;
            float4 _KeyColour;
            float _UseKey;

            float4 frag(v2f i) : SV_Target
            {
                float2 maskUV = (i.uv - _MaskRect.xy) / _MaskRect.zw;
                float2 here = i.uv * _ScreenPixels.xy;
                if (any(maskUV < 0) || any(maskUV > 1))
                    return float4(NOWHERE, 0, 0);

                float4 c = tex2D(_MainTex, maskUV);
                bool inside = _UseKey > 0.5 ? distance(c.rgb, _KeyColour.rgb) > 0.25 : c.a > 0.5;
                return float4(inside ? here : NOWHERE, 0, 0);
            }
            ENDCG
        }

        // Jump.
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Step;
            float4 _ScreenPixels;

            float4 frag(v2f i) : SV_Target
            {
                float2 here = i.uv * _ScreenPixels.xy;
                float2 best = NOWHERE;
                float bestDistance = 1e20;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 uv = i.uv + float2(x, y) * _Step * _MainTex_TexelSize.xy;
                        if (any(uv < 0) || any(uv > 1))
                            continue;
                        float2 seed = tex2Dlod(_MainTex, float4(uv, 0, 0)).xy;
                        float2 apart = seed - here;
                        float d = dot(apart, apart);
                        if (d < bestDistance)
                        {
                            bestDistance = d;
                            best = seed;
                        }
                    }
                }
                return float4(best, 0, 0);
            }
            ENDCG
        }
    }
}
