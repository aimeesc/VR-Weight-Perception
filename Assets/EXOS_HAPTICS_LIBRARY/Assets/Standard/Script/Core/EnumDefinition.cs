namespace exiii.Unity
{
#pragma warning disable CS1591 // 公開されている型またはメンバーの XML コメントがありません

    #region ExTag

    public enum ETag
    {
        UserDefined = 0,
        Starter = 1,
        Ender = 2,
        Caster = 3,
        IgnorePhysics = 4, // [TODO]わかりづらいので変更検討
        ReferenceTransform = 5,
        VoxelTrigger = 6,
        TrackingOrigin = 7,
        Tools = 8,
        SystemCollider = 9,
    }

    public enum ETouchType
    {
        Penalty,
        Tools,
    }

    public enum EDeviceEquipType
    {
        Default,
        Reverse,
    }

    public enum EPathType
    {
        All,
        Force,
        Position,
        Vibration,
        Materila,
    }

    /// <summary>
    /// Mode of hand
    /// </summary>
    public enum EHandMode
    {
        Controller,
        Tracker,
        HandTracking,
    }

    #endregion ExTag

    /// <summary>
    /// Type of Tracking
    /// </summary>
    public enum ETrackingType
    {
        Controller,
        Tracker,
        Camera,
    }

    /// <summary>
    /// Type of prefab
    /// </summary>
    public enum EPrefabType
    {
        PhysicsModel,
        DisplayModel,
        TouchManipulator,
        EventCaster,
        Detector,
        HandModel,
        ArmModel,
    }

    /// <summary>
    /// Type of Manipulation
    /// </summary>
    public enum EManipulationType
    {
        Touch,
        Grip,
        Grab,
        Use,
        ThumStick, // thum stick button.

        None = 255,
    }

    /// <summary>
    /// Type of TrackingPosition
    /// </summary>
    public enum ETrackingPositionType
    {
        Palm,
        BackOfTheHand,
        InsideTheArm,
        OutsideTheArm,
        BackOfTheHandOnJoint,
    }

    public enum EDetectionType
    {
        Any,
        All,
    }

    public enum EListType
    {
        WhiteOR,
        BlackOR,
    }        

#pragma warning restore CS1591 // 公開されている型またはメンバーの XML コメントがありません
}