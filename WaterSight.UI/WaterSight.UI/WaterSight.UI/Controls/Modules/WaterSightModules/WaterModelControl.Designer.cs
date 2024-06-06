namespace WaterSight.UI.Controls.Modules.WaterSightModules
{
    partial class WaterModelControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaterModelControl));
            textBoxModelFilePath = new TextBox();
            labelModelFilePath = new Label();
            buttonModelOpen = new Button();
            groupBoxModel = new GroupBox();
            labelZipFilePath = new Label();
            textBoxZipFilePath = new TextBox();
            buttonZipModel = new Button();
            buttonBrowseModel = new Button();
            checkBoxForceZip = new CheckBox();
            groupBoxModel.SuspendLayout();
            SuspendLayout();
            // 
            // ProgressBarControl
            // 
            ProgressBarControl.Size = new Size(286, 19);
            // 
            // labelSectionTitle
            // 
            labelSectionTitle.BackColor = Color.SteelBlue;
            labelSectionTitle.Size = new Size(387, 23);
            labelSectionTitle.Text = "Water Model: Upload Model";
            // 
            // buttonAddOrRemove
            // 
            buttonAddOrRemove.Location = new Point(390, 0);
            // 
            // ButtonRun
            // 
            ButtonRun.Image = (Image)resources.GetObject("ButtonRun.Image");
            ButtonRun.Location = new Point(390, 24);
            // 
            // ButtonValidate
            // 
            ButtonValidate.Image = (Image)resources.GetObject("ButtonValidate.Image");
            ButtonValidate.Location = new Point(364, 24);
            // 
            // buttonSave
            // 
            buttonSave.Image = (Image)resources.GetObject("buttonSave.Image");
            buttonSave.Location = new Point(335, 24);
            // 
            // textBoxModelFilePath
            // 
            textBoxModelFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxModelFilePath.BackColor = Color.LightSteelBlue;
            textBoxModelFilePath.BorderStyle = BorderStyle.FixedSingle;
            textBoxModelFilePath.Location = new Point(72, 86);
            textBoxModelFilePath.Name = "textBoxModelFilePath";
            textBoxModelFilePath.ReadOnly = true;
            textBoxModelFilePath.RightToLeft = RightToLeft.Yes;
            textBoxModelFilePath.Size = new Size(263, 23);
            textBoxModelFilePath.TabIndex = 9;
            textBoxModelFilePath.Text = "Select Water Model";
            textBoxModelFilePath.TextAlign = HorizontalAlignment.Right;
            // 
            // labelModelFilePath
            // 
            labelModelFilePath.AutoSize = true;
            labelModelFilePath.Location = new Point(3, 88);
            labelModelFilePath.Name = "labelModelFilePath";
            labelModelFilePath.Size = new Size(68, 15);
            labelModelFilePath.TabIndex = 8;
            labelModelFilePath.Text = "Model Path";
            // 
            // buttonModelOpen
            // 
            buttonModelOpen.Location = new Point(6, 0);
            buttonModelOpen.Name = "buttonModelOpen";
            buttonModelOpen.Size = new Size(85, 23);
            buttonModelOpen.TabIndex = 11;
            buttonModelOpen.Text = "Open Model";
            buttonModelOpen.UseVisualStyleBackColor = true;
            // 
            // groupBoxModel
            // 
            groupBoxModel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxModel.Controls.Add(buttonModelOpen);
            groupBoxModel.Location = new Point(3, 171);
            groupBoxModel.Name = "groupBoxModel";
            groupBoxModel.Size = new Size(410, 106);
            groupBoxModel.TabIndex = 12;
            groupBoxModel.TabStop = false;
            groupBoxModel.Text = "groupBox1";
            // 
            // labelZipFilePath
            // 
            labelZipFilePath.AutoSize = true;
            labelZipFilePath.Location = new Point(3, 117);
            labelZipFilePath.Name = "labelZipFilePath";
            labelZipFilePath.Size = new Size(51, 15);
            labelZipFilePath.TabIndex = 8;
            labelZipFilePath.Text = "Zip Path";
            // 
            // textBoxZipFilePath
            // 
            textBoxZipFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxZipFilePath.BackColor = Color.LightSteelBlue;
            textBoxZipFilePath.BorderStyle = BorderStyle.FixedSingle;
            textBoxZipFilePath.Location = new Point(72, 115);
            textBoxZipFilePath.Name = "textBoxZipFilePath";
            textBoxZipFilePath.ReadOnly = true;
            textBoxZipFilePath.RightToLeft = RightToLeft.Yes;
            textBoxZipFilePath.Size = new Size(263, 23);
            textBoxZipFilePath.TabIndex = 9;
            textBoxZipFilePath.TextAlign = HorizontalAlignment.Right;
            // 
            // buttonZipModel
            // 
            buttonZipModel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonZipModel.Location = new Point(341, 115);
            buttonZipModel.Name = "buttonZipModel";
            buttonZipModel.Size = new Size(72, 23);
            buttonZipModel.TabIndex = 11;
            buttonZipModel.Text = "Zip It";
            buttonZipModel.UseVisualStyleBackColor = true;
            // 
            // buttonBrowseModel
            // 
            buttonBrowseModel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonBrowseModel.Location = new Point(341, 86);
            buttonBrowseModel.Name = "buttonBrowseModel";
            buttonBrowseModel.Size = new Size(72, 23);
            buttonBrowseModel.TabIndex = 11;
            buttonBrowseModel.Text = "Browse";
            buttonBrowseModel.UseVisualStyleBackColor = true;
            // 
            // checkBoxForceZip
            // 
            checkBoxForceZip.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBoxForceZip.AutoSize = true;
            checkBoxForceZip.Location = new Point(338, 144);
            checkBoxForceZip.Name = "checkBoxForceZip";
            checkBoxForceZip.RightToLeft = RightToLeft.Yes;
            checkBoxForceZip.Size = new Size(75, 19);
            checkBoxForceZip.TabIndex = 12;
            checkBoxForceZip.Text = "Force Zip";
            checkBoxForceZip.UseVisualStyleBackColor = true;
            // 
            // WaterModelControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightSteelBlue;
            Controls.Add(checkBoxForceZip);
            Controls.Add(buttonBrowseModel);
            Controls.Add(buttonZipModel);
            Controls.Add(groupBoxModel);
            Controls.Add(textBoxZipFilePath);
            Controls.Add(textBoxModelFilePath);
            Controls.Add(labelZipFilePath);
            Controls.Add(labelModelFilePath);
            MinimumSize = new Size(315, 103);
            Name = "WaterModelControl";
            Section = "Water Model";
            Size = new Size(416, 280);
            Title = "Upload Model";
            TitleColor = Color.SteelBlue;
            Controls.SetChildIndex(buttonSave, 0);
            Controls.SetChildIndex(CheckBoxUseAnalysisDir, 0);
            Controls.SetChildIndex(ButtonRun, 0);
            Controls.SetChildIndex(ButtonValidate, 0);
            Controls.SetChildIndex(labelSectionTitle, 0);
            Controls.SetChildIndex(buttonAddOrRemove, 0);
            Controls.SetChildIndex(CheckBoxEnableAction, 0);
            Controls.SetChildIndex(ProgressBarControl, 0);
            Controls.SetChildIndex(labelModelFilePath, 0);
            Controls.SetChildIndex(labelZipFilePath, 0);
            Controls.SetChildIndex(textBoxModelFilePath, 0);
            Controls.SetChildIndex(textBoxZipFilePath, 0);
            Controls.SetChildIndex(groupBoxModel, 0);
            Controls.SetChildIndex(buttonZipModel, 0);
            Controls.SetChildIndex(buttonBrowseModel, 0);
            Controls.SetChildIndex(checkBoxForceZip, 0);
            groupBoxModel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxModelFilePath;
        private Label labelModelFilePath;
        private Button buttonModelOpen;
        private GroupBox groupBoxModel;
        private Label labelZipFilePath;
        private TextBox textBoxZipFilePath;
        private Button buttonZipModel;
        private Button buttonBrowseModel;
        private CheckBox checkBoxForceZip;
    }
}
