#ifndef MASSIVE_CLOUDS_PIPELINE_DEPENDENT_INCLUDED
#define MASSIVE_CLOUDS_PIPELINE_DEPENDENT_INCLUDED

#define MASSIVE_CLOUDS_HLSL

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

CBUFFER_START(UnityPerCameraRare)
    float4 unity_CameraWorldClipPlanes[6];

#if defined(USING_STEREO_MATRICES)
    #define glstate_matrix_projection unity_StereoMatrixP[unity_StereoEyeIndex]
    #define unity_MatrixV unity_StereoMatrixV[unity_StereoEyeIndex]
    #define unity_MatrixInvV unity_StereoMatrixInvV[unity_StereoEyeIndex]
    #define unity_MatrixVP unity_StereoMatrixVP[unity_StereoEyeIndex]

    #define unity_CameraProjection unity_StereoCameraProjection[unity_StereoEyeIndex]
    #define unity_CameraInvProjection unity_StereoCameraInvProjection[unity_StereoEyeIndex]
    #define unity_WorldToCamera unity_StereoWorldToCamera[unity_StereoEyeIndex]
    #define unity_CameraToWorld unity_StereoCameraToWorld[unity_StereoEyeIndex]
    #define _WorldSpaceCameraPos unity_StereoWorldSpaceCameraPos[unity_StereoEyeIndex]
#endif

#if !defined(USING_STEREO_MATRICES)
    // Projection matrices of the camera. Note that this might be different from projection matrix
    // that is set right now, e.g. while rendering shadows the matrices below are still the projection
    // of original camera.
    float4x4 unity_CameraProjection;
    float4x4 unity_CameraInvProjection;
    float4x4 unity_WorldToCamera;
    float4x4 unity_CameraToWorld;
#endif
CBUFFER_END

float3 ODSOffset(float3 worldPos, float ipd)
{
    //based on google's omni-directional stereo rendering thread
    const float EPSILON = 2.4414e-4;
    float3 worldUp = float3(0.0, 1.0, 0.0);
    float3 camOffset = worldPos.xyz - _WorldSpaceCameraPos.xyz;
    float4 direction = float4(camOffset.xyz, dot(camOffset.xyz, camOffset.xyz));
    direction.w = max(EPSILON, direction.w);
    direction *= rsqrt(direction.w);

    float3 tangent = cross(direction.xyz, worldUp.xyz);
    if (dot(tangent, tangent) < EPSILON)
        return float3(0, 0, 0);
    tangent = normalize(tangent);

    float directionMinusIPD = max(EPSILON, direction.w*direction.w - ipd*ipd);
    float a = ipd * ipd / direction.w;
    float b = ipd / direction.w * sqrt(directionMinusIPD);
    float3 offset = -a*direction.xyz + b*tangent;
    return offset;
}

inline float4 UnityObjectToClipPosODS(float3 inPos)
{
    float4 clipPos;
    float3 posWorld = mul(UNITY_MATRIX_M, float4(inPos, 1.0)).xyz;
#if defined(STEREO_CUBEMAP_RENDER_ON)
    float3 offset = ODSOffset(posWorld, unity_HalfStereoSeparation.x);
    clipPos = mul(UNITY_MATRIX_VP, float4(posWorld + offset, 1.0));
#else
    clipPos = mul(UNITY_MATRIX_VP, float4(posWorld, 1.0));
#endif
    return clipPos;
}

// Tranforms position from object to homogenous space
inline float4 UnityObjectToClipPos(in float3 pos)
{
    #if defined(STEREO_CUBEMAP_RENDER_ON)
    return UnityObjectToClipPosODS(pos);
    #else
    // More efficient than computing M*VP matrix product
    return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(pos, 1.0)));
    #endif
}

inline float4 UnityObjectToClipPos(float4 pos) // overload for float4; avoids "implicit truncation" warning for existing shaders
{
    return UnityObjectToClipPos(pos.xyz);
}

#endif