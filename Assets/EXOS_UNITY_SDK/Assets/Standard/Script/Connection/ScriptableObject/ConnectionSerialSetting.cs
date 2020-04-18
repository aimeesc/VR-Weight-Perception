using exiii.Async;
using exiii.Library.IO;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace exiii.Unity.Connection
{
    /// <summary>
    /// ScriptableObject for holding set values for Serial communication
    /// </summary>
    [CreateAssetMenu(fileName = "ConnectionSerialSetting", menuName = "EXOS/Connection/SerialSettings")]
    public class ConnectionSerialSetting : ConnectionSettingBase
    {
        #region Inspector

        [Header(nameof(ConnectionSerialSetting))]

        /// <summary>
        /// Connection destination when COM port is not automatically determined
        /// </summary>
        [SerializeField]
        private string m_ComPortName = "COMX";

        /// <summary>
        /// When it is true, COM port is automatically determined
        /// </summary>
        [SerializeField]
        private bool m_AutoDetection = true;

        [SerializeField, Unchangeable]
        private string m_DetectedComPortName = "COMX";

        /// <summary>
        /// BaudRate
        /// </summary>
        [SerializeField]
        private int m_BaudRate = 230400;

        [Header("Debug")]
        [SerializeField]
        private bool m_DebugMode = false;

        #endregion Inspector

        /// <summary>
        /// Connection type of this setting
        /// </summary>
        public override EConnectionType ConnectionType
        {
            get { return EConnectionType.Serial; }
        }

        /// <summary>
        /// Set up CommandPort according to the specified parameters
        /// </summary>
        /// <param name="deviceID">Target device ID</param>
        /// <returns>Whether you could close the target</returns>
        protected override async Task<ICommandPort> OpenCommandPortAsync(byte deviceID)
        {
            await new SynchronizationContextRemover();

            SerialConnection serial = null;

            try
            {
                if (m_AutoDetection)
                {
                    serial = await SerialManager.SearchSerialConnectionAsync(deviceID, m_BaudRate, DataTimeout, ControlTimeout, m_DebugMode);

                    if (serial != null) { m_DetectedComPortName = serial.PortName; }
                }
                else
                {
                    serial = await SerialConnection.GetSerialConnectionAsync(m_ComPortName, m_BaudRate, DataTimeout, ControlTimeout, m_DebugMode);
                }

                if (serial == null)
                {
                    if (DebugLog) { Debug.LogWarning($"Setup command port failed : {ExName} / {deviceID}", this); }
                    return null;
                }
                else if(serial.UsedBy.Count > 0 && !serial.UsedBy.Contains(deviceID))
                {
                    if (DebugLog) { Debug.LogWarning($"Setup command port failed : {ExName} / {deviceID}", this); }
                    return null;
                }

                if (DebugLog) { Debug.Log($"Setup command port success : {ExName} / {deviceID} / {serial.PortName}", this); }
                return serial;
            }
            catch (TimeoutException e)
            {
                if (serial != null)
                {
                    if (serial.UsedBy.Contains(deviceID))
                    {
                        serial.UsedBy.Remove(deviceID);
                    }

                    if (serial.UsedBy.Count == 0)
                    {
                        serial.Dispose();
                    }
                }

                if (DebugLog) { Debug.LogWarning($"[Exception] Setup command port failed : {ExName} : {e}", this); }
                return null;
            }
        }


#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        /// <summary>
        /// Try to close CommandPort
        /// </summary>
        /// <param name="port">Target command port</param>
        /// <param name="forceClose">Force closing when to fail to close-sequence</param>
        /// <returns>Whether you could close the target</returns>
        protected override async Task<bool> CloseCommandPortAsync(ICommandPort port, bool forceClose = false)
        {
            await new SynchronizationContextRemover();

            if (port != null) { port.Dispose(); }
            
            return true;
        }
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
    }
}