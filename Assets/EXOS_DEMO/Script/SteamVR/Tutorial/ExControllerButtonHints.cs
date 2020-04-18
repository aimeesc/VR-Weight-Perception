#if EHL_SteamVR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using exiii.Unity.Rx;
using UnityEngine.XR;
using exiii.Extensions;
using exiii.Unity.Rx.Triggers;

using Valve.VR;

namespace exiii.Unity.SteamVR
{
    //-------------------------------------------------------------------------
    public class ExControllerButtonHints : MonoBehaviour
    {
#region Inspector

        [SerializeField]
        private TextMesh m_Text;

        [SerializeField]
        private SteamVR_RenderModel m_RenderModel;

        [SerializeField]
        private Material[] m_DefaultMaterials;

        [SerializeField]
        private Material m_ControllerMaterial;

        [SerializeField]
        private Hint[] m_HintsForVive;

        [SerializeField]
        private Hint[] m_HintsForOculus;

        [SerializeField]
        private Hint[] m_HintsForWindowsMR;

        [SerializeField]
        private float m_LerpInterval = 1;

        [SerializeField]
        private bool m_AutoChange = false;

        [SerializeField]
        private float m_ChangeInterval = 5;

        #endregion  Inspector


        private List<MeshRenderer> m_Renderers = new List<MeshRenderer>();

        private int m_HintIndex = 0;

        private Hint[] m_Hints;

        private CompositeDisposable m_Disposables = new CompositeDisposable();

        IDisposable m_Disposable;

        private void OnEnable()
        {
            if (m_Disposable == null && m_AutoChange)
            {
                m_Disposable = Observable
                    .Interval(TimeSpan.FromSeconds(m_ChangeInterval))
                    .Subscribe(_ => ChangeDescription());
            }
        }

        private void OnDisable()
        {
            if (m_Disposable != null)
            {
                m_Disposable.Dispose();
                m_Disposable = null;
            }
        }

        private void OnDestroy()
        {
            if (m_Disposable != null)
            {
                m_Disposable.Dispose();
                m_Disposable = null;
            }
        }

        private bool CheckDevice()
        {
            if (m_RenderModel == null) { return false; }

            if (m_Hints == null)
            {
                if (m_RenderModel.renderModelName == null)
                {
                    return false;
                }
                else if (m_RenderModel.renderModelName.Contains("vive"))
                {
                    m_Hints = m_HintsForVive;
                }
                else if (m_RenderModel.renderModelName.Contains("oculus"))
                {
                    m_Hints = m_HintsForOculus;
                }
                else if (m_RenderModel.renderModelName.Contains("controller.obj"))
                {
                    m_Hints = m_HintsForWindowsMR;
                }
            }

            if (m_Hints == null || m_Hints.Length == 0) { return false; }

            return true;
        }

        private void ChangeDescription()
        {
            if (CheckDevice() == false) { return; }

            ChangeDescription(m_Hints[m_HintIndex]);

            m_HintIndex++;

            if (m_HintIndex >= m_Hints.Length) { m_HintIndex = 0; }
        }

        public void ChangeDescription(EEventType type)
        {
            if (CheckDevice() == false) { return; }

            var hint = m_Hints.Where(x => x.EventType == type).FirstOrDefault();

            ChangeDescription(hint);
        }

        private void ChangeDescription(Hint hint)
        {
            ResetColor();

            if (hint == null) { return; }

            foreach (var index in hint.Target)
            {
                ChangeColor(index, hint.Color);

                m_Text.text = hint.Description;
            }
        }

        private Color Transparent(Color color, float a)
        {
            color.a = a;
            return color;
        }

        private void ChangeColor(int index, Color color)
        {
            if (m_Renderers == null || m_Renderers.Count == 0)
            {
                m_RenderModel.GetComponentsInChildren<MeshRenderer>(m_Renderers);
            }

            if (m_DefaultMaterials == null || m_DefaultMaterials.Length == 0)
            {
                m_DefaultMaterials = m_Renderers.Select(x => x.material).ToArray();
            }

            if (m_Renderers == null || m_Renderers.Count == 0) { return; }

            index = Mathf.Clamp(index, 0, m_Renderers.Count - 1);

            Texture mainTexture = m_Renderers[index].material.mainTexture;
            m_Renderers[index].sharedMaterial = m_ControllerMaterial;
            m_Renderers[index].material.mainTexture = mainTexture;
            m_Renderers[index].material.color = color;

            // This is to poke unity into setting the correct render queue for the model
            m_Renderers[index].material.renderQueue = m_ControllerMaterial.shader.renderQueue;

            this.UpdateAsObservable()
                .Subscribe(_ => LerpColor(m_Renderers[index].material, color))
                .AddTo(m_Disposables);
        }

        public void LerpColor(Material material, Color flush)
        {
            material.color = Color.Lerp(flush, Color.black, Mathf.PingPong(Time.time, m_LerpInterval) / m_LerpInterval);
        }

        public void ResetColor()
        {
            for(int index = 0; index < m_Renderers.Count; index++)
            {
                m_Renderers[index].sharedMaterial = m_DefaultMaterials[index];
            }

            m_Disposables.Dispose();

            m_Disposables = new CompositeDisposable();
        }
    }

    [Serializable]
    class Hint
    {
        [SerializeField]
        public string Description;

        [SerializeField]
        public EEventType EventType;

        [SerializeField]
        public int[] Target;

        [SerializeField]
        public Color Color;
    }

    public enum EEventType
    {
        Grab,
        Use,
        Release,
        None,
    }
}

#endif