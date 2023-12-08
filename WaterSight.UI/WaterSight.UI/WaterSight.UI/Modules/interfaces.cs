using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSight.UI.Modules;

public interface IModuleControl
{


    public bool IsMinimized { get; set; }
    public bool CanSave { get; set; }

    

    public void ToggleSize();
    public bool ValidateInput();
    public void Run();
    public void Save();

    public string Label { get; }
    public Category Category { get; }
    public TargetType TargetType { get; }

    public int OrderNumber { get; set; }
    public bool WaitForPrevious { get; set; }
    //public IModuleControl? ModuleControl { get; set; }

    public int PercentDone { get; set; }

    public event EventHandler<TaskStatus>? TaskStatusChanged;

    //public Size MaxSize { get; set; }
}

//public class ModuleControl : Control, IModuleControl
//{
//    public bool IsMinimized { get; set; }
//    public bool CanSave { get; set; }
//}

//public interface IModuleBase
//{
//    public void Run();
//    public void Save();

//    public string Label { get; }
//    public Category Category { get; }
//    public TargetType TargetType { get; }

//    public int OrderNumber { get; set; }
//    public bool WaitForPrevious { get; set; }
//    public IModuleControl? ModuleControl { get; set; }

//    public int PercentDone { get; set; }

//    public event EventHandler<TaskStatus> TaskStatusChanged;
//}



//public abstract class ModuleBase : IModuleBase
//{
//    #region Constructor
//    public ModuleBase(string label, Category category, TargetType targetType)
//    {
//        Label = label;
//        Category = category;
//        TargetType = targetType;

//    }

//    #endregion

//    #region Public Methods
//    public abstract void Run();
//    public abstract void Save();
//    #endregion

//    #region Public Events
//    public event EventHandler<TaskStatus>? TaskStatusChanged;
//    #endregion

//    #region Public Properties

//    public string Label { get; }
//    public Category Category { get; }
//    public TargetType TargetType { get; }

//    public int OrderNumber { get; set; }
//    public bool WaitForPrevious { get; set; } = true;
//    public int PercentDone { get; set; }
//    public IModuleControl? ModuleControl { get; set; }

//    #endregion

//}

public enum TaskStatusType
{
    ValidationInProgress,
    ValidationPassed,
    ValidationFailed,
    InQueue,
    Started,
    InProgress,
    Finished,
    FinishedWithError
}

public class TaskStatus : EventArgs
{
    public TaskStatus(string message, TaskStatusType taskStatusType)
    {
        Message = message;
        Type = TaskStatusType.InQueue;
    }

    public TaskStatusType Type { get; private set; }
    public string Message { get; private set; }
    public int PercentDone { get; private set; }
}


public enum Category
{
    WaterSightSensors,
    WaterSightGIS,
    WaterSightZone,
    WaterSightSettings,
    WaterSightNumericModel,
    WaterSightPumps,
    WaterSightTanks,
    WaterSightSmartMeters,
    WaterSightPushTSD,
}

public enum TargetType
{
    WaterSight,
    WaterGEMS,
    Excel
}