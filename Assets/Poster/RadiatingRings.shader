// For RadiatingOutline: the rings around a silhouette, drawn on a RawImage covering the screen. Its texture is
// the jump flood's result, which gives each pixel the nearest silhouette pixel; the distance to it is how far out
// the pixel is, and a ring is wherever that distance is a whole number of `_Spacing`s, give or take half
// `_Width`. `_Offset` moves them all outward, so that a new ring is always coming out of the silhouette.
//
// Spacing and width are in pixels of a screen 1080 high, so a recording at another size looks the same.
Shader "Poster/Radiating Rings"
{
    Properties
    {
        [PerRendererData] _MainTex ("Nearest Silhouette Pixel", 2D) = "black" {}
        _LineColour ("Line Colour", Color) = (1, 1, 1, 1)
        _BackColour ("Background Colour", Color) = (0, 0, 0, 1)
        _FillColour ("Silhouette Colour", Color) = (1, 1, 1, 1)
        _Spacing ("Spacing (px at 1080)", Float) = 36
        _Width ("Line Width (px at 1080)", Float) = 9
        _Gap ("Gap Before The First Ring (px at 1080)", Float) = 0
        _FadeFrom ("Fade Out From (px at 1080, 0 for none)", Float) = 0
        _FadeOver ("Fade Out Over (px at 1080)", Float) = 600
        _Offset ("Offset (px at 1080)", Float) = 0
        [Header(Rainbow)]
        [Toggle] _Rainbow ("Rainbow Over The Lines", Float) = 0
        _RainbowLength ("Distance For One Round Of Colours (px at 1080)", Float) = 1200
        _RainbowStart ("Starting Hue (0 to 1)", Range(0, 1)) = 0
        _RainbowSaturation ("Saturation", Range(0, 1)) = 0.8
        [Toggle] _RainbowTravels ("Colours Travel With The Rings", Float) = 0
        _RainbowSpeed ("Colours Move Out At (px a second at 1080, on top)", Float) = 0
        [Header(Impacts)]
        [Toggle] _Impacts ("Shown Only By Impacts (Set From A Script)", Float) = 0
        _ImpactSoftness ("Softness Of An Impact's Front (px at 1080)", Float) = 160
        [HideInInspector] _ImpactFronts ("Impact Fronts", Vector) = (0, 0, 0, 0)
        [HideInInspector] _ImpactStrengths ("Impact Strengths", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _LineColour, _BackColour, _FillColour;
            float _Spacing, _Width, _Gap, _FadeFrom, _FadeOver, _Offset;
            float _Rainbow, _RainbowLength, _RainbowStart, _RainbowSaturation, _RainbowTravels, _RainbowSpeed;
            float _Impacts, _ImpactSoftness;
            float4 _ImpactFronts, _ImpactStrengths;

            // With impacts, the lines show only where one has reached: up to four at once, each a front going out
            // from the silhouette (how far out, at 1080) and how strongly its lines show. A pixel shows the
            // strongest that has reached it, so a new impact never takes away what an older one shows.
            float ImpactShown(float d)
            {
                float4 reached = 1 - smoothstep(_ImpactFronts - _ImpactSoftness, _ImpactFronts, d);
                float4 shown = reached * _ImpactStrengths;
                return max(max(shown.x, shown.y), max(shown.z, shown.w));
            }

            // A colour round the colour wheel, at full brightness.
            float3 Hue(float h, float saturation)
            {
                float3 pure = saturate(abs(frac(h + float3(0, 2.0 / 3, 1.0 / 3)) * 6 - 3) - 1);
                return lerp(1, pure, saturation);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 pixels = _MainTex_TexelSize.zw;
                float2 here = i.uv * pixels;
                float2 seed = tex2D(_MainTex, i.uv).xy;
                // In pixels of a screen 1080 high.
                float scale = 1080.0 / pixels.y;
                float d = distance(here, seed) * scale;

                if (d < 0.75 * scale)
                    return _FillColour;

                // How far this pixel is from the nearest ring's middle.
                float along = (d - _Gap - _Offset) / _Spacing;
                float fromMiddle = abs(frac(along + 0.5) - 0.5) * _Spacing;
                // A pixel's worth of softening, so the lines are smooth and exactly `_Width` wide at any size.
                float soft = max(fwidth(d), 0.0001);
                float ring = 1 - smoothstep(_Width * 0.5 - soft * 0.5, _Width * 0.5 + soft * 0.5, fromMiddle);
                // None inside the gap: the rings come out of it.
                ring *= smoothstep(_Gap - soft, _Gap + soft, d);
                if (_FadeFrom > 0)
                    ring *= 1 - saturate((d - _FadeFrom) / max(_FadeOver, 1));
                if (_Impacts > 0.5)
                    ring *= ImpactShown(d);

                // Ticked, the line colour is multiplied by a hue going round the wheel with distance. The colours
                // stay in place unless they travel with the rings, or move out at their own `_RainbowSpeed` (on top
                // of the rings' movement, if they travel with them; negative for inward).
                float4 lineColour = _LineColour;
                if (_Rainbow > 0.5)
                {
                    float moved = (_RainbowTravels > 0.5 ? _Offset : 0) + _Time.y * _RainbowSpeed;
                    float turns = (d - moved) / max(_RainbowLength, 1);
                    lineColour.rgb *= Hue(_RainbowStart + turns, _RainbowSaturation);
                }

                return lerp(_BackColour, lineColour, ring);
            }
            ENDCG
        }
    }
}
