using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.XR;

namespace Mewlist.MassiveClouds
{
    internal enum MassiveCloudsProfileId
    {
        RenderSky
    }

    public class MassiveCloudsSkyRenderer : SkyRenderer, IFullScreenDrawable
    {
        private static readonly int FullScreenCameraColorBlitPassId = 0;
        private static readonly int FullScreenBlitPassId = 1;

        private static readonly int RenderCubemapID = 0;       // FragBaking
        private static readonly int RenderFullscreenSkyID = 1; // FragRender
        private static readonly int pixelCoordToViewDirWS = Shader.PropertyToID("_PixelCoordToViewDirWS");

        private Material skyMaterial; // Renders a cubemap into a render texture (can be cube or 2D)
        private Material textureArrayBlitMaterial;
        private Material blitMaterial;
        private readonly MaterialPropertyBlock skyPropertyBlock = new MaterialPropertyBlock();
        private AbstractMassiveClouds currentMassiveClouds;
        private DynamicRenderTexture rt;
        private int skyHash = 0;
        private BuiltinSkyParameters currentBuiltinSkyParameters;

        private MassiveCloudsHDRPBlitter blitter;
        private MassiveCloudsHDRPBlitter fullScreenBlitter;

        private Material TextureArrayBlitMaterial
        {
            get
            {
                if (!textureArrayBlitMaterial) textureArrayBlitMaterial = new Material(Shader.Find("Hidden/MassiveCloudsAtmos/HDRP/Blit"));
                return textureArrayBlitMaterial;
            }
        }

        private Material SkyMaterial
        {
            get
            {
                if (!skyMaterial) skyMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Mewlist/HDRP/Sky/MassiveCloudsSky"));
                return skyMaterial;
            }
        }

        public override void Build()
        {
            blitter = new MassiveCloudsHDRPBlitter();
            fullScreenBlitter = new MassiveCloudsHDRPBlitter();
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(skyMaterial);
            CoreUtils.Destroy(textureArrayBlitMaterial);
            rt?.Dispose();
            rt = null;
            currentMassiveClouds?.Clear();
            currentMassiveClouds = null;
        }

        private int CalculateSkyHash(AbstractMassiveClouds massiveClouds)
        {
            if (massiveClouds is MassiveCloudsPhysicsCloud)
            {
                return massiveClouds.SkyPass.GetHashCode();
            }
            else
            {
                return 0;
            }
        }

        protected override bool Update(BuiltinSkyParameters builtinParams)
        {
            var sky = builtinParams.skySettings as MassiveCloudsHDRPSky;
            if (currentMassiveClouds == null || currentMassiveClouds != sky.MassiveClouds.value)
                currentMassiveClouds = sky.MassiveClouds.value;
            if (currentMassiveClouds == null) return false;
            if (builtinParams.hdCamera.camera.cameraType == CameraType.Reflection) return false;
            currentMassiveClouds.UpdateClouds(builtinParams.sunLight, null);
            var hash = CalculateSkyHash(currentMassiveClouds);
            if (hash != skyHash)
            {
                skyHash = hash;
                return true;
            }
            return false;
        }

        public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
        {
            if (currentMassiveClouds == null) return;

            CommandBufferUtility.EnableKeyword("_MASSIVE_CLOUDS_HDRP");
            SkyMaterial.EnableKeyword("_MASSIVE_CLOUDS_HDRP");
            currentBuiltinSkyParameters = builtinParams;
            using (new ProfilingScope(builtinParams.commandBuffer, ProfilingSampler.Get(MassiveCloudsProfileId.RenderSky)))
            {
                var passId = renderForCubemap ? RenderCubemapID : RenderFullscreenSkyID;
                var cmd = builtinParams.commandBuffer;
                var camera = builtinParams.hdCamera.camera;
                var skyIntensity = GetSkyIntensity(builtinParams.skySettings, builtinParams.debugSettings);
                cmd.SetGlobalFloat("_SkyIntensity", skyIntensity);

                var msaaFrameSetting = builtinParams.hdCamera.frameSettings.IsEnabled(FrameSettingsField.MSAA); 
                var litShaderMode = builtinParams.hdCamera.frameSettings.litShaderMode; 
                if (msaaFrameSetting && litShaderMode == LitShaderMode.Forward &&
                    (int) builtinParams.hdCamera.msaaSamples > 1)
                    MassiveCloudsHDRPBlitter.SetMSAAEnabled(true);
                else
                    MassiveCloudsHDRPBlitter.SetMSAAEnabled(false);

                if (renderForCubemap)
                {
                    var newSky = builtinParams.skySettings as MassiveCloudsHDRPSky;
                    float phi = -Mathf.Deg2Rad * newSky.rotation.value; // -rotation to match Legacy
                    float intensity = GetSkyIntensity(newSky, builtinParams.debugSettings);
                    cmd.SetGlobalFloat("_MassiveCloudsProbeScale", 1f);
                    SkyMaterial.DisableKeyword("_MASSIVE_CLOUDS_HDRP");
                    if (renderSunDisk)
                        SkyMaterial.DisableKeyword("_REMOVE_SUN");
                    else
                        SkyMaterial.EnableKeyword("_REMOVE_SUN");

                    currentMassiveClouds.Sun.ApplySunParameters(SkyMaterial, 1f);
                    currentMassiveClouds.Moon.ApplyMoonParameters(SkyMaterial, 1f);
                    currentMassiveClouds.SkyPass.ApplyTo(skyPropertyBlock);
                    skyPropertyBlock.SetVector("_SkyParam", new Vector4(intensity, 0.0f, Mathf.Cos(phi), Mathf.Sin(phi)));
                    skyPropertyBlock.SetMatrix(pixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);
                    CoreUtils.DrawFullScreen(builtinParams.commandBuffer, SkyMaterial, skyPropertyBlock, passId);
                }
                else if (passId == 1)
                {
                    if (builtinParams.hdCamera.camera.cameraType == CameraType.Reflection)
                        cmd.SetGlobalFloat("_MassiveCloudsProbeScale", 0.001f);
                    else
                        cmd.SetGlobalFloat("_MassiveCloudsProbeScale", 1f);

                    var rtDesc = CreateRenderTextureDesc(builtinParams);

                    var formatAlpha = RenderTextureFormat.DefaultHDR;
                    if (rt == null) rt = new DynamicRenderTexture(formatAlpha);
                    rt.Update(builtinParams.hdCamera.camera, rtDesc);

                    var destination = rt.GetRenderTexture(camera);
                    var massiveCloudsCtx = CreateMassiveCloudsPassContext(builtinParams, destination);

                    blitter.Blit(cmd, builtinParams.colorBuffer, destination, CalculateRtScale(builtinParams));
                    currentMassiveClouds.BuildCommandBuffer(massiveCloudsCtx, this);
                }
            }
        }

        private Vector2 CalculateRtScale(BuiltinSkyParameters builtinSkyParameters)
        {
            var colorBuffer = builtinSkyParameters.colorBuffer;
            var rtScale = builtinSkyParameters.colorBuffer.rtHandleProperties.rtHandleScale;
            if (builtinSkyParameters.hdCamera.camera.cameraType == CameraType.SceneView)
            {
                return rtScale;
            }
            if (XRSettings.enabled)
            {
                var eyeTextureW = XRSettings.eyeTextureDesc.width;
                var eyeTextureH = XRSettings.eyeTextureDesc.height;
                var hScale = (float)eyeTextureW / colorBuffer.rt.width;
                var vScale = (float)eyeTextureH / colorBuffer.rt.height;
                return new Vector2(hScale, vScale);
            }
            else
            {
                return rtScale;
            }
        }

        private RenderTextureDesc CreateRenderTextureDesc(BuiltinSkyParameters builtinSkyParameters)
        {
            var colorBuffer = builtinSkyParameters.colorBuffer;

            if (builtinSkyParameters.hdCamera.camera.cameraType == CameraType.SceneView)
            {
                return new RenderTextureDesc(
                    builtinSkyParameters.hdCamera.camera.pixelWidth,
                    builtinSkyParameters.hdCamera.camera.pixelHeight);
            }

            if (XRSettings.enabled)
            {
                var w = XRSettings.eyeTextureDesc.width;
                var h = XRSettings.eyeTextureDesc.height;
                return new RenderTextureDesc(w, h);
            }

            return new RenderTextureDesc(colorBuffer.rt.width, colorBuffer.rt.height, CalculateRtScale(builtinSkyParameters), VRTextureUsage.None);
        }

        private MassiveCloudsPassContext CreateMassiveCloudsPassContext(
            BuiltinSkyParameters builtinSkyParameters,
            RenderTexture source)
        {
            var cmd = builtinSkyParameters.commandBuffer;
            var camera = builtinSkyParameters.hdCamera.camera;
            var colorBuffer = builtinSkyParameters.colorBuffer;
            var rtScale = builtinSkyParameters.colorBuffer.rtHandleProperties.rtHandleScale;

            if (camera.cameraType == CameraType.SceneView)
            {
                return new MassiveCloudsPassContext(
                    cmd,
                    camera,
                    new RenderTextureDesc(camera.pixelWidth, camera.pixelHeight),
                    source);
            }
            if (XRSettings.enabled)
            {
                var w = XRSettings.eyeTextureDesc.width;
                var h = XRSettings.eyeTextureDesc.height;
                return new MassiveCloudsPassContext(
                    cmd,
                    camera,
                    new RenderTextureDesc(w, h),
                    source);
            }
            else
            {
                return new MassiveCloudsPassContext(
                    cmd,
                    camera,
                    new RenderTextureDesc(colorBuffer.rt.width, colorBuffer.rt.height,
                        CalculateRtScale(builtinSkyParameters), VRTextureUsage.None),
                    source);
            }
        }

        public void Draw(CommandBuffer commandBuffer, RenderTargetIdentifier source)
        {
            fullScreenBlitter.Blit(commandBuffer, source, currentBuiltinSkyParameters.colorBuffer, currentBuiltinSkyParameters.depthBuffer);
        }
    }
}