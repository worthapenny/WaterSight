namespace WaterSight.UI.Controls.Modules.WaterSightModules
{
    partial class ExcelUploadControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExcelUploadControl));
            labelExcelCategory = new Label();
            comboBoxExcelCategory = new ComboBox();
            checkBoxDeleteExistingItems = new CheckBox();
            labelLocalPath = new Label();
            textBoxXlFilePath = new TextBox();
            SuspendLayout();
            // 
            // ProgressBarControl
            // 
            ProgressBarControl.Size = new Size(228, 19);
            // 
            // CheckBoxEnableAction
            // 
            CheckBoxEnableAction.Checked = true;
            CheckBoxEnableAction.CheckState = CheckState.Checked;
            // 
            // labelSectionTitle
            // 
            labelSectionTitle.BackColor = Color.SeaGreen;
            labelSectionTitle.Size = new Size(329, 23);
            labelSectionTitle.Text = "Excel: Upload Excel File";
            // 
            // buttonAddOrRemove
            // 
            buttonAddOrRemove.ForeColor = Color.DarkGreen;
            buttonAddOrRemove.Location = new Point(332, -2);
            buttonAddOrRemove.Text = "┼";
            // 
            // ButtonRun
            // 
            ButtonRun.Image = (Image)resources.GetObject("ButtonRun.Image");
            ButtonRun.Location = new Point(332, 24);
            // 
            // ButtonValidate
            // 
            ButtonValidate.Image = (Image)resources.GetObject("ButtonValidate.Image");
            ButtonValidate.Location = new Point(306, 24);
            // 
            // labelExcelCategory
            // 
            labelExcelCategory.AutoSize = true;
            labelExcelCategory.Location = new Point(6, 90);
            labelExcelCategory.Name = "labelExcelCategory";
            labelExcelCategory.Size = new Size(85, 15);
            labelExcelCategory.TabIndex = 5;
            labelExcelCategory.Text = "Excel Category";
            // 
            // comboBoxExcelCategory
            // 
            comboBoxExcelCategory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxExcelCategory.BackColor = Color.LightGreen;
            comboBoxExcelCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxExcelCategory.FormattingEnabled = true;
            comboBoxExcelCategory.Location = new Point(137, 87);
            comboBoxExcelCategory.Name = "comboBoxExcelCategory";
            comboBoxExcelCategory.Size = new Size(218, 23);
            comboBoxExcelCategory.TabIndex = 4;
            // 
            // checkBoxDeleteExistingItems
            // 
            checkBoxDeleteExistingItems.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBoxDeleteExistingItems.AutoSize = true;
            checkBoxDeleteExistingItems.Location = new Point(185, 116);
            checkBoxDeleteExistingItems.Name = "checkBoxDeleteExistingItems";
            checkBoxDeleteExistingItems.RightToLeft = RightToLeft.Yes;
            checkBoxDeleteExistingItems.Size = new Size(170, 19);
            checkBoxDeleteExistingItems.TabIndex = 5;
            checkBoxDeleteExistingItems.Text = "Delete Existing Server Items";
            checkBoxDeleteExistingItems.UseVisualStyleBackColor = true;
            // 
            // labelLocalPath
            // 
            labelLocalPath.AutoSize = true;
            labelLocalPath.Location = new Point(6, 144);
            labelLocalPath.Name = "labelLocalPath";
            labelLocalPath.Size = new Size(62, 15);
            labelLocalPath.TabIndex = 5;
            labelLocalPath.Text = "Local Path";
            // 
            // textBoxXlFilePath
            // 
            textBoxXlFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxXlFilePath.BackColor = Color.LightGreen;
            textBoxXlFilePath.BorderStyle = BorderStyle.FixedSingle;
            textBoxXlFilePath.Location = new Point(137, 141);
            textBoxXlFilePath.Name = "textBoxXlFilePath";
            textBoxXlFilePath.ReadOnly = true;
            textBoxXlFilePath.Size = new Size(218, 23);
            textBoxXlFilePath.TabIndex = 6;
            textBoxXlFilePath.TextAlign = HorizontalAlignment.Right;
            // 
            // ExcelUploadControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGreen;
            Controls.Add(textBoxXlFilePath);
            Controls.Add(checkBoxDeleteExistingItems);
            Controls.Add(labelLocalPath);
            Controls.Add(comboBoxExcelCategory);
            Controls.Add(labelExcelCategory);
            MinimumSize = new Size(315, 103);
            Name = "ExcelUploadControl";
            Section = "Excel";
            Size = new Size(358, 167);
            Title = "Upload Excel File";
            TitleColor = Color.SeaGreen;
            Controls.SetChildIndex(ButtonRun, 0);
            Controls.SetChildIndex(ButtonValidate, 0);
            Controls.SetChildIndex(labelSectionTitle, 0);
            Controls.SetChildIndex(buttonAddOrRemove, 0);
            Controls.SetChildIndex(CheckBoxEnableAction, 0);
            Controls.SetChildIndex(ProgressBarControl, 0);
            Controls.SetChildIndex(labelExcelCategory, 0);
            Controls.SetChildIndex(comboBoxExcelCategory, 0);
            Controls.SetChildIndex(labelLocalPath, 0);
            Controls.SetChildIndex(checkBoxDeleteExistingItems, 0);
            Controls.SetChildIndex(textBoxXlFilePath, 0);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelExcelCategory;
        private ComboBox comboBoxExcelCategory;
        private CheckBox checkBoxDeleteExistingItems;
        private Label labelLocalPath;
        private TextBox textBoxXlFilePath;
    }
}
