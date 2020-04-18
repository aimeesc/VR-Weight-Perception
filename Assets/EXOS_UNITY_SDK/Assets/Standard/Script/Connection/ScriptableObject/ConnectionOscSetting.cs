using exiii.Async;
using exiii.Library.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Connection
{
    /// <summary>
    /// ScriptableObject for holding set values for Osc communication
    /// </summary>
    [CreateAssetMenu(fileName = "ConnectionOscSetting", menuName = "EXOS/Connection/OscSettings")]
    public class ConnectionOscSetting : ConnectionSettingBase
    {
        #region Inspector

        [Header(nameof(ConnectionOscSetting))]

        /// <summary>
        /// IPAddress of Host
        /// </summary>
        /// <remarks>
        /// If the given IP address is invalid, estimate that from Device IPAddress.
        /// </remarks>
        [SerializeField]
        [FormerlySerializedAs("HostIPAddress")]
        public string m_HostIPAddress = "";

        [SerializeField]
        public bool m_AutoDetection = true;

        /// <summary>
        /// IPAddress of Device
        /// </summary>
        [SerializeField]
        public List<string> m_HostIPAddressList;

        [SerializeField, Unchangeable]
        public string m_HostIPAddressDetected = "";

        /// <summary>
        /// IPAddress of Device
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("DeviceIPAddress")]
        public string m_DeviceIPAddress = "192.168.X.X";

        /// <summary>
        /// Subnetmask of Device IPAddress
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("DeviceSubnetMask")]
        public string m_DeviceSubnetMask = "255.255.255.0";

        /// <summary>
        /// Subnetmask of Device IPAddress
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("DeviceControlPort")]
        public int m_DeviceControlPort = 50194;

#pragma warning disable 0414
        [SerializeField, Unchangeable]
        private int m_LocalDataPort = 0;
#pragma warning restore 0414

        #endregion Inspector

        /// <summary>
        /// Connection type of this setting
        /// </summary>
        public override EConnectionType ConnectionType
        {
            get { return EConnectionType.Osc; }
        }

        private bool IsValid
        {
            get { return m_HostIPAddressDataList != null && m_HostIPAddressDataList.Length > 0 && m_DeviceIPAddressData != null; }
        }

        private IPAddress[] m_HostIPAddressDataList;
        private IPAddress m_DeviceIPAddressData;
        private IPAddress m_DeviceSubnetMaskData;

        /// <summary>
        /// Set up CommandPort according to the specified parameters
        /// </summary>
        /// <param name="deviceID">Target device ID</param>
        /// <returns>ICommandPort obtained</returns>
        protected override async Task<ICommandPort> OpenCommandPortAsync(byte deviceID)
        {
            await new SynchronizationContextRemover();

            if (!IsValid)
            {
                Retry = false;

                if (DebugLog) { Debug.LogWarning($"Setup command port failed (IPAddress is not valid) : {ExName}", this); }
                return null;
            }

            foreach(var HostIPAddressData in m_HostIPAddressDataList)
            {
                if (OscManager.Instance == null) { break; }

                try
                {
                    var osc = await OscManager.Instance.OscOpenAsync(HostIPAddressData, m_DeviceIPAddressData, m_DeviceControlPort, ControlTimeout, DataTimeout);

                    if (osc == null)
                    {
                        if (DebugLog) { Debug.LogWarning($"Setup command port failed : {ExName}/{HostIPAddressData.ToString()}",this); }
                        continue;
                    }

                    m_LocalDataPort = osc.LocalPort;
                    m_HostIPAddressDetected = HostIPAddressData.ToString();

                    if (DebugLog) { Debug.LogFormat($"Setup command port success : {ExName}/{HostIPAddressData.ToString()}", ExName, this); }
                    return osc;
                }
                catch (TimeoutException e)
                {
                    if (DebugLog) { Debug.LogWarning($"[Exception] Setup command port failed : {ExName}/{HostIPAddressData.ToString()} : {e}", this); }
                    continue;
                }
            }

            if (DebugLog) { Debug.LogWarning($"Setup command port failed All : {ExName}", this); }
            return null;
        }

        /// <summary>
        /// Try to close CommandPort
        /// </summary>
        /// <param name="port">Target command port</param>
        /// <param name="forceClose">Force closing when to fail to close-sequence</param>
        /// <returns>Whether you could close the target</returns>
        protected override async Task<bool> CloseCommandPortAsync(ICommandPort port, bool forceClose = false)
        {
            await new SynchronizationContextRemover();

            if (OscManager.Instance == null) { return false; }

            try
            {
                return await OscManager.Instance.OscCloseAsync(port, ControlTimeout, forceClose);
            }
            catch (TimeoutException e)
            {
                if (DebugLog) { Debug.LogWarning($"[Exception] Close command port failed : {ExName} : {e}", this); }
                return false;
            }  
            catch (NullReferenceException e)
            {
                if (DebugLog) { Debug.LogWarning($"[Exception] Close command port failed : {ExName} : {e}", this); }
                return false;
            }
        }

        /// <summary>
        /// Initialize connection
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            bool isValid = true;

            if (!IPAddress.TryParse(m_DeviceIPAddress, out m_DeviceIPAddressData))
            {
                isValid = false;
                Debug.LogWarning("DeviceIPAddress is invalid : {ExName}", this);
            }

            if (!IPAddress.TryParse(m_DeviceSubnetMask, out m_DeviceSubnetMaskData))
            {
                isValid = false;
                Debug.LogWarning("DeviceSubnetMask is invalid : {ExName}", this);
            }

            if (m_AutoDetection && isValid)
            {
                m_HostIPAddressDataList = Tools.GetIPAddressOfHost(m_DeviceIPAddressData, m_DeviceSubnetMaskData);
            }
            else
            {
                m_HostIPAddressDataList = new IPAddress[] { IPAddress.Parse(m_HostIPAddress) };
            }

            if (m_HostIPAddressDataList != null)
            {
                m_HostIPAddressList = m_HostIPAddressDataList.Select(x => x.ToString()).ToList();
            }
        }

        /// <summary>
        /// Terminate connection
        /// </summary>
        public override void Termination()
        {
            base.Termination();

            m_HostIPAddressList.Clear();

            m_LocalDataPort = 0;
        }
    }
}