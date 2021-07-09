using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Mewlist.MassiveClouds
{
    internal static class MassiveCloudsAtmosWizardHDRPSkySetup
    {
        public static void Setup()
        {
            var volumes = Object.FindObjectsOfType<Volume>();
            var targetVolume = (Volume)null;
            var targetProfile = (VolumeProfile)null;
            var targetVisualEnvironment = (VisualEnvironment)null;
            var targetMassiveCloudsSky = (MassiveCloudsHDRPSky)null;
            var isVisualEnvironmentExist = false;
            var isVisualEnvironmentValid = false;
            var isMassiveCloudsSkyExist = false;
            
            foreach (var volume in volumes)
            {
                var components = volume.sharedProfile.components;
                var visualEnvironments = components.Where(x => x is VisualEnvironment).Cast<VisualEnvironment>();
                var massiveCloudsSkyComponents = components.Where(x => x is MassiveCloudsHDRPSky).Cast<MassiveCloudsHDRPSky>();

                isVisualEnvironmentExist = isVisualEnvironmentExist || visualEnvironments.Any();
                isMassiveCloudsSkyExist = isMassiveCloudsSkyExist || massiveCloudsSkyComponents.Any();
                foreach (var visualEnvironment in visualEnvironments)
                {
                    targetVolume = volume;
                    targetProfile = volume.sharedProfile;
                    targetVisualEnvironment = visualEnvironment;

                    isVisualEnvironmentValid = visualEnvironment.skyType == MassiveCloudsHDRPSky.MASSIVE_CLOUDS_SKY_UNIQUE_ID;
                    if (isVisualEnvironmentValid && massiveCloudsSkyComponents.Any())
                    {
                        targetMassiveCloudsSky = massiveCloudsSkyComponents.First();
                        break;
                    }
                }
                if (isVisualEnvironmentValid) break;
            }

            EditorGUILayout.HelpBox("HDRP Sky only supports PhysicsCloudRenderer.", MessageType.None);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("VisualEnvironment");

                if (!isVisualEnvironmentExist)
                {
                    MassiveCloudsAtmosWizard.Ng(
                        "Please create Volume and set VisualEnvironment component in your scene.");
                }
                else if (!isVisualEnvironmentValid || !isMassiveCloudsSkyExist)
                {
                    MassiveCloudsAtmosWizard.Ng("Sky Type is not Massive Clouds Sky");
                    if (MassiveCloudsAtmosWizard.FixButton("Install"))
                    {
                        if (!isVisualEnvironmentValid)
                        {
                            targetVisualEnvironment.skyType.value = MassiveCloudsHDRPSky.MASSIVE_CLOUDS_SKY_UNIQUE_ID;
                            EditorUtility.SetDirty(targetProfile);
                        }

                        if (!isMassiveCloudsSkyExist)
                        {
                            targetMassiveCloudsSky = ScriptableObject.CreateInstance<MassiveCloudsHDRPSky>();
                            AssetDatabase.AddObjectToAsset(targetMassiveCloudsSky, targetProfile);
                            targetProfile.components.Add(targetMassiveCloudsSky);
                            EditorUtility.SetDirty(targetProfile);
                        }
                    }
                }
                else
                {
                    MassiveCloudsAtmosWizard.Ok("Ok");
                    EditorGUILayout.ObjectField(targetVolume, typeof(Volume), true);
                }
            }

            var isInstalled = isVisualEnvironmentExist && isVisualEnvironmentValid && isMassiveCloudsSkyExist;
            if (isInstalled)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("MassiveCloudsSky Component");

                    if (!targetMassiveCloudsSky.active)
                    {
                        MassiveCloudsAtmosWizard.Ng("Inactive");
                        if (MassiveCloudsAtmosWizard.FixButton("Fix now"))
                        {
                            targetMassiveCloudsSky.active = true;
                        }
                    }
                    else
                    {
                        MassiveCloudsAtmosWizard.Ok("Ok");
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Cloud Renderer");

                    var renderer = targetMassiveCloudsSky.MassiveClouds.value;
                    if (renderer == null)
                    {
                        var color = GUI.color;
                        GUI.color = new Color(1f, 0.6f, 0.6f, 1f);
                        MassiveCloudsAtmosWizard.Ng("Please select Renderer Object.");
                        targetMassiveCloudsSky.MassiveClouds.value =
                            EditorGUILayout.ObjectField(renderer, typeof(MassiveCloudsPhysicsCloud), false) as
                                MassiveCloudsPhysicsCloud;
                        if (targetMassiveCloudsSky.MassiveClouds.value != null)
                        {
                            targetMassiveCloudsSky.MassiveClouds.overrideState = true;
                            EditorUtility.SetDirty(targetProfile);
                        }

                        GUI.color = color;
                    }
                    else if (targetMassiveCloudsSky.MassiveClouds.overrideState == false)
                    {
                        MassiveCloudsAtmosWizard.Ng("Override State disabled");
                        if (MassiveCloudsAtmosWizard.FixButton("Fix now"))
                        {
                            targetMassiveCloudsSky.MassiveClouds.overrideState = true;
                            EditorUtility.SetDirty(targetProfile);
                        }
                    }
                    else
                    {
                        MassiveCloudsAtmosWizard.Ok("Ok");
                        targetMassiveCloudsSky.MassiveClouds.value =
                            EditorGUILayout.ObjectField(renderer, typeof(MassiveCloudsPhysicsCloud), false) as
                                MassiveCloudsPhysicsCloud;
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Ambient Mode");

                    if (targetVisualEnvironment != null)
                    {
                        if (targetVisualEnvironment.skyAmbientMode.value == SkyAmbientMode.Static ||
                            targetVisualEnvironment.skyAmbientMode.overrideState == false)
                        {
                            MassiveCloudsAtmosWizard.Warn("Static");
                            if (MassiveCloudsAtmosWizard.FixButton("Fix now"))
                            {
                                targetVisualEnvironment.skyAmbientMode.overrideState = true;
                                targetVisualEnvironment.skyAmbientMode.value = SkyAmbientMode.Dynamic;
                                EditorUtility.SetDirty(targetVisualEnvironment);
                            }
                        }
                        else
                        {
                            MassiveCloudsAtmosWizard.Ok("Dynamic");
                        }
                    }
                }

                MassiveCloudsEditorGUI.SectionSpace();
                MassiveCloudsEditorGUI.Header("Uninstall", 2);
                MassiveCloudsEditorGUI.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("");
                    if (GUILayout.Button("Uninstall"))
                    {
                        targetVisualEnvironment.skyType.value = 1;
                        AssetDatabase.RemoveObjectFromAsset(targetMassiveCloudsSky);
                        targetProfile.components.Remove(targetMassiveCloudsSky);
                        EditorUtility.SetDirty(targetProfile);
                    }
                }

                MassiveCloudsEditorGUI.SectionSpace();
            }
        }
    }
}