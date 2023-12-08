namespace WaterSight.UI.Controls.Modules;


public class ModuleControlBaseEventArgs : EventArgs
{
    public ModuleControlBaseEventArgs(ModuleControlBaseEventType eventType)
    {
        ModuleControlBaseEventType = eventType;
    }

    public ModuleControlBaseEventType ModuleControlBaseEventType { get; set; }
}

public enum ModuleControlBaseEventType
{
    MoveUp,
    MoveDown,
    ToggleSize,
    Close
}