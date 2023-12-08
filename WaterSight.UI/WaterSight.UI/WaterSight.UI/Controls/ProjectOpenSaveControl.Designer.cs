namespace WaterSight.UI.Controls
{
    partial class ProjectOpenSaveControl
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
            buttonNewProject = new Button();
            buttonBrowseProjectPath = new Button();
            textBoxProjectPath = new TextBox();
            buttonLoadDigitalTwins = new Button();
            comboBoxDigitalTwins = new ComboBox();
            groupBoxProject = new GroupBox();
            groupBoxProject.SuspendLayout();
            SuspendLayout();
            // 
            // buttonNewProject
            // 
            buttonNewProject.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonNewProject.Location = new Point(275, 49);
            buttonNewProject.Name = "buttonNewProject";
            buttonNewProject.Size = new Size(95, 25);
            buttonNewProject.TabIndex = 6;
            buttonNewProject.Text = "&New";
            buttonNewProject.UseVisualStyleBackColor = true;
            // 
            // buttonBrowseProjectPath
            // 
            buttonBrowseProjectPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonBrowseProjectPath.Location = new Point(275, 22);
            buttonBrowseProjectPath.Name = "buttonBrowseProjectPath";
            buttonBrowseProjectPath.Size = new Size(95, 25);
            buttonBrowseProjectPath.TabIndex = 5;
            buttonBrowseProjectPath.Text = "&Select Project";
            buttonBrowseProjectPath.UseVisualStyleBackColor = true;
            buttonBrowseProjectPath.Click += buttonBrowseProjectPath_Click;
            // 
            // textBoxProjectPath
            // 
            textBoxProjectPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxProjectPath.Location = new Point(5, 22);
            textBoxProjectPath.Name = "textBoxProjectPath";
            textBoxProjectPath.Size = new Size(264, 23);
            textBoxProjectPath.TabIndex = 4;
            // 
            // buttonLoadDigitalTwins
            // 
            buttonLoadDigitalTwins.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            buttonLoadDigitalTwins.Location = new Point(5, 48);
            buttonLoadDigitalTwins.Name = "buttonLoadDigitalTwins";
            buttonLoadDigitalTwins.Size = new Size(31, 23);
            buttonLoadDigitalTwins.TabIndex = 7;
            buttonLoadDigitalTwins.Text = "↻";
            buttonLoadDigitalTwins.TextAlign = ContentAlignment.TopCenter;
            buttonLoadDigitalTwins.UseVisualStyleBackColor = true;
            // 
            // comboBoxDigitalTwins
            // 
            comboBoxDigitalTwins.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxDigitalTwins.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDigitalTwins.FormattingEnabled = true;
            comboBoxDigitalTwins.Location = new Point(42, 48);
            comboBoxDigitalTwins.Name = "comboBoxDigitalTwins";
            comboBoxDigitalTwins.Size = new Size(227, 23);
            comboBoxDigitalTwins.TabIndex = 8;
            comboBoxDigitalTwins.SelectedIndexChanged += comboBoxDigitalTwins_SelectedIndexChanged;
            // 
            // groupBoxProject
            // 
            groupBoxProject.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxProject.Controls.Add(comboBoxDigitalTwins);
            groupBoxProject.Controls.Add(textBoxProjectPath);
            groupBoxProject.Controls.Add(buttonLoadDigitalTwins);
            groupBoxProject.Controls.Add(buttonBrowseProjectPath);
            groupBoxProject.Controls.Add(buttonNewProject);
            groupBoxProject.Location = new Point(3, 3);
            groupBoxProject.Name = "groupBoxProject";
            groupBoxProject.Size = new Size(376, 80);
            groupBoxProject.TabIndex = 9;
            groupBoxProject.TabStop = false;
            groupBoxProject.Text = "Project";
            // 
            // ProjectOpenSaveControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBoxProject);
            Name = "ProjectOpenSaveControl";
            Size = new Size(382, 86);
            groupBoxProject.ResumeLayout(false);
            groupBoxProject.PerformLayout();
            ResumeLayout(false);
        }



        #endregion

        private Button buttonNewProject;
        private Button buttonBrowseProjectPath;
        private TextBox textBoxProjectPath;
        private Button buttonLoadDigitalTwins;
        private ComboBox comboBoxDigitalTwins;
        private GroupBox groupBoxProject;
    }
}
