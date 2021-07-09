using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Mewlist.MassiveClouds
{
    internal static class MassiveCloudsAtmosWizardHDRPCustomPassSetup
    {
        public static void Setup()
        {
            var volumes = Object.FindObjectsOfType<CustomPassVolume>().OrderBy(x => x.isGlobal ? 0 : 1);
            var targetVolume = (CustomPassVolume)null;
            var targetMassiveCloudsCustomPass = (MassiveCloudsHDRPCustomPass)null;
            var isCustomPassVolumeExist = false;
            var isCustomPassValid = false;
            
            foreach (var volume in volumes)
            {
                var customPasses = volume.customPasses;
                var massiveCloudsCustomPasses = customPasses.Where(x => x is MassiveCloudsHDRPCustomPass).Cast<MassiveCloudsHDRPCustomPass>();

                targetVolume = volume;
                isCustomPassVolumeExist = true;
                isCustomPassValid = isCustomPassValid || massiveCloudsCustomPasses.Any();
                if (isCustomPassValid)
                {
                    targetMassiveCloudsCustomPass = massiveCloudsCustomPasses.First();
                    break;
                }
            }

            var isInstalled = isCustomPassVolumeExist && isCustomPassValid;

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Custom Pass");

                if (!isInstalled)
                {
                    MassiveCloudsAtmosWizard.Ng("Not installed");
                    if (MassiveCloudsAtmosWizard.FixButton("Install"))
                    {
                        if (!isCustomPassVolumeExist)
                        {
                            var go = new GameObject("CustomPassVolume");
                            targetVolume = go.AddComponent<CustomPassVolume>();
                            targetVolume.isGlobal = true;
                        }

                        if (!isCustomPassValid)
                        {
                            targetVolume.AddPassOfType(typeof(MassiveCloudsHDRPCustomPass));
                        }
                    }
                }
                else
                {
                    MassiveCloudsAtmosWizard.Ok("Ok");
                    EditorGUILayout.ObjectField(targetVolume, typeof(CustomPassVolume), true);
                }
            }

            if (!isInstalled) return;

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Cloud Renderer");

                var renderer = targetMassiveCloudsCustomPass.Renderer;
                if (renderer == null)
                {
                    var color = GUI.color;
                    GUI.color = new Color(1f, 0.6f, 0.6f, 1f);
                    MassiveCloudsAtmosWizard.Ng("Please select Renderer Object.");
                    targetMassiveCloudsCustomPass.Renderer =
                        EditorGUILayout.ObjectField(renderer, typeof(AbstractMassiveClouds), false) as AbstractMassiveClouds;
                    GUI.color = color;
                }
                else
                {
                    MassiveCloudsAtmosWizard.Ok("Ok");
                    targetMassiveCloudsCustomPass.Renderer =
                        EditorGUILayout.ObjectField(renderer, typeof(AbstractMassiveClouds), false) as AbstractMassiveClouds;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Sun Light Source");

                var sunLight = targetMassiveCloudsCustomPass.Sun;
                if (sunLight)
                {
                    MassiveCloudsAtmosWizard.Ok("Ok");
                    targetMassiveCloudsCustomPass.Sun = EditorGUILayout.ObjectField(sunLight, typeof(Light), true) as Light;
                }
                else
                {
                    MassiveCloudsAtmosWizard.Ng("Please set Directional Light.");
                    if (MassiveCloudsAtmosWizard.FixButton("Fix Now"))
                    {
                        var lights = Object.FindObjectsOfType<Light>().Where(x => x.type == LightType.Directional)
                            .OrderByDescending(x => x.intensity).ToArray();
                        if (lights.Any()) targetMassiveCloudsCustomPass.Sun = lights.First();
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
                    targetVolume.customPasses.Remove(targetMassiveCloudsCustomPass);
                }
            }

            MassiveCloudsEditorGUI.SectionSpace();
        }
    }
}