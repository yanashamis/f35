using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsHDRPBlitter
    {
        private static readonly int PassId = 0;
        private static readonly int SingleTexturePassId = 8;
        private static readonly string ShaderName = "Hidden/MassiveCloudsAtmos/HDRP/Blit";

        private static readonly int blitTextureNameId = Shader.PropertyToID("_BlitTexture");
        private static readonly int blitSingleTextureNameId = Shader.PropertyToID("_MCFullScreenBlitSourceTexture");
        private static readonly int blitScaleBiasNameId = Shader.PropertyToID("_BlitScaleBias");

        private static Material textureArrayBlitMaterial;

        private readonly MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        private static Material TextureArrayBlitMaterial
        {
            get
            {
                if (!textureArrayBlitMaterial) textureArrayBlitMaterial = new Material(Shader.Find(ShaderName));
                return textureArrayBlitMaterial;
            }
        }

        public static void SetMSAAEnabled(bool value)
        {
            if (value)
                TextureArrayBlitMaterial.EnableKeyword("MC_MSAA_ENABLED");
            else
                TextureArrayBlitMaterial.DisableKeyword("MC_MSAA_ENABLED");
        }

        public void Blit(CommandBuffer commandBuffer, RTHandle source, RenderTexture destination, Vector2 rtScale)
        {
            var rtScaleSource = rtScale;
            var blitScale = new Vector4(rtScaleSource.x, rtScaleSource.y, 0.0f, 0.0f);

            propertyBlock.SetTexture(blitTextureNameId, source);
            propertyBlock.SetVector(blitScaleBiasNameId, blitScale);

            commandBuffer.SetRenderTarget(destination);
            commandBuffer.DrawProcedural(
                Matrix4x4.identity,
                TextureArrayBlitMaterial,
                PassId,
                MeshTopology.Triangles,
                3,
                1,
                propertyBlock);
        }

        public void Blit(CommandBuffer commandBuffer, RTHandle source, RenderTexture destination)
        {
            Blit(commandBuffer, source, destination, source.rtHandleProperties.rtHandleScale);
        }

        public void Blit(CommandBuffer commandBuffer, RenderTargetIdentifier source, RTHandle destination, RTHandle depthDestination)
        {
            commandBuffer.SetRenderTarget(destination, depthDestination);
            Blit(commandBuffer, source);
        }

        public void Blit(CommandBuffer commandBuffer, RenderTargetIdentifier source)
        {
            var blitScale = new Vector4(1, 1, 0.0f, 0.0f);
            propertyBlock.SetVector(blitScaleBiasNameId, blitScale);
            commandBuffer.SetGlobalTexture(blitSingleTextureNameId, source);
            commandBuffer.DrawProcedural(
                Matrix4x4.identity,
                TextureArrayBlitMaterial,
                SingleTexturePassId,
                MeshTopology.Triangles,
                3,
                1,
                propertyBlock);
        }
    }
}