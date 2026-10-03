Shader "GrabPassInvert"
{
    SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 neighbourGrabPos0 : TEXCOORD1;
                float4 neighbourGrabPos1 : TEXCOORD2;
                float4 neighbourGrabPos2 : TEXCOORD3;
                float4 neighbourGrabPos3 : TEXCOORD4;
                float4 neighbourGrabPos4 : TEXCOORD5;
                float4 neighbourGrabPos5 : TEXCOORD6;
                float4 neighbourGrabPos6 : TEXCOORD7;
                float4 neighbourGrabPos7 : TEXCOORD8;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate

                float blur = 0.01;
                float hstep = 1.0;
                float vstep = 0.0;
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.neighbourGrabPos0 = ComputeGrabScreenPos(UnityObjectToClipPos(float4(v.vertex.x + 1*blur*hstep, v.vertex.y + 1*blur*vstep, v.vertex.z, v.vertex.w)));
                o.neighbourGrabPos1 = ComputeGrabScreenPos(UnityObjectToClipPos(float4(v.vertex.x + 2*blur*hstep, v.vertex.y + 2*blur*vstep, v.vertex.z, v.vertex.w)));
                o.neighbourGrabPos2 = ComputeGrabScreenPos(UnityObjectToClipPos(float4(v.vertex.x + 3*blur*hstep, v.vertex.y + 3*blur*vstep, v.vertex.z, v.vertex.w)));
                o.neighbourGrabPos3 = ComputeGrabScreenPos(UnityObjectToClipPos(float4(v.vertex.x + 4*blur*hstep, v.vertex.y + 4*blur*vstep, v.vertex.z, v.vertex.w)));
                o.neighbourGrabPos4 = ComputeGrabScreenPos(UnityObjectToClipPos(float4(v.vertex.x - 1*blur*hstep, v.vertex.y - 1*blur*vstep, v.vertex.z, v.vertex.w)));
                o.neighbourGrabPos5 = ComputeGrabScreenPos(UnityObjectToClipPos(float4(v.vertex.x - 2*blur*hstep, v.vertex.y - 2*blur*vstep, v.vertex.z, v.vertex.w)));
                o.neighbourGrabPos6 = ComputeGrabScreenPos(UnityObjectToClipPos(float4(v.vertex.x - 3*blur*hstep, v.vertex.y - 3*blur*vstep, v.vertex.z, v.vertex.w)));
                o.neighbourGrabPos7 = ComputeGrabScreenPos(UnityObjectToClipPos(float4(v.vertex.x - 4*blur*hstep, v.vertex.y - 4*blur*vstep, v.vertex.z, v.vertex.w)));
                return o;
            }

            sampler2D _BackgroundTexture;

            half4 frag(v2f i) : SV_Target
            {
                float4 sum = float4(0, 0, 0, 0);
                
                //the amount to blur, i.e. how far off center to sample from 
                //1.0 -> blur by one pixel
                //2.0 -> blur by two pixels, etc.
                //float blur = radius / res; 
                
                //the direction of our blur
                //(1.0, 0.0) -> x-axis blur
                //(0.0, 1.0) -> y-axis blur
                //float hstep = dir.x;
                //float vstep = dir.y;

                float blur = 0.01;
                float hstep = 1.0;
                float vstep = 0.0;
                //apply blurring, using a 9-tap filter with predefined gaussian weights
                
                sum += tex2Dproj(_BackgroundTexture, i.neighbourGrabPos3) * 0.0162162162;
                sum += tex2Dproj(_BackgroundTexture, i.neighbourGrabPos2) * 0.0540540541;
                sum += tex2Dproj(_BackgroundTexture, i.neighbourGrabPos1) * 0.1216216216;
                sum += tex2Dproj(_BackgroundTexture, i.neighbourGrabPos0) * 0.1945945946;
                
                sum += tex2Dproj(_BackgroundTexture, i.grabPos) * 0.2270270270;
                
                sum += tex2Dproj(_BackgroundTexture, i.neighbourGrabPos4) * 0.1945945946;
                sum += tex2Dproj(_BackgroundTexture, i.neighbourGrabPos5) * 0.1216216216;
                sum += tex2Dproj(_BackgroundTexture, i.neighbourGrabPos6) * 0.0540540541;
                sum += tex2Dproj(_BackgroundTexture, i.neighbourGrabPos7) * 0.0162162162;

                return float4(sum.rgb, 1.0);
            }
            ENDCG
        }

    }
}