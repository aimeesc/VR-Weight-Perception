using UnityEngine;
using System;
using exiii.Unity.Rx;
using System.Collections.Generic;
using exiii.Unity.Rx.Triggers;

#if UNITY_EDITOR

using UnityEditor;

#endif

#pragma warning disable 414

namespace exiii.Unity
{
    public abstract class ExMonoBehaviour : MonoBehaviour, IExMonoBehaviour
    {
        #region Inspector

        [Header(nameof(ExMonoBehaviour))]
        [SerializeField, Unchangeable]
        private bool m_IsActive = false;

        public bool IsActive
        {
            get { return m_IsActive && enabled && gameObject.activeInHierarchy; }
        }

        [SerializeField]
        private ExTagContainer[] m_ExTags;

        public IReadOnlyCollection<IExTag> ExTags { get { return m_ExTags; } }

        //[SerializeField, Unchangeable]
        private string m_ExName;

        public string ExName => m_ExName;

        //[SerializeField, Unchangeable]
        private bool m_CalledAwake = false;

        //[SerializeField, Unchangeable]
        private bool m_CalledStart = false;

        #endregion Inspector

        private bool m_PromiseInitialize;

        private IDisposable m_DisposableEndOfFrame;

        private IObservable<string> OnNameChanged
        {
            get { return this.ObserveEveryValueChanged(x => x.name); }
        }

        public bool DrawTransformForDebug { get; set; } = false;

        protected virtual void OnValidate()
        {
            UpdateName();

#if UNITY_EDITOR

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                m_IsActive = false;
            }

#endif
        }

        protected virtual void Awake()
        {
            m_CalledAwake = true;

            UpdateName();

            OnNameChanged.Subscribe(_ => UpdateName());

            if (DrawTransformForDebug)
            {
                this.UpdateAsObservable().Subscribe(_ => DrawTransform());
            }
        }

        protected virtual void Start()
        {
            m_CalledStart = true;

            if (m_PromiseInitialize) { InvokeInitialize(); }
        }

        protected virtual void OnEnable()
        {
            if (m_CalledStart)
            {
                Initialize();
            }
            else
            {
                m_PromiseInitialize = true;
                return;
            }
        }

        protected virtual void OnDisable()
        {
            InvokeTerminate();
        }

        private void UpdateName()
        {
            m_ExName = name;
        }

        private void InvokeInitialize()
        {
            m_PromiseInitialize = false;

            if (IsActive == true) { return; }

            Initialize();

            m_IsActive = true;
        }

        private void InvokeTerminate()
        {
            Terminate();

            m_IsActive = false;
        }

        private void DrawTransform()
        {
            LineDrawerGL.DrawLine(transform.position, transform.position + transform.right * 0.01f, Color.red);
            LineDrawerGL.DrawLine(transform.position, transform.position + transform.up * 0.01f, Color.green);
            LineDrawerGL.DrawLine(transform.position, transform.position + transform.forward * 0.01f, Color.blue);
        }

        #region IExObject

        public virtual void Initialize() { }

        public virtual void Terminate() { }

        #endregion IExObject
    }
}