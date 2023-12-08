using Haestad.Framework.Application;
using Haestad.Framework.Windows.Forms.Forms;
using OpenFlows.Application;
using OpenFlows.Water;
using System;

namespace WaterSight.Model.Forms;

public partial class WaterAppParentForm : HaestadParentForm, IParentFormSurrogate
{
    #region Constructor
    public WaterAppParentForm(HaestadParentFormModel parentFormModel)
        : base(parentFormModel)
    {
        InitializeComponent();
    }
    #endregion

    #region Public Methods   
    public void SetParentWindowHandle(IntPtr handle)
    {
        //no-op
    }
    #endregion

    #region Event Handlers
    protected override void HaestadForm_Load(object sender, EventArgs e)
    {
        OpenFlowsWater.StartSession(ParentFormModel.LicensedFeatureSet);

        //var modelFilePath = @"D:\Development\Data\ModelData\NCAR\NCAR.wtg";
        //if (OpenFile(modelFilePath))
        //{
        //    var waterModel = OpenFlowsWater.GetModel(ApplicationManager.GetInstance().ParentFormModel.CurrentProject);
        //}



        base.HaestadForm_Load(sender, e);
    }
    #endregion
}
