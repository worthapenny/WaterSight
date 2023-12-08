
using Haestad.Drawing.Domain;
using Haestad.Framework.Application;
using Haestad.LicensingFacade;
using Haestad.Mapping.Support;
using Haestad.Support.Support;
using Haestad.Water.Forms.Application;
using Haestad.WaterProduct.Application;
using NUnit.Framework;
using OpenFlows.Application;
using OpenFlows.Water;
using OpenFlows.Water.Application;
using OpenFlows.Water.Domain;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WaterSight.Model.Library;
using WaterSight.Model.Support;
using static OpenFlows.Water.OpenFlowsWater;


namespace WaterSight.Model.Test;

public abstract class OpenFlowsWaterTestFixtureBase
{
    #region Constructor
    public OpenFlowsWaterTestFixtureBase(
        bool shouldOpenWaterProject = false
        )
    {
        ShouldOpenWaterProject = shouldOpenWaterProject;

        string logTemplate = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug() // Default's to Information 
            .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
            .WriteTo.Debug(outputTemplate: logTemplate)
            .CreateLogger();
    }
    #endregion

    #region Setup/Tear-down
    [OneTimeSetUp]
    public void Setup()
    {   
        ApplicationManagerBase.SetApplicationManager(new WaterApplicationManager());

        // By passing in false, this will suppress the primary user interface.
        // Make sure you are logged into CONNECTION client.
        WaterApplicationManager.GetInstance().Start(false);

        OneTimeSetupImpl();
    }
    protected virtual void OneTimeSetupImpl()
    {
    }
    [TearDown]
    public void Teardown()
    {
        TeardownImpl();

        WaterApplicationManager.GetInstance().Stop();
    }
    protected virtual void TeardownImpl()
    {

    }
    #endregion

    #region Protected Methods
    protected IWaterModel OpenProject()
    {
        if (waterModel != null)
        {
            Log.Information($"Water model, '{waterModel}', is already opened.");
            return waterModel;
        }

        Assert.That(File.Exists(ModelFilePath), Is.True);

        LogLibrary.Separate_StartGroup();
        Log.Debug($"About to open the hydraulic model. File: {ModelFilePath}");

        Assert.That(StartSession(WaterProductLicenseType.WaterGEMS), Is.EqualTo(LicenseRunStatusEnum.OK));
        Assert.That(IsValid, Is.True);
        Log.Debug($"License for {Enum.GetName(typeof(WaterProductLicenseType), WaterProductLicenseType.WaterGEMS)} is OK.");


        if (!ShouldOpenWaterProject)
        {
            waterModel = Open(ModelFilePath);
            IsWaterModelOpen = true;
        }
        else
        {
            ApplicationManagerBase.SetApplicationManager(new WaterApplicationManager());
            AppManager.SetParentFormSurrogateDelegate((fm) =>
                new Win32WindowSurrogate(new Form()));


            //AppManager.Start(true); // false = suppress the primary user interface
            //AppManager.ParentFormUIModel.OpenProject(ModelFilePath);

            AppManager.Start(true); // false = suppress the primary user interface
            var app = ProjectProperties.Default;
            app.NominalProjectPath = ModelFilePath;
            AppManager.ParentFormModel.OpenProject(app);

            waterModel = AppManager.CurrentWaterModel;
            IsWaterModelOpen = waterModel != null;
        }

        Log.Information($"Opened the hydraulic model. File: {ModelFilePath}");
        LogLibrary.Separate_EndGroup();

        Assert.That(waterModel, Is.Not.Null);
        return waterModel;
    }
    protected void OpenProjectInMainUI()
    {
        Process.Start(ModelFilePath);
    }
    protected void SaveProject()
    {
        Log.Debug($"About to save, Path: {ModelFilePath}");

        ProjectProperties pp = ProjectProperties.Default;
        pp.NominalProjectPath = ModelFilePath;

        //AppManager.ParentFormModel.SaveAsProject(pp);
        AppManager.ParentFormModel.CurrentProject.Save(pp);

        Log.Information($"Saved. Path: {ModelFilePath}");
    }
    protected void SaveAsProject()
    {
        Log.Debug($"About to save as, Path: {ModelFilePath}");

        ProjectProperties pp = ProjectProperties.Default;
        pp.NominalProjectPath = ModelFilePath;

        //AppManager.ParentFormModel.SaveAsProject(pp);
        AppManager.ParentFormModel.CurrentProject.SaveAs(pp);

        Log.Information($"Saved to {ModelFilePath}");
    }
    protected void CloseProject()
    {
        if (!IsWaterModelOpen)
        {
            Log.Information($"Project is not opened. Nothing to close");
            return;
        }

        Log.Debug($"Closing the model...");

        var pfm = WaterApplicationManager.GetInstance()?.ParentFormModel;
        if (pfm != null)
        {
            ProjectProperties pp = ProjectProperties.Default;
            pp.NominalProjectPath = pfm.CurrentProject.FullPath;
            pfm.CloseCurrentProject(pp);
        }
        else
        {
            // Only WaterModel was opened (not the project)
            WaterModel.Close();
            waterModel = null;
        }

        IsWaterModelOpen = false;
        Log.Information($"Closed the model. Path: {ModelFilePath}");
        Log.Debug(new string('x', 100));
    }
    #endregion

    #region Protected Properties
    protected IWaterApplicationManager WaterAppManager => WaterApplicationManager.GetInstance();
    protected string ModelFilePath { get; set; }
    public IWaterModel WaterModel => waterModel ??= OpenProject();
    public ModelEditor ModelEditor => modelEditor ??= new ModelEditor(WaterModel);
    protected bool IsWaterModelOpen { get; set; }

    public WaterApplicationManager AppManager => (WaterApplicationManager)WaterApplicationManager.GetInstance();
    public IProject Project => AppManager.ParentFormModel.CurrentProject;
    public IDomainProject DomainProject => (IDomainProject)Project;
    protected WaterProductParentFormModel WaterParentFormModel => (WaterProductParentFormModel)AppManager.ParentFormModel;
    protected WaterProductParentFormUIModel WaterParentFormUIModel => (WaterProductParentFormUIModel)AppManager.ParentFormUIModel;
    protected IDomainApplicationModel WaterAppModel => AppManager.DomainApplicationModel;

    protected IGraphicalProject CurrentProject => (IGraphicalProject)WaterParentFormModel.CurrentProject;
    protected IMappingApplicationModel MappingAppModel => (IMappingApplicationModel)WaterAppModel;
    protected IFeatureManager FeatureManager => MappingAppModel.FeatureManager;

    #endregion

    #region Private Properties
    private bool ShouldOpenWaterProject { get; set; } = false;
    #endregion


    #region Private Fields
    private IWaterModel waterModel;
    private ModelEditor modelEditor;
    //private bool _isOpened = false;
    #endregion
}
