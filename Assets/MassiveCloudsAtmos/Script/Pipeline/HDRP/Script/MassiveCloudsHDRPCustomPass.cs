using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Object = UnityEngine.Object;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsHDRPCustomPass : CustomPass, IFullScreenDrawable
    {
        private static readonly int FullScreenCameraColorBlitPassId = 0;
        private static readonly int FullScreenBlitPassId = 1;
        private static readonly string ShaderName = "Hidden/MassiveCloudsCustomPass";

        private Material fullscreenPassMaterial;
        public AbstractMassiveClouds Renderer;
        public bool LookUpTransparentDepthPrepass = true;
        public Light Sun;
        public Transform Moon;

        private DynamicRenderTexture rt;
        private MassiveCloudsHDRPBlitter fullScreenBlitter;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            fullscreenPassMaterial = new Material(Shader.Find(ShaderName));
            fullScreenBlitter = new MassiveCloudsHDRPBlitter();
        }

#if UNITY_2020_2_OR_NEWER
        protected override void Execute(CustomPassContext ctx)
#else
        protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera camera,
            CullingResults cullingResult)
#endif
        {
            const RenderTextureFormat formatAlpha = RenderTextureFormat.ARGBFloat;

#if UNITY_2020_2_OR_NEWER
            var cmd = ctx.cmd;
            var camera = ctx.hdCamera;
#endif

            if (Renderer == null) return;

            Renderer.UpdateClouds(Sun, Moon);

            CommandBufferUtility.EnableKeyword("_MASSIVE_CLOUDS_HDRP");

            ResolveMSAAColorBuffer(cmd, camera);

#if UNITY_2020_2_OR_NEWER
            var cameraColorBuffer = ctx.cameraColorBuffer;
            var cameraDepthBuffer = ctx.cameraDepthBuffer;
#else
            RTHandle cameraColorBuffer;
            RTHandle cameraDepthBuffer;
            GetCameraBuffers(out cameraColorBuffer, out cameraDepthBuffer);
#endif
            var rtInfo = CreateRenderTextureDesc(camera.camera);
            if (rt == null) rt = new DynamicRenderTexture(formatAlpha);
            rt.Update(camera.camera, rtInfo);

            if (camera.camera.cameraType == CameraType.Reflection)
                cmd.SetGlobalFloat("_MassiveCloudsProbeScale", 0.001f);
            else
                cmd.SetGlobalFloat("_MassiveCloudsProbeScale", 1f);
            cmd.SetGlobalFloat("_SkyIntensity", 1.0f);

            cmd.SetRenderTarget(rt.GetRenderTexture(camera.camera));
            if (LookUpTransparentDepthPrepass)
                cmd.SetGlobalTexture("_CameraDepthTexture", cameraDepthBuffer);
            cmd.DrawProcedural(Matrix4x4.identity, fullscreenPassMaterial, FullScreenCameraColorBlitPassId, MeshTopology.Triangles, 3, 1, null);

            var massiveCloudsCtx = CreateMassiveCloudsPassContext(
                cmd,
                camera.camera,
                rt.GetRenderTexture(camera.camera));

            Renderer.BuildCommandBuffer(massiveCloudsCtx, this);
        }

        private int GetEyeIndex(HDCamera camera)
        {
            switch (camera.camera.stereoActiveEye)
            {
                case Camera.MonoOrStereoscopicEye.Mono:
                case Camera.MonoOrStereoscopicEye.Left:
                    return 0;
                case Camera.MonoOrStereoscopicEye.Right:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private RenderTextureDesc CreateRenderTextureDesc(Camera camera)
        {
            return new RenderTextureDesc(camera.pixelWidth, camera.pixelHeight);
        }

        private MassiveCloudsPassContext CreateMassiveCloudsPassContext(
            CommandBuffer cmd,
            Camera camera,
            RenderTexture source)
        {
            var colorBufferDesc = new RenderTextureDesc(camera.pixelWidth, camera.pixelHeight);
            return new MassiveCloudsPassContext(
                cmd,
                camera,
                colorBufferDesc,
                source);
        }

        protected override void Cleanup()
        {
            Renderer.Clear();
            if (Application.isPlaying)
                Object.Destroy(fullscreenPassMaterial);
            else
                Object.DestroyImmediate(fullscreenPassMaterial);
        }

        public void Draw(CommandBuffer commandBuffer, RenderTargetIdentifier source)
        {
            SetRenderTargetAuto(commandBuffer);
            fullScreenBlitter.Blit(commandBuffer, source);
        }
    }
}