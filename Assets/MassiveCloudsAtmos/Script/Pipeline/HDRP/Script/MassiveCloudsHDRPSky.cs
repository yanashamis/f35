using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Mewlist.MassiveClouds
{
    [SkyUniqueID(MASSIVE_CLOUDS_SKY_UNIQUE_ID)]
    public class MassiveCloudsHDRPSky : SkySettings
    {
        public const int MASSIVE_CLOUDS_SKY_UNIQUE_ID = 1095834;
        public MassiveCloudsVolumeParameter MassiveClouds = new MassiveCloudsVolumeParameter(null);
        public BoolParameter SkyOnly = new BoolParameter(false);
        public FloatParameter AmbientLightMultiplier = new FloatParameter(1f);

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            unchecked
            {
                hash = MassiveClouds.value != null ? hash * 23 + MassiveClouds.GetHashCode() : hash;
            }
            return hash;
        }
        public override Type GetSkyRendererType() { return typeof(MassiveCloudsSkyRenderer); }
    }
}