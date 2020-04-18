using exiii.Library.IO;
using exiii.Unity.Rx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace exiii.Unity.Connection
{
    /// <summary>
    /// Manager class to communication
    /// </summary>
    /// <remarks>
    /// At the timing after completion of FixedUpdate, call for device update processing
    /// After that, call communication processing
    /// </remarks>
    [CreateAssetMenu(fileName = "ConnectionManager", menuName = "EXOS/Connection/ConnectionManager")]
    public sealed class ConnectionManager : ScriptableObject
    {
        #region Static

        private static string AssetName { get { return nameof(ConnectionManager); } }

        private static ConnectionManager s_Instance;

        /// <summary>
        /// Reference for the singleton instans. You neede create manager instance explicit from inheritance class.
        /// </summary>
        public static ConnectionManager Instance
        {
            get
            {
                if (s_Instance != null) { return s_Instance; }

                s_Instance = Resources.Load<ConnectionManager>(AssetName);

                if (s_Instance != null)
                {
                    Log();
                    return s_Instance;
                }

                var objects = Resources.LoadAll<ConnectionManager>("");

                if (objects.Length > 0)
                {
                    s_Instance = objects[0];

                    Log();
                    return s_Instance;
                }

                LogWarning();
                return null;
            }
        }

        /// <summary>
        /// Instanse is exist or not.
        /// </summary>
        public static bool IsExist
        {
            get { return Instance != null; }
        }

        #region Log

        private static bool s_WarningDone = false;

        private static void Log()
        {
            Debug.Log($"[EXOS_SDK] {AssetName} : Instance is set", s_Instance);
        }

        private static void LogWarning()
        {
            if (s_WarningDone) { return; }

            Debug.LogWarning($"[EXOS_SDK] {AssetName} : Instance is not found.");

            s_WarningDone = true;
        }

        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RunOnStart()
        {
            MainThreadDispatcher.Initialize();

            if (IsExist) { Instance.Initialize(); }
        }

        #endregion

        #region Inspector

        [Header("ConnectionManager")]
        [SerializeField, Unchangeable]
        private bool m_IsActive = false;

        /// <summary>
        /// Manager is activated or not.
        /// </summary>
        public bool IsActive
        {
            get { return m_IsActive; }
            //set { m_IsActive = value; }
        }

        [SerializeField, UnchangeableInPlaying]
        private bool m_IsEnabled = true;

        /// <summary>
        /// The manager will work or not.
        /// </summary>
        public bool IsEnabled { get { return m_IsEnabled; } }

        /// <summary>
        /// Uppdate timing of connection
        /// </summary>
        [SerializeField]
        private FrameCountType m_ConnectionTiming = FrameCountType.FixedUpdate;

        /// <summary>
        /// Type of OSC library to use
        /// </summary>
        [SerializeField]
        private EOscType m_OscType = EOscType.SharpOSC;


        [SerializeField]
        private int m_ControlPort = 50194;

        /// <summary>
        /// Port number used for device control communication
        /// </summary>
        public int ControlPort { get { return m_ControlPort; } }

        [SerializeField]
        private int m_DataPortStart = 50195;

        /// <summary>
        /// Start value of port number used for device data communication
        /// </summary>
        public int DataPortStart { get { return m_DataPortStart; } }

        //[Header("EventList")]

        /// <summary>
        /// Events called at timing of connection update
        /// </summary>
        public UnityEvent ConnectionUpdateEvent { get; } = new UnityEvent();

        /// <summary>
        /// Events called at timing of connection disposed
        /// </summary>
        public UnityEvent ConnectionDisposeEvent { get; } = new UnityEvent();

        /// <summary>
        /// Events called at end of frame
        /// </summary>
        public UnityEvent CleanupEvent { get; } = new UnityEvent();

        #endregion Inspector

        private Dictionary<int, ICommandPort> m_Dictionary = new Dictionary<int, ICommandPort>();

        /// <summary>
        /// The dictionary of the existing command port.
        /// </summary>
        public IReadOnlyDictionary<int, ICommandPort> Dictionary { get { return m_Dictionary; } }

        private IDisposable m_ConnectionTimer;
        private IDisposable m_CleanupTimer;

        private OscManager m_Manager;

        private void CheckName()
        {
            if (name != AssetName)
            {
                Debug.LogError($"Don't change singleton resource file name : {name} / {AssetName}", this);
            }
        }

        private void OnValidate()
        {
            CheckName();
        }

        /// <summary>
        /// Get the command port associated with the specified device ID
        /// </summary>
        /// <param name="deviceID">Target DeviceID</param>
        /// <returns>ICommandPort obtained</returns>
        public ICommandPort GetCommandPort(int deviceID)
        {
            ICommandPort port;
            if (!m_Dictionary.TryGetValue(deviceID, out port))
            {
                Debug.LogError("[EXOS_SDK] GetCommandPort : The specified connection is not found", this);
                return null;
            }

            return port;
        }

        /// <summary>
        /// Release the command port associated with the specified DeviceID
        /// </summary>
        /// <param name="deviceID">Target DeviceID</param>
        /// <returns>Whether you could dispose</returns>
        public bool DisposeCommandPort(int deviceID)
        {
            ICommandPort port;
            if (!m_Dictionary.TryGetValue(deviceID, out port))
            {
                Debug.LogError("[EXOS_SDK] DisposeCommandPort : The specified connection is not found", this);
                return false;
            }

            m_Dictionary.Remove(deviceID);
            port.Dispose();

            return true;
        }

        private IEnumerator UpdateConnection()
        {
            while (IsActive)
            {
                yield return null;

                // Debug.Log("OutsideUpdateStart".ColorTag(Color.magenta));

                try
                {
                    ConnectionUpdateEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            DisposeConnection();
        }

        private void DisposeConnection()
        {
            if (m_ConnectionTimer != null)
            {
                m_ConnectionTimer.Dispose();
                m_ConnectionTimer = null;
            }
        }

        private IEnumerator UpdateCleanup()
        {
            while (IsActive)
            {
                yield return null;

                // Debug.Log("CleanupUpdateStart".ColorTag(Color.magenta));

                try
                {
                    CleanupEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                // Debug.Log("CleanupUpdateEnd".ColorTag(Color.magenta));
            }

            DisposeCleanup();
        }

        private void DisposeCleanup()
        {
            if (m_CleanupTimer != null)
            {
                m_CleanupTimer.Dispose();
                m_CleanupTimer = null;
            }
        }

        /// <summary>
        /// Called with Initialize
        /// </summary>
        public void Initialize()
        {
            if (IsActive == true || IsEnabled == false) { return; }

            try
            {
                switch (m_OscType)
                {
                    case EOscType.None:
                        break;

                    case EOscType.SharpOSC:
                        m_Manager = SharpOscManager.CreateInstance(ControlPort, DataPortStart);
                        break;
                }

                if (m_ConnectionTimer == null)
                {
                    m_ConnectionTimer = Observable
                        .FromMicroCoroutine<Unit>(_ => UpdateConnection(), m_ConnectionTiming)
                        .Subscribe(onNext => { }, onError => Debug.LogError(onError), () => Debug.LogWarning("[EXOS_SDK] OutsideTimer completed"));
                }

                if (m_CleanupTimer == null)
                {
                    m_CleanupTimer = Observable
                        .FromMicroCoroutine<Unit>(_ => UpdateCleanup(), FrameCountType.EndOfFrame)
                        .Subscribe(onNext => { }, onError => Debug.LogError(onError), () => Debug.LogWarning("[EXOS_SDK] CleanupTimer completed"));
                }
            }
            catch
            {
                m_IsActive = false;
                throw;
            }

            Observable.OnceApplicationQuit().Subscribe(_ => Termination());

            m_IsActive = true;

            Debug.Log($"[EXOS_SDK] Initialize {AssetName}", this);
        }

        /// <summary>
        /// Called with Termination
        /// </summary>
        public void Termination()
        {
            if (m_IsActive == false) { return; }
                      
            ConnectionDisposeEvent.Invoke();

            DisposeConnection();
            DisposeCleanup();

            try
            {
                if (m_Manager != null)
                {
                    m_Manager.Dispose();
                    m_Manager = null;
                }
            }
            finally
            {
                m_IsActive = false;
            }

            Debug.Log($"[EXOS_SDK] Terminate {AssetName}", this);
        }
    }
}