// Screen-space tilt-shift blur. Blur strength comes from the pixel's vertical screen position,
// not scene depth. Run as two URP Full Screen Pass features: pass 0 (horizontal), then pass 1 (vertical).
Shader "LittleGuy/TiltShiftBlur"
{
    Properties
    {
        [Header(Top band (0 is screen bottom and 1 is screen top))]
        _TopBlurStart ("Top Blur Start", Range(0, 1)) = 0.85
        _TopBlurEnd ("Top Blur End", Range(0, 1)) = 0.97
        [Header(Bottom band)]
        _BottomBlurStart ("Bottom Blur Start", Range(0, 1)) = 0.15
        _BottomBlurEnd ("Bottom Blur End", Range(0, 1)) = 0.03
        [Header(Strength)]
        _BlurSize ("Max Blur Size (fraction of screen height)", Range(0, 0.05)) = 0.012
        [Toggle(_SHOW_MASK)] _ShowMask ("Debug: Show Mask", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off Blend Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        #pragma multi_compile_local_fragment _ _SHOW_MASK

        #define KERNEL_RADIUS 8

        float _TopBlurStart;
        float _TopBlurEnd;
        float _BottomBlurStart;
        float _BottomBlurEnd;
        float _BlurSize;
        // Global switch (Shader.SetGlobalFloat), e.g. to turn the blur off in the character creator. 0 = blur on.
        float _TiltShiftDisabled;

        // 0 = sharp, 1 = fully blurred. Ramps from Start (no blur) to End (full blur) for each band.
        float Ramp(float y, float start, float end)
        {
            float range = end - start;
            if (abs(range) < 1e-4)
                return step(0.0, (y - start) * sign(range + 1e-5));
            return saturate((y - start) / range);
        }

        float BlurMask(float y)
        {
            float top = Ramp(y, _TopBlurStart, _TopBlurEnd);
            float bottom = Ramp(y, _BottomBlurStart, _BottomBlurEnd);
            return smoothstep(0.0, 1.0, max(top, bottom)) * (1.0 - _TiltShiftDisabled);
        }

        half4 BlurAlong(float2 uv, float2 direction)
        {
            float mask = BlurMask(uv.y);
            half4 center = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv, 0);

            #if defined(_SHOW_MASK)
            return half4(1.0 - mask.xxx, 1.0);
            #endif

            if (mask <= 0.001)
                return center;

            // Blur radius is a fraction of screen height; the horizontal step is aspect-corrected so the blur stays round.
            float2 aspect = float2(_ScreenParams.y / _ScreenParams.x, 1.0);
            float2 stepUV = direction * aspect * (_BlurSize * mask / KERNEL_RADIUS);
            float sigma = KERNEL_RADIUS * 0.5;

            half4 sum = center;
            float weightSum = 1.0;
            [unroll]
            for (int i = 1; i <= KERNEL_RADIUS; i++)
            {
                float w = exp(-(i * i) / (2.0 * sigma * sigma));
                sum += SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv + stepUV * i, 0) * w;
                sum += SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv - stepUV * i, 0) * w;
                weightSum += 2.0 * w;
            }
            return sum / weightSum;
        }
        ENDHLSL

        Pass
        {
            Name "TiltShiftBlurHorizontal"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return BlurAlong(input.texcoord, float2(1, 0));
            }
            ENDHLSL
        }

        Pass
        {
            Name "TiltShiftBlurVertical"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return BlurAlong(input.texcoord, float2(0, 1));
            }
            ENDHLSL
        }
    }
}
