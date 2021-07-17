#ifndef MASSIVE_CLOUDS_EXPOSURE
#define MASSIVE_CLOUDS_EXPOSURE

#include "Packages/com.unity.render-pipelines.high-definition-config/Runtime/ShaderConfig.cs.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

#ifndef MASSIVE_CLOUDS_HLSL

    TEXTURE2D(_ExposureTexture);
    TEXTURE2D(_PrevExposureTexture);
    float _ProbeExposureScale;

#endif


float _Exposure;

inline void PrepareExposure()
{
    #if SHADEROPTIONS_PRE_EXPOSITION
    _Exposure = LOAD_TEXTURE2D(_ExposureTexture, int2(0, 0)).x * _ProbeExposureScale;
    #else
    _Exposure = _ProbeExposureScale;
    #endif
}

inline float ExposureMultiplier()
{
    return _Exposure;
}

inline float ExposureMultiplier2()
{
    return _Exposure * _Exposure;
}

#endif
