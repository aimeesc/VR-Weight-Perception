using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace exiii.Unity.SteamVR
{
    [InitializeOnLoad]
    public class ToggleVR : EditorWindow
    {
        const float width = 80.0f;        

        static ToggleVR instance;
        static bool enabled;

        static bool dashboardFlag;
        static bool buff_dashboardFlag;

        [MenuItem("Window/Exos/ToggleVR")]
        static void ChangeShowWindow()
        {
            if (instance == null)
            {
                instance = CreateInstance<ToggleVR>();
            }

            instance.titleContent = new GUIContent("ToggleVR");
            instance.ShowUtility();
        }

        static ToggleVR()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!enabled) return;

            //Debug.Log("ToggleVR : OnPlayModeStateChanged");

            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    //Debug.Log("ToggleVR : EnteredPlayMode");

                    bool value;

                    if (ViveDashboard.TryGetEnabled(out value))
                    {
                        buff_dashboardFlag = value;
                    }

                    ViveDashboard.TrySetEnabled(dashboardFlag);
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    //Debug.Log("ToggleVR : ExitingPlayMode");

                    ViveDashboard.TrySetEnabled(buff_dashboardFlag);
                    break;
            }
        }



        GUIStyle guiStyle = new GUIStyle();
        GUIStyleState styleState = new GUIStyleState();
        List<GUILayoutOption> options = new List<GUILayoutOption>();

        void Awake()
        {
            //Debug.Log("ToggleVR : Awake");

            //styleState.textColor = Color.white;
            //styleState.background = Texture2D.whiteTexture;

            guiStyle.normal = styleState;

            options.Add(GUILayout.Width(width));
        }

        void OnEnable()
        {
            //Debug.Log("ToggleVR : OnEnable");

            enabled = true;

            bool value;

            if (ViveDashboard.TryGetEnabled(out value))
            {
                buff_dashboardFlag = value;
            }
        }

        void OnDisable()
        {
            if (Application.isPlaying)
            {
                //Debug.Log("ToggleVR : OnDisable in PlayMode");

                ViveDashboard.TrySetEnabled(buff_dashboardFlag);
            }
            else
            {
                //Debug.Log("ToggleVR : OnDisable");
            }

            enabled = false;
        }

        void OnDestroy()
        {
            //Debug.Log("ToggleVR : OnDestroy");
        }

        void OnGUI()
        {
            if (Application.isPlaying)
            {
                using (new EnabledScope(false))
                {
                    if (PlayerSettings.virtualRealitySupported)
                    {
                        EditorGUILayout.ToggleLeft("VR Enabled", true, guiStyle, options.ToArray());
                    }
                    else
                    {
                        EditorGUILayout.ToggleLeft("VR Disabled", false, guiStyle, options.ToArray());
                    }
                }

                var preValue = dashboardFlag;
                dashboardFlag = EditorGUILayout.ToggleLeft("VR Dashboad", dashboardFlag, guiStyle, options.ToArray());

                if (dashboardFlag != preValue)
                {
                    ViveDashboard.TrySetEnabled(dashboardFlag);
                }
            }
            else
            {
                PlayerSettings.virtualRealitySupported = EditorGUILayout.ToggleLeft("VR Support", PlayerSettings.virtualRealitySupported, guiStyle, options.ToArray());
                dashboardFlag = EditorGUILayout.ToggleLeft("VR Dashboad", dashboardFlag, guiStyle, options.ToArray());
            }
        }
    } 
}

