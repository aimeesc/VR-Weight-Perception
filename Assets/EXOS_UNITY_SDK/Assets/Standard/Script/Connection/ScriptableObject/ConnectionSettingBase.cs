using exiii.Async;
using exiii.Library.IO;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Connection
{
    /// <summary>
    /// Superclass of ScriptableObject for setting for communication
    /// </summary>
    public abstract class ConnectionSettingBase : ScriptableObject
    {
        #region ExScriptableObject

        [Header(nameof(ScriptableObject))]
        [SerializeField, Unchangeable]
        private string m_ExName;

        /// <summary>
        /// Name of asset
        /// </summary>
        public string ExName => m_ExName;

        [SerializeField, Unchangeable]
        private bool m_IsActive;

        /// <summary>
        /// Device is active or not
        /// </summary>
        public bool IsActive => m_IsActive;

        /// <summary>
        /// Chack validation of values
        /// </summary>
        protected virtual void OnValidate()
        {
            UpdateName();
        }

        /// <summary>
        /// awake
        /// </summary>
        protected virtual void Awake()
        {
            UpdateName();
        }

        /// <summary>
        /// Initialize and activate connection
        /// </summary>
        public virtual void Initialize()
        {
            m_IsActive = true;
        }

        /// <summary>
        /// Terminat and deactivate connection
        /// </summary>
        public virtual void Termination()
        {
            m_IsActive = false;
        }

        [ContextMenu("UpdateName")]
        public void UpdateName()
        {
            m_ExName = name;
        }

        #endregion ExScriptableObject

        #region Inspector

        [Header(nameof(ConnectionSettingBase))]

        [SerializeField]
        private bool m_ConnectionEnabled = true;

        /// <summary>
        /// TimeOut[ms] This time is used for both read and write
        /// </summary>
        [SerializeField]        
        private int m_ControlTimeout = 2000;

        public int ControlTimeout => m_ControlTimeout;

        /// <summary>
        /// TimeOut[ms] This time is used for both read and write
        /// </summary>
        [SerializeField]
        private int m_DataTimeout = 500;

        public int DataTimeout => m_DataTimeout;

        /// <summary>
        /// If connection fails, attempt to reconnect until the specified number of times
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("RetryNumber")]
        private int m_RetryNumber = 3;


        [Header("Debug")]
        [SerializeField]
        private bool m_DebugLog = false;

        protected bool DebugLog { get { return m_DebugLog; } }

        #endregion Inspector

        /// <summary>
        /// Connection type of this setting
        /// </summary>
        public abstract EConnectionType ConnectionType { get; }

        protected bool Retry { get; set; } = true;

        private AsyncSubject<bool> m_Getting;

        private void CompleteGetting(bool result)
        {
            if (m_Getting == null) { return; }

            var getting = m_Getting;

            m_Getting = null;

            getting.OnNext(result);
            getting.OnCompleted();
        }

        /// <summary>
        /// Acquire the command port according to the specified parameters
        /// </summary>
        /// <param name="deviceID">Target device ID</param>
        /// <returns></returns>
        public virtual async Task<ICommandPort> GetCommandPortAsync(byte deviceID)
        {
            await new SynchronizationContextRemover();

            if (!m_ConnectionEnabled) { return null; }

            while(m_Getting != null)
            {
                await m_Getting;
            }

            bool result = false;

            try
            {
                m_Getting = new AsyncSubject<bool>();

                ConnectionManager manager;

                if (ConnectionManager.IsExist)
                {
                    manager = ConnectionManager.Instance;
                }
                else
                {
                    return null;
                }

                if (manager.Dictionary.ContainsKey(deviceID))
                {
                    if (DebugLog) { Debug.LogWarning("The specified connection already existed", this); }

                    result = true;
                    return manager.GetCommandPort(deviceID);
                }

                Retry = true;

                for (int i = 0; i < m_RetryNumber && Retry; i++)
                {
                    var port = await OpenCommandPortAsync(deviceID);

                    if (port != null)
                    {
                        port.UsedBy.Add(deviceID);
                        port.Timeout = DataTimeout;

                        port.Disposed.Subscribe(async _ => await DisposeCommandPortAsync(port));

                        Debug.Log($"Setup command port success : {ExName}", this);

                        result = true;
                        return port;
                    }

                    if (DebugLog) { Debug.Log($"Setup command port failed on {i + 1} time(s) : Retry = {Retry}", this); }

                    await Task.Delay(10);
                }
            }
            catch
            {
                Debug.LogWarning($"GetCommandPort aborted : {ExName}", this);

                throw;
            }
            finally
            {
                CompleteGetting(result);                
            }

            return null;
        }

        private AsyncSubject<bool> m_Termination;

        private bool CompletemTermination(bool result)
        {
            if (m_Termination == null) { return result; }

            var termination = m_Termination;

            m_Termination = null;

            termination.OnNext(result);
            termination.OnCompleted();

            return result;
        }

        public virtual async Task<bool> DisposeCommandPortAsync(ICommandPort port)
        {
            await new SynchronizationContextRemover();

            if (m_Termination != null)
            {
                return await m_Termination;
            }

            m_Termination = new AsyncSubject<bool>();

            for (int i = 0; i < m_RetryNumber; i++)
            {
                if (DebugLog) { Debug.LogFormat(this, "Try to close command port : " + i); }

                if (await CloseCommandPortAsync(port, i == (m_RetryNumber - 1)))
                {
                    Debug.LogFormat(this, $"Close command port success : {ExName}");

                    CompletemTermination(true);
                    return CompletemTermination(true);
                }
            }

            
            return CompletemTermination(false);
        }

        /// <summary>
        /// Set up CommandPort according to the specified parameters
        /// </summary>
        /// <param name="deviceID">Target device ID</param>
        /// <returns></returns>
        protected abstract Task<ICommandPort> OpenCommandPortAsync(byte deviceID);

        protected abstract Task<bool> CloseCommandPortAsync(ICommandPort port, bool forceClose = false);
    }
}