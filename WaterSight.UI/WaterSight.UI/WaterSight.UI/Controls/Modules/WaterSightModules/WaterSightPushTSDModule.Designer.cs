namespace WaterSight.UI.Controls.Modules.WaterSightModules
{
    partial class WaterSightPushTSDModule
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
            labelPathTitle = new Label();
            moduleControlBase = new ModuleControlBase();
            textBoxDatabaseFileDir = new TextBox();
            comboBoxDatabaseFileName = new ComboBox();
            labelDbTableName = new Label();
            labelTagColName = new Label();
            labelValueColName = new Label();
            labelTimestampColName = new Label();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            comboBox3 = new ComboBox();
            comboBox4 = new ComboBox();
            buttonReadDbStructure = new Button();
            SuspendLayout();
            // 
            // labelPathTitle
            // 
            labelPathTitle.AutoSize = true;
            labelPathTitle.Location = new Point(12, 54);
            labelPathTitle.Name = "labelPathTitle";
            labelPathTitle.Size = new Size(147, 15);
            labelPathTitle.TabIndex = 0;
            labelPathTitle.Text = "Time Series Database Path:";
            // 
            // moduleControlBase
            // 
            moduleControlBase.Dock = DockStyle.Fill;
            moduleControlBase.Label = "Push Time Series Data";
            moduleControlBase.Location = new Point(0, 0);
            moduleControlBase.Name = "moduleControlBase";
            moduleControlBase.Size = new Size(391, 269);
            moduleControlBase.TabIndex = 1;
            // 
            // textBoxDatabaseFileDir
            // 
            textBoxDatabaseFileDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxDatabaseFileDir.Location = new Point(12, 72);
            textBoxDatabaseFileDir.Name = "textBoxDatabaseFileDir";
            textBoxDatabaseFileDir.ReadOnly = true;
            textBoxDatabaseFileDir.Size = new Size(240, 23);
            textBoxDatabaseFileDir.TabIndex = 2;
            // 
            // comboBoxDatabaseFileName
            // 
            comboBoxDatabaseFileName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBoxDatabaseFileName.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDatabaseFileName.FormattingEnabled = true;
            comboBoxDatabaseFileName.Location = new Point(258, 72);
            comboBoxDatabaseFileName.Name = "comboBoxDatabaseFileName";
            comboBoxDatabaseFileName.Size = new Size(121, 23);
            comboBoxDatabaseFileName.TabIndex = 3;
            // 
            // labelDbTableName
            // 
            labelDbTableName.AutoSize = true;
            labelDbTableName.Location = new Point(12, 153);
            labelDbTableName.Name = "labelDbTableName";
            labelDbTableName.Size = new Size(69, 15);
            labelDbTableName.TabIndex = 4;
            labelDbTableName.Text = "Table Name";
            // 
            // labelTagColName
            // 
            labelTagColName.AutoSize = true;
            labelTagColName.Location = new Point(12, 182);
            labelTagColName.Name = "labelTagColName";
            labelTagColName.Size = new Size(84, 15);
            labelTagColName.TabIndex = 4;
            labelTagColName.Text = "Tag Col. Name";
            // 
            // labelValueColName
            // 
            labelValueColName.AutoSize = true;
            labelValueColName.Location = new Point(12, 211);
            labelValueColName.Name = "labelValueColName";
            labelValueColName.Size = new Size(94, 15);
            labelValueColName.TabIndex = 4;
            labelValueColName.Text = "Value Col. Name";
            // 
            // labelTimestampColName
            // 
            labelTimestampColName.AutoSize = true;
            labelTimestampColName.Location = new Point(12, 240);
            labelTimestampColName.Name = "labelTimestampColName";
            labelTimestampColName.Size = new Size(125, 15);
            labelTimestampColName.TabIndex = 4;
            labelTimestampColName.Text = "Timestamp Col. Name";
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(258, 150);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 3;
            // 
            // comboBox2
            // 
            comboBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(258, 179);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(121, 23);
            comboBox2.TabIndex = 3;
            // 
            // comboBox3
            // 
            comboBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(258, 208);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(121, 23);
            comboBox3.TabIndex = 3;
            // 
            // comboBox4
            // 
            comboBox4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4.FormattingEnabled = true;
            comboBox4.Location = new Point(258, 237);
            comboBox4.Name = "comboBox4";
            comboBox4.Size = new Size(121, 23);
            comboBox4.TabIndex = 3;
            // 
            // buttonReadDbStructure
            // 
            buttonReadDbStructure.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonReadDbStructure.Location = new Point(258, 101);
            buttonReadDbStructure.Name = "buttonReadDbStructure";
            buttonReadDbStructure.Size = new Size(121, 23);
            buttonReadDbStructure.TabIndex = 5;
            buttonReadDbStructure.Text = "Get Structure";
            buttonReadDbStructure.UseVisualStyleBackColor = true;
            // 
            // WaterSightPushTSDModule
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(buttonReadDbStructure);
            Controls.Add(labelTimestampColName);
            Controls.Add(labelValueColName);
            Controls.Add(labelTagColName);
            Controls.Add(labelDbTableName);
            Controls.Add(comboBox4);
            Controls.Add(comboBox3);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(comboBoxDatabaseFileName);
            Controls.Add(textBoxDatabaseFileDir);
            Controls.Add(labelPathTitle);
            Controls.Add(moduleControlBase);
            Name = "WaterSightPushTSDModule";
            Size = new Size(391, 269);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelPathTitle;
        private ModuleControlBase moduleControlBase;
        private TextBox textBoxDatabaseFileDir;
        private ComboBox comboBoxDatabaseFileName;
        private Label labelDbTableName;
        private Label labelTagColName;
        private Label labelValueColName;
        private Label labelTimestampColName;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private ComboBox comboBox3;
        private ComboBox comboBox4;
        private Button buttonReadDbStructure;
    }
}
