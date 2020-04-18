namespace exiii.Unity
{
    public enum EDebugActions
    {
        SwitchColliderDisplay,
        SwitchEditor,
        SwitchMeter,
        SwitchController,
        SwitchTracker,
        SwitchObject,
        DebugAction0 = 100,
        DebugAction1 = 101,
        DebugAction2 = 102,
        DebugAction3 = 103,
    }

    public enum EControllerActions
    {
        ChangeForceEnable,
        Reconnect,
    }

    public enum ESceneActions
    {
        Scene0 = 0,
        Scene1 = 1,
        Scene2 = 2,
        Scene3 = 3,
        Scene4 = 4,
        Scene5 = 5,
        Scene6 = 6,
        Scene7 = 7,
        Scene8 = 8,
        Scene9 = 9,
    }

    public enum ECameraActions
    {
        MoveRight,
        MoveLeft,
        MoveForward,
        MoveBack,
        MoveUp,
        MoveDown,
        TurnRight,
        TurnLeft,
        PositionReset,
        SaveTransform,
        LoadTransform,
        DeleteSave,
        Amplifier,
        ChangeEnableSubCamera,
    }

    public enum EVRControllerActions
    {
        RUseStart,
        RUseUpdate,
        RUseEnd,
        RGrabStart,
        RGrabUpdate,
        RGrabEnd,        
        LUseStart,
        LUseUpdate,
        LUseEnd,
        LGrabStart,
        LGrabUpdate,
        LGrabEnd,
        RThumStickStart, // R thum stick button.
        RThumStickUpdate, // R thum stick button.
        RThumStickEnd, // R thum stick button.
        LThumStickStart, // L thum stick button.
        LThumStickUpdate, // L thum stick button.
        LThumStickEnd, // L thum stick button.
    }
}