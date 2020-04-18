namespace exiii.Unity.Device
{
#pragma warning disable CS1591 // 公開されている型またはメンバーの XML コメントがありません

    /// <summary>
    /// Type of Device
    /// </summary>
    public enum EDeviceType
    {
        Wrist,
        Gripper,
    }

    /// <summary>
    /// Type of Axis
    /// </summary>
    public enum EAxisType
    {
        Pinch, // 開閉
        Abduction, // 撓尺屈
        Flexion, // 掌背屈
    }

#pragma warning restore CS1591 // 公開されている型またはメンバーの XML コメントがありません
}