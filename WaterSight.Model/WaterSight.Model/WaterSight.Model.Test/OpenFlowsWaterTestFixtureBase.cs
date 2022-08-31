
using Haestad.Drawing.Domain;
using Haestad.Framework.Application;
using Haestad.Mapping.Support;
using Haestad.Support.Support;
using Haestad.Water.Forms.Application;
using NUnit.Framework;
using OpenFlows.Application;
using OpenFlows.Water.Application;
using OpenFlows.Water.Domain;
using System.IO;

namespace WaterSight.Model.Test;

public abstract class OpenFlowsWaterTestFixtureBase
{
    #region Constructor
    public OpenFlowsWaterTestFixtureBase()
    {

    }
    #endregion

    #region Setup/Tear-down
    [SetUp]
    public void Setup()
    {   
        ApplicationManagerBase.SetApplicationManager(new WaterApplicationManager());

        // By passing in false, this will suppress the primary user interface.
        // Make sure you are logged into CONNECTION client.
        WaterApplicationManager.GetInstance().Start(false);

        SetupImpl();
    }
    protected virtual void SetupImpl()
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
    protected void OpenModel(string filename)
    {
        ProjectProperties app = ProjectProperties.Default;
        app.NominalProjectPath = filename;
        WaterAppManager.ParentFormModel.OpenProject(app);
    }
    protected virtual string BuildTestFilename(string filename)
    {
        // The defualt base path is the samples folder for WaterGEMS. You can change this to
        // whatever you want.  Remember this is the BASE path as it will be comined with the provided filename.
        return Path.Combine(@"C:\Program Files (x86)\Bentley\WaterGEMS\Samples", filename);
    }
    protected void SaveAs(string filePath = "")
    {
        ProjectProperties app = ProjectProperties.Default;
        app.NominalProjectPath = string.IsNullOrEmpty(filePath) ? Project.FullPath : filePath;
        WaterAppManager.ParentFormModel.SaveAsProject(app);
    }
    #endregion

    #region Protected Properties
    protected IWaterApplicationManager WaterAppManager => WaterApplicationManager.GetInstance();
    protected IWaterModel WaterModel => WaterAppManager.CurrentWaterModel;
    protected IProject Project => WaterAppManager.ParentFormModel.CurrentProject;
    protected IDomainProject DomainProject => Project as IDomainProject;
    protected IGraphicalProject GraphicalProject => Project as IGraphicalProject;

    protected HaestadParentFormModel WaterParentFormModel => WaterApplicationManager.GetInstance().ParentFormModel;
    protected OpenFlowsWaterParentFormUIModel WaterParentFormUIModel => (OpenFlowsWaterParentFormUIModel)WaterAppManager.ParentFormUIModel;
    protected IWaterApplicationModel DomainAppMaodel => (IWaterApplicationModel)WaterAppManager.DomainApplicationModel;
    protected IMappingApplicationModel MappingAppModel => (IMappingApplicationModel)DomainAppMaodel;
    protected IFeatureManager FeatureManager => MappingAppModel.FeatureManager;

    #endregion
}
