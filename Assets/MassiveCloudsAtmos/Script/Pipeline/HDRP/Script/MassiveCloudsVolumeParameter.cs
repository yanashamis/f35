using System;
using System.Diagnostics;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
    public class MassiveCloudsVolumeParameter : VolumeParameter<MassiveCloudsPhysicsCloud>
    {
        public MassiveCloudsVolumeParameter(MassiveCloudsPhysicsCloud value, bool overrideState = false)
            : base(value, overrideState) {}
    }
}