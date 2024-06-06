namespace WaterSight.UI.Controls.Modules.WaterSightModules
{
    partial class WaterSightControlBase
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaterSightControlBase));
            CheckBoxUseAnalysisDir = new CheckBox();
            SuspendLayout();
            // 
            // ProgressBarControl
            // 
            ProgressBarControl.Size = new Size(271, 19);
            // 
            // CheckBoxEnableAction
            // 
            CheckBoxEnableAction.Checked = true;
            CheckBoxEnableAction.CheckState = CheckState.Checked;
            // 
            // labelSectionTitle
            // 
            labelSectionTitle.Size = new Size(401, 23);
            // 
            // buttonAddOrRemove
            // 
            buttonAddOrRemove.ForeColor = Color.DarkGreen;
            buttonAddOrRemove.Location = new Point(404, 0);
            buttonAddOrRemove.Text = "┼";
            // 
            // ButtonRun
            // 
            ButtonRun.Image = (Image)resources.GetObject("ButtonRun.Image");
            ButtonRun.Location = new Point(404, 25);
            // 
            // ButtonValidate
            // 
            ButtonValidate.Image = (Image)resources.GetObject("ButtonValidate.Image");
            ButtonValidate.Location = new Point(378, 25);
            // 
            // buttonSave
            // 
            buttonSave.Image = (Image)resources.GetObject("buttonSave.Image");
            buttonSave.Location = new Point(349, 25);
            // 
            // CheckBoxUseAnalysisDir
            // 
            CheckBoxUseAnalysisDir.AutoSize = true;
            CheckBoxUseAnalysisDir.Location = new Point(3, 53);
            CheckBoxUseAnalysisDir.Name = "CheckBoxUseAnalysisDir";
            CheckBoxUseAnalysisDir.Size = new Size(109, 19);
            CheckBoxUseAnalysisDir.TabIndex = 11;
            CheckBoxUseAnalysisDir.Text = "Use Analysis Dir";
            CheckBoxUseAnalysisDir.UseVisualStyleBackColor = true;
            // 
            // WaterSightControlBase
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(CheckBoxUseAnalysisDir);
            MinimumSize = new Size(315, 75);
            Name = "WaterSightControlBase";
            Size = new Size(374, 103);
            Controls.SetChildIndex(buttonSave, 0);
            Controls.SetChildIndex(ButtonRun, 0);
            Controls.SetChildIndex(ButtonValidate, 0);
            Controls.SetChildIndex(labelSectionTitle, 0);
            Controls.SetChildIndex(buttonAddOrRemove, 0);
            Controls.SetChildIndex(CheckBoxEnableAction, 0);
            Controls.SetChildIndex(ProgressBarControl, 0);
            Controls.SetChildIndex(CheckBoxUseAnalysisDir, 0);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public CheckBox CheckBoxUseAnalysisDir;
    }
}
