using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsPipelineDependent
    {
        public static void SetLightIntensity(Light light, float intensity, Color lightColor, float temperature)
        {
            var hdLight = light.GetComponent<HDAdditionalLightData>();
            light.colorTemperature = temperature;
            light.color = lightColor;
            light.intensity = intensity;
            hdLight.color = lightColor;
            hdLight.SetIntensity(intensity);
        }
    }
}