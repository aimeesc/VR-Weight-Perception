using exiii.Collections;
using exiii.Extensions;
using exiii.Library.IO;
using exiii.Unity.Connection;
using exiii.Unity.Rx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

//using UnityEditor;
using exiii.Async;
using System.Threading;

namespace exiii.Unity.Device
{
    /// <summary>
    /// A class wrapped for device management and communication management in DLL for Unity
    /// </summary>
    [CreateAssetMenu(fileName = "ExosDevice", menuName = "EXOS/Device/DeviceSettings")]
    public sealed class ExosDevice : ScriptableObject
    {
        private const int DefaultTimeout = 3000;
        private const int DelayRate = 10;

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

        public bool IsActive => m_IsActive;

        /// <summary>
        /// Awake
        /// </summary>
        public void Awake()
        {
            UpdateName();
        }       

        /// <summary>
        /// Update
        /// </summary>
        [ContextMenu("UpdateName")]
        public void UpdateName()
        {
            m_ExName = name;
        }

        #endregion ExScriptableObject

        private static float ForceMax
        {
            get { return CommandBoard.TorqueMax; }
        }

        private static float AnarogeSensorMax
        {
            get { return CommandBoard.AnalogueMax; }
        }

        #region Inspector

        [Header(nameof(ExosDevice))]

        [SerializeField, Unchangeable]
        public int m_Firmware = 0;

        /// <summary>
        /// Target device ID
        /// </summary>
        [Range(200, 254)]
        [SerializeField]
        [FormerlySerializedAs("DeviceID")]
        private byte m_DeviceID = 200;

        /// <summary>
        /// DeviceID
        /// </summary>
        public byte DeviceID => m_DeviceID;

        [SerializeField]
        private EDeviceType m_DeviceType;

        /// <summary>
        /// Type of device
        /// </summary>
        public EDeviceType DeviceType => m_DeviceType;

        [SerializeField]
        private ELRType m_LRType;

        /// <summary>
        /// Type of LR
        /// </summary>
        public ELRType LRType => m_LRType;
        
        [SerializeField]
        private CommandBoard.ModeMotorControl m_ModeSetting = CommandBoard.ModeMotorControl.TorqueB;

        [SerializeField, Unchangeable]
        private CommandBoard.ModeMotorControl m_Mode;

        [SerializeField]
        private bool m_ReverseFromDirectInput;

        /// <summary>
        /// Settings for force diretion
        /// </summary>
        public bool ReverseFromDirectInput => m_ReverseFromDirectInput;

        [Header("Connection")]
        [SerializeField]
        private EConnectionType m_PrimaryConnectionType = EConnectionType.Osc;

        [SerializeField]
        private bool m_ConnectionEnabled = true;

        [SerializeField]
        [FormerlySerializedAs("ConnectionSettings")]
        private ConnectionSettingBase[] m_ConnectionSettings;

        [SerializeField, Unchangeable]
        private ConnectionSettingBase m_UsedConnectionSetting;

        [SerializeField]
        private bool m_PacketLimitter = true;        

        [SerializeField]
        [Range(1, 10)]
        private int m_PacketLimitRate = 1;

        [SerializeField]
        private int m_ErrorCountLimit = 1000;

        [SerializeField, Unchangeable]
        private int m_ErrorCount = 0;

        [SerializeField, Unchangeable]
        private int m_ErrorCountMax = 0;

        [Header("Joint")]
        [SerializeField, Range(1, 2)]
        [FormerlySerializedAs("JointNumber")]
        private int m_JointNumber = 2;

        [SerializeField]
        [FormerlySerializedAs("Joints")]
        private ExosJoint[] m_Joints;

        [Header("Voltage")]
        [SerializeField, Unchangeable]
        [FormerlySerializedAs("voltage")]
        private float m_Voltage;

        [SerializeField]
        [FormerlySerializedAs("voltageDropLimit")]
        private float m_VoltageDropLimit = 10.5f;

        [SerializeField]
        [FormerlySerializedAs("OnVoltageDrop")]
        private UnityEvent m_OnVoltageDrop;

        [SerializeField]
        [FormerlySerializedAs("ReNotifyWaitMin")]
        private float m_ReNotifyWaitMin = 5.0f;

        [Header("Debug")]
        [SerializeField]
        private bool m_DebugLog = false;

        [SerializeField]
        private bool m_DeviceDebugMode = false;

        #endregion

        private LimitedList<float> m_VoltageFilter;
        private System.Diagnostics.Stopwatch m_ReInvokeWaitCount = new System.Diagnostics.Stopwatch();
        private IDisposable m_VoltageTimer;

        private bool VoltageDropInvoke
        {
            get { return (!m_ReInvokeWaitCount.IsRunning) || (m_ReInvokeWaitCount.IsRunning && m_ReInvokeWaitCount.Elapsed.Minutes > m_ReNotifyWaitMin); }
        }

        private int CounterDelay
        {
            get { return m_ErrorCount * DelayRate / m_ErrorCountLimit; }
        }

        /// <summary>
        /// Whether the device is enabled or not
        /// </summary>
        public bool Enabled
        {
            get
            {
                if (CommandBoard == null) { return false; }

                return (CommandBoard.Enable == CommandBoard.ModeEnable.Enable || CommandBoard.Enable == CommandBoard.ModeEnable.Debug);
            }

            set
            {
                if (CommandBoard == null) { return; }

                if (value == true)
                {
                    if (m_DeviceDebugMode)
                    {
                        CommandBoard.Enable = CommandBoard.ModeEnable.Debug;
                    }
                    else
                    {
                        CommandBoard.Enable = CommandBoard.ModeEnable.Enable;
                    }
                }
                else
                {
                    CommandBoard.Enable = CommandBoard.ModeEnable.None;
                }

                if (IsConnected)
                {
                    CommandPort.SendCommandAsync(Command.Short(CommandBoard, CommandBoard.Map.Enable));
                }
            }
        }

        /// <summary>
        /// Command board instance for controlling the device
        /// </summary>
        public CommandBoard CommandBoard { get; private set; }

        // public bool 

        /// <summary>
        /// Command port for communicating with the device
        /// </summary>
        public ICommandPort CommandPort { get; private set; }

        /// <summary>
        /// Whether or not the device and communication are connected
        /// </summary>
        public bool IsConnected
        {
            get { return CommandPort != null && CommandPort.IsConnected; }
        }

        /// <summary>
        /// Get the value of the device's battery voltage
        /// </summary>
        /// <returns>Voltage value</returns>
        public float Voltage
        {
            get
            {
                const float mVtoV = 1000.0f;

                return CommandBoard.AnalogReadMapValue[2] * mVtoV;
            }
        }
        /// <summary>
        /// Refresh when Editor value changed
        /// </summary>
        public void OnValidate()
        {
            UpdateName();

            if (m_Joints == null) { return; }

            System.Array.Resize(ref m_Joints, m_JointNumber);

            m_Joints.CheckNull().Foreach(x => x.OnValidate());
        }

        /// <summary>
        /// OnEnable
        /// </summary>
        public void OnEnable()
        {
            m_Joints.CheckNull().Foreach(x => x.ResetRatio());
        }

        private async Task<bool> SetupCommandPort(EConnectionType primary)
        {
            await new SynchronizationContextRemover();

            try
            {
                if (!await SetupPrimaryConnection(primary))
                {
                    if (!await SetupOtherConnection(primary))
                    {
                        if (m_DebugLog) { Debug.LogWarning($"[EXOS_SDK] Setup connection failed : {ExName}", this); }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EXOS_SDK] Setup connection failed : {ExName} : {e}", this);
                return false;
            }

            if (m_DebugLog) { Debug.Log($"[EXOS_SDK] Setup connection success : {ExName}", this); }

            Enabled = true;

            CommandPort.Disposed.Subscribe(_ => Termination());
            return true;
        }

        private async Task<bool> SetupPrimaryConnection(EConnectionType primary)
        {
            await new SynchronizationContextRemover();

            // Obtain Connection specified by primarys
            var target = m_ConnectionSettings
                .CheckNull()
                .Where(setting => setting.ConnectionType == primary)
                .FirstOrDefault();

            try
            {
                CommandPort = await target.GetCommandPortAsync(DeviceID);

                if (!IsConnected)
                {
                    if (m_DebugLog) { Debug.LogWarning($"[EXOS_SDK] Setup primary connection failed : {ExName}", this); }
                    return false;
                }

                m_UsedConnectionSetting = target;

                if (m_DebugLog) { Debug.Log($"[EXOS_SDK] Setup primary connection success : {ExName}", this); }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[EXOS_SDK][Exception] Setup primary connection failed : {ExName} : {e}",this);
                return false;
            }
        }

        private async Task<bool> SetupOtherConnection(EConnectionType primary)
        {
            await new SynchronizationContextRemover();

            // Obtain Connection from others
            var targets = m_ConnectionSettings
                .CheckNull()
                .Where(setting => setting.ConnectionType != primary);

            foreach (var target in targets)
            {
                try
                {
                    CommandPort = await target.GetCommandPortAsync(DeviceID);

                    if (!IsConnected)
                    {
                        if (m_DebugLog) { Debug.LogWarning($"[EXOS_SDK] Setup other connection failed : {ExName}", this); }
                        return false;
                    }

                    m_UsedConnectionSetting = target;

                    if (m_DebugLog) { Debug.Log($"[EXOS_SDK] Setup other connection success : {ExName}", this); }
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EXOS_SDK] Setup other connection failed :{ExName} : {e}", this);
                    return false;
                }
            }

            if (m_DebugLog) { Debug.LogWarning($"[EXOS_SDK] Setup all connection failed : {ExName}", this); }
            return false;
        }

        private bool CheckFirmwareVirsion()
        {
            if (CommandBoard == null) { return false; }

            if (CommandBoard.Firmware < 50)
            {
                switch(m_ModeSetting)
                {
                    case CommandBoard.ModeMotorControl.TorqueB:
                        m_Mode = CommandBoard.ModeMotorControl.TorqueA;
                        break;

                    case CommandBoard.ModeMotorControl.PositionB:
                        m_Mode = CommandBoard.ModeMotorControl.PositionA;
                        break;
                }
            }
            else
            {
                m_Mode = m_ModeSetting;
            }

            return true;
        }

        private void UpdateConnection()
        {
            if (IsConnected == false) { return; }

            m_ErrorCount = CommandPort.ErrorCount;

            if (!m_PacketLimitter || UnityEngine.Time.frameCount % (m_PacketLimitRate + CounterDelay) == 0)
            {
                if (CommandBoard.Firmware < 60)
                {
                    CommandPort.SendCommandAsync(Command.Short(CommandBoard, CommandBoard.Map.GoalTorqueAll));
                    CommandPort.SendCommandAsync(Command.Demand(CommandBoard, CommandBoard.Map.AnalogReadMapValueAll));
                }
                else
                {
                    Command[] commands = new Command[]
                    {
                        Command.Short(CommandBoard, CommandBoard.Map.GoalTorqueAll),
                        Command.Demand(CommandBoard, CommandBoard.Map.AnalogReadMapValueAll)
                    };

                    CommandPort.SendCommandAsync(commands);
                }                
            }

            if (m_ErrorCount > m_ErrorCountMax) { m_ErrorCountMax = m_ErrorCount; }

            if (CommandPort.ErrorCount > m_ErrorCountLimit)
            {
                Debug.LogError($"[EXOS_SDK] {ExName} has many error : {CommandPort.ErrorCount}. Disconect automaticaly.");

#pragma warning disable CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します
                TerminationAsync();
#pragma warning restore CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します
            }
        }

        private void CheckVoltage()
        {
            if (m_VoltageFilter == null) { return; }

            m_VoltageFilter.Add(Voltage);

            if (m_VoltageFilter.Max() < m_VoltageDropLimit && VoltageDropInvoke)
            {
                if (m_OnVoltageDrop != null) { m_OnVoltageDrop.Invoke(); }

                m_ReInvokeWaitCount.Reset();
                m_ReInvokeWaitCount.Start();
            }
        }

        /// <summary>
        /// Check it has a specific joint or not
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool HasJoint(EAxisType axis)
        {
            if (m_Joints == null) { return false; }

            return m_Joints.FirstOrDefault(x => x.AxisType == axis) != null;
        }

        /// <summary>
        /// Get a joint of specific type
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public ExosJoint GetJoint(EAxisType axis)
        {
            if (m_Joints == null) { return null; }

            return m_Joints.FirstOrDefault(x => x.AxisType == axis);
        }

        /// <summary>
        /// Try to Get a joint of specific type
        /// </summary>
        /// <param name="axis">Type of joint</param>
        /// <param name="joint">Joint obtained</param>
        /// <returns>Whether you could get the target</returns>
        public bool TryGetJoint(EAxisType axis, out ExosJoint joint)
        {
            joint = null;

            if (m_Joints == null) { return false; }

            joint = m_Joints.FirstOrDefault(x => x.AxisType == axis);

            if (joint != null) { return true; }

            return false;
        }

        /// <summary>
        /// Set 0 for all outputs of the device
        /// </summary>
        public void ResetForce()
        {
            m_Joints.Foreach(joint => joint.ForceRatio = 0);
        }

        /// <summary>
        /// Default callback when the voltage dropped.
        /// </summary>
        public void OnVoltageDropDefault()
        {
            Debug.LogWarning($"[EXOS_SDK] Battery voltage is droped under {m_VoltageDropLimit}[V]");
        }

        #region ExScriptableObject
        
        private AsyncSubject<bool> m_Initializing;

        private bool CompleteInitialize(bool result)
        {
            if (m_Initializing == null) { return result; }

            var initializing = m_Initializing;

            m_Initializing = null;

            initializing.OnNext(result);
            initializing.OnCompleted();

            return result;
        }

        /// <summary>
        /// Initialize device async
        /// </summary>
        [ContextMenu("Initialize")]
        public async Task<bool> InitializeAsync()
        {
            await new SynchronizationContextRemover();

            bool returnValue = false;

            CancellationTokenSource source = new CancellationTokenSource();

            Observable
                .OnceApplicationQuit()
                .Subscribe(_ => source.Cancel());

            try
            {
                if (!m_ConnectionEnabled) { return returnValue; }

                if (Enabled)
                {                
                    return returnValue = true;
                }

                if (m_Termination != null)
                {
                    await m_Termination; ;
                }

                if (m_Initializing != null)
                {
                    return returnValue = await m_Initializing;
                }

                m_Initializing = new AsyncSubject<bool>();

                if (ConnectionManager.IsExist)
                {
                    ConnectionManager.Instance.ConnectionUpdateEvent.AddListener(UpdateConnection);
                    ConnectionManager.Instance.ConnectionDisposeEvent.AddListener(Termination);
                }

                m_IsActive = true;

                Debug.Log($"[EXOS_SDK] Device is initializing : {ExName}", this);

                CommandBoard = CommandBoard.GetCommandBoard(DeviceID);

                m_ConnectionSettings.Foreach(x => x.Initialize());

                try
                {
                    var result = await SetupCommandPort(m_PrimaryConnectionType);

                    //if (source.Token.IsCancellationRequested) { return returnValue; }

                    if (result == false)
                    {
                        if (m_DebugLog) { Debug.LogWarningFormat($"[EXOS_SDK] SetupCommandPort timeout : {ExName}", this); }
                    }
                }
                catch (TimeoutException e)
                {
                    if (m_DebugLog) { Debug.LogErrorFormat($"[EXOS_SDK][Exception] SetupCommandPort failed : {ExName} : {e}", this); }

                    CompleteInitialize(false);

    #pragma warning disable CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します
                    TerminationAsync();
    #pragma warning restore CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します

                    return returnValue;
                }

                if (!IsConnected)
                {
                    CompleteInitialize(false);

    #pragma warning disable CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します
                    TerminationAsync();
    #pragma warning restore CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します

                    return returnValue;
                };

                //Scheduler.MainThread.Schedule(() => EditorApplication.isPlaying = false);

                try
                {
                    Command command;

                    command = await CommandPort.SendCommandAsync(Command.Demand(CommandBoard, CommandBoard.Map.Firmware));

                    //if (source.Token.IsCancellationRequested) { return returnValue; }

                    if (this == null) { return returnValue; }

                    if (command == null)
                    {
                        if (m_DebugLog) { Debug.LogWarning($"[EXOS_SDK] Get Firmwara timeout : {ExName}", this); }
                    }
                    else
                    {
                        m_Firmware = CommandBoard.Firmware;
                    }

                    command = await CommandPort.SendCommandAsync(Command.Demand(CommandBoard, CommandBoard.Map.ParameterAll));

                    //if (source.Token.IsCancellationRequested) { return returnValue; }

                    if (this == null) { return returnValue; }

                    if (command == null)
                    {
                        if (m_DebugLog) { Debug.LogWarning($"[EXOS_SDK] Get ParameterAll timeout : {ExName}", this); }
                    }
                }
                catch(TimeoutException e)
                {
                    if (m_DebugLog) { Debug.LogError($"[EXOS_SDK][Exception] Initialize failed : {ExName} : {e}", this); }

                    CompleteInitialize(false);

    #pragma warning disable CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します
                    TerminationAsync();
    #pragma warning restore CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します

                    return returnValue;
                }
                
                CheckFirmwareVirsion();

                m_Joints.CheckNull().Foreach(x => x.Initialize(this, m_Mode));

                m_VoltageFilter = new LimitedList<float>(60);

                m_OnVoltageDrop = new UnityEvent();

                if (MainThreadDispatcher.IsInitialized)
                {
                    m_VoltageTimer = Observable
                        .Interval(TimeSpan.FromSeconds(1))
                        .ObserveOnMainThread()
                        .Subscribe(_ => CheckVoltage());

                    Observable
                        .OnceApplicationQuit()
                        .Where(_ => m_VoltageTimer != null)
                        .Subscribe(_ => m_VoltageTimer.Dispose());
                }

                Debug.Log($"[EXOS_SDK] Device is initialized : {ExName}", this);
                return returnValue = true;
            }
            finally
            {
                CompleteInitialize(returnValue);
            }
        }

        /// <summary>
        /// Initialize device
        /// </summary>
        public async void Initialize()
        {
            await new SynchronizationContextRemover();

            await InitializeAsync();
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

        /// <summary>
        /// Terminate device async
        /// </summary>
        [ContextMenu("Termination")]
        public async Task<bool> TerminationAsync()
        {
            await new SynchronizationContextRemover();

            bool returnValue = false;

            try
            {
                if (m_Initializing != null)
                {
                    await m_Initializing;
                }

                if (m_Termination != null)
                {
                    return returnValue = await m_Termination; ;
                }

                m_Termination = new AsyncSubject<bool>();

                if (ConnectionManager.IsExist)
                {
                    ConnectionManager.Instance.ConnectionUpdateEvent.RemoveListener(UpdateConnection);
                    ConnectionManager.Instance.ConnectionDisposeEvent.RemoveListener(Termination);
                }

                if (CommandBoard == null)
                {
                    return returnValue;
                }

                if (IsConnected)
                {
                    m_Joints.CheckNull().Foreach(x => x.ResetRatio());
                    m_Joints.CheckNull().Foreach(x => x.ModeMotorControl = CommandBoard.ModeMotorControl.None);
                }

                Enabled = false;

                if (m_UsedConnectionSetting != null && CommandPort != null)
                {
                    bool result;

                    try
                    {
                        result = await m_UsedConnectionSetting.DisposeCommandPortAsync(CommandPort);
                    }
                    catch (TimeoutException e)
                    {
                        if (m_DebugLog) { Debug.LogError($"[EXOS_SDK][Exception] DisposeCommandPortAsync failed : {ExName} : {e}", this); }

                        return returnValue;
                    }

                    if (result == false)
                    {
                        if (m_DebugLog) { Debug.LogWarning($"[EXOS_SDK] DisposeCommandPortAsync failed : {ExName}", this); }
                    }

                    m_UsedConnectionSetting = null;
                }

                m_ConnectionSettings.Foreach(x => x.Termination());

                if (IsConnected)
                {
                    CommandPort.Dispose();
                    CommandPort = null;
                }

                if (m_VoltageTimer != null)
                {
                    m_VoltageTimer.Dispose();
                    m_VoltageTimer = null;
                }

                m_VoltageFilter = null;
                m_OnVoltageDrop = null;

                if (CommandBoard != null)
                {
                    CommandBoard.Dispose();
                    CommandBoard = null;
                }

                m_Firmware = 0;
                m_ErrorCount = 0;
                m_ErrorCountMax = 0;

                m_Mode = CommandBoard.ModeMotorControl.None;

                m_IsActive = false;

                Debug.Log($"[EXOS_SDK] Device is terminated : {ExName}", this);

                return returnValue = true;
            }
            finally
            {
                CompletemTermination(returnValue);
            }
        }

        /// <summary>
        /// Terminate device
        /// </summary>
        public async void Termination()
        {
            await new SynchronizationContextRemover();

            await TerminationAsync();
        }

        #endregion
    }
}