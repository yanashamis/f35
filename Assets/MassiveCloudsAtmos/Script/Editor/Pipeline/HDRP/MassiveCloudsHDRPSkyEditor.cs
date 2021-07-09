using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    [CanEditMultipleObjects]
    [VolumeComponentEditor(typeof(MassiveCloudsHDRPSky))]
    public class MassiveCloudsHDRPSkyEditor
        : SkySettingsEditor
    {
        public override void OnInspectorGUI()
        {
            m_CommonUIElementsMask = (uint)SkySettingsUIElement.UpdateMode 
                                     | (uint)SkySettingsUIElement.SkyIntensity 
                                     | (uint)SkySettingsUIElement.IncludeSunInBaking;
            
            EditorGUI.BeginChangeCheck();
            {
                var rendererFetcher = new PropertyFetcher<MassiveCloudsHDRPSky>(serializedObject);

                var renderer = Unpack(rendererFetcher.Find(x => x.MassiveClouds));
                var skyOnly = Unpack(rendererFetcher.Find("SkyOnly"));
                var ambientLightMultiplier = Unpack(rendererFetcher.Find("AmbientLightMultiplier"));
                PropertyField(renderer);
                PropertyField(skyOnly);
                PropertyField(ambientLightMultiplier);

                base.CommonSkySettingsGUI();
            }
            if (EditorGUI.EndChangeCheck())
            {
            }
        }
    }
}