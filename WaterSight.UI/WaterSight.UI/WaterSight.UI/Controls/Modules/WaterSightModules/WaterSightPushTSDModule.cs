using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterSight.Model.Generator.Data;
using WaterSight.UI.Modules;

namespace WaterSight.UI.Controls.Modules.WaterSightModules
{
    public partial class WaterSightPushTSDModule : UserControl, IModuleControl
    {
        public WaterSightPushTSDModule()
        {
            InitializeComponent();


            TimeSeriesDbStructure = new TimeSeriesDbStructure(
                filePath: string.Empty);
        }

        //protected override void InitializeBinding()
        //{
        //}

        //protected override void InitializeVisually()
        //{
        //}

        private TimeSeriesDbStructure TimeSeriesDbStructure { get; }

        public bool IsMinimized { get; set; } = true;
        public bool CanSave { get; set; } = true;
        public string Label => moduleControlBase.Label;
        public Category Category { get; } = Category.WaterSightPushTSD;
        public TargetType TargetType { get; } = TargetType.WaterSight;
        public int OrderNumber { get; set; }
        public bool WaitForPrevious { get; set; } = false;
        //public IModuleControl? ModuleControl { get; set; }
        public int PercentDone { get; set; }

        public event EventHandler<UI.Modules.TaskStatus>? TaskStatusChanged;


        public void ToggleSize()
        {

        }
        public void Run()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public bool ValidateInput()
        {
            TaskStatusChanged?.Invoke(this,
                new UI.Modules.TaskStatus($"{Label}: Validation Started", TaskStatusType.ValidationInProgress));

            var validated = true;
            try
            {


            }
            catch (Exception ex)
            {
                TaskStatusChanged?.Invoke(this,
                    new UI.Modules.TaskStatus($"{Label}: Validation Failed", TaskStatusType.ValidationFailed));

                validated = false;
            }

            TaskStatusChanged?.Invoke(this,
                new UI.Modules.TaskStatus($"{Label}: Validation Finished", TaskStatusType.ValidationPassed));

            return validated;
        }

       
    }
}
