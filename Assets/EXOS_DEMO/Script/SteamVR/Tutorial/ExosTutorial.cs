#if EHL_SteamVR

using exiii.Extensions;
using exiii.Unity.Rx;
using System.Linq;
using System;
using UnityEngine;
using exiii.Unity.Device;
using exiii.Unity.Rx.Triggers;
using UnityEngine.SceneManagement;

namespace exiii.Unity.SteamVR
{
    public class ExosTutorial : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private MeshRenderer m_ScreenMeshRenderer;

        [SerializeField, Unchangeable]
        private Material m_ScreenMaterial;

        [SerializeField]
        private float m_SceneInterval = 2;

        [SerializeField]
        private int m_NextSceneIndex = 1;

        [SerializeField]
        private ExControllerButtonHints[] m_ExControllerButtonHints;

        [SerializeField]
        private InitializeSetting[] m_Settings;

        #endregion

        private CompositeDisposable m_Disposables = new CompositeDisposable();

        public void Start()
        {
            m_ScreenMaterial = m_ScreenMeshRenderer.material.Copy();
            m_ScreenMeshRenderer.material = m_ScreenMaterial;

            foreach (var setting in m_Settings)
            {
                setting.RootObjectToEnable.SetActive(true);

                ChangeDisplayEnable(setting.RootObjectToDisable);

                if (setting.ExosDevice.IsConnected)
                {
                    DeviceConnected(setting);
                }
                else
                {
                    setting.Disposable = Observable
                        .Interval(TimeSpan.FromSeconds(1.0))
                        .Where(_ => setting.ExosDevice.IsConnected)
                        .Subscribe(_ => DeviceConnected(setting));
                }
            }
        }

        private void OnDestroy()
        {
            if (m_Disposables != null)
            {
                m_Disposables.Dispose();
                m_Disposables = null;
            }
        }

        private void ChangeDisplayEnable(GameObject obj)
        {
            var prefabs = obj.GetComponentsInChildren<ExosPrefab>();

            var display = prefabs.Where(x => x.PrefabType == EPrefabType.DisplayModel).FirstOrDefault();

            if (display == null) { return; }

            display.gameObject.SetActive(false);
        }

        private void CheckDispose(InitializeSetting setting)
        {
            if (setting.Disposable != null)
            {
                setting.Disposable.Dispose();
                setting.Disposable = null;
            }
        }

        private void DeviceConnected(InitializeSetting setting)
        {
            CheckDispose(setting);

            if (setting.RenderModel.GetComponentsInChildren<MeshRenderer>().Count() > 0)
            {
                ControllerConnected(setting);
            }
            else
            {
                setting.Disposable = Observable
                    .Interval(TimeSpan.FromSeconds(1.0))
                    .Where(_ => setting.RenderModel.GetComponentsInChildren<MeshRenderer>().Count() > 0)
                    .Subscribe(_ => ControllerConnected(setting))
                    .AddTo(setting.RenderModel);
            }
        }

        private void ControllerConnected(InitializeSetting setting)
        {
            CheckDispose(setting);

            OnStart();
        }

        private bool m_Started = false;

        public void OnStart()
        {
            m_Started = true;

            ResetStatus();
        }

        private void ResetStatus()
        {
            m_Grabed = false;
            m_Used = false;

            m_ExControllerButtonHints.Foreach(x => x.ChangeDescription(EEventType.Grab));
        }

        private bool m_Grabed = false;

        public void OnGrab()
        {
            if (m_Started == false) { return; }

            if (m_Grabed) { return; }

            m_Grabed = true;

            m_ExControllerButtonHints.Foreach(x => x.ChangeDescription(EEventType.Use));
        }

        private bool m_Used = false;

        public void OnUse()
        {
            if (m_Started == false) { return; }

            if (m_Grabed == false || m_Used) { return; }

            m_Used = true;

            m_ExControllerButtonHints.Foreach(x => x.ChangeDescription(EEventType.Release));
        }

        public void OnRelease()
        {
            if (m_Started == false) { return; }

            if (m_Used == false)
            {
                ResetStatus();
                return;
            }

            m_ExControllerButtonHints.Foreach(x => x.ChangeDescription(EEventType.None));

            StartTime = Time.time;
            StartColor = m_ScreenMaterial.color;

            this.UpdateAsObservable()
                .Subscribe(_ => LerpScreen())
                .AddTo(m_Disposables);

            Observable
                .Timer(TimeSpan.FromSeconds(m_SceneInterval))
                .Subscribe(_ => NextScene())
                .AddTo(m_Disposables);
        }

        private float StartTime;
        private Color StartColor;

        private void LerpScreen()
        {
            m_ScreenMaterial.color = Color.Lerp(StartColor, Color.black, (Time.time - StartTime) / m_SceneInterval);
        }

        private void NextScene()
        {
            if (SceneManager.sceneCountInBuildSettings > m_NextSceneIndex)
            {
                SceneManager.LoadScene(m_NextSceneIndex);
            }
            else
            {
                EHLDebug.LogWarning("Scene not set", this, "", ELogLevel.Overview);
            }
        }
    }

    [Serializable]
    class InitializeSetting
    {
        [SerializeField]
        public ExosDevice ExosDevice;

        [SerializeField]
        public GameObject RenderModel;

        [SerializeField]
        public GameObject RootObjectToEnable;

        [SerializeField]
        public GameObject RootObjectToDisable;

        public IDisposable Disposable;
    }
}

#endif