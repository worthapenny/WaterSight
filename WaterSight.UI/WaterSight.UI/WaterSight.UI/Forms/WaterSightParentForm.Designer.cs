namespace WaterSight.UI.Forms
{
    partial class WaterSightParentForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SynchronizationContext synchronizationContext1 = new SynchronizationContext();
            projectOpenSaveControl = new Controls.ProjectOpenSaveControl();
            signInControl = new Controls.SignInControl();
            tabControlMain = new TabControl();
            tabPageWaterSight = new TabPage();
            splitContainerActions = new SplitContainer();
            flowLayoutPanelActions = new FlowLayoutPanel();
            flowLayoutPanelActionsRepo = new FlowLayoutPanel();
            dragableControlBase2 = new Controls.DragableControlBase();
            dragableControlBase1 = new Controls.DragableControlBase();
            excelUploadControl1 = new Controls.Modules.WaterSightModules.ExcelUploadControl();
            waterModelControl1 = new Controls.Modules.WaterSightModules.WaterModelControl();
            tabPageLog = new TabPage();
            richTextBoxLog = new RichTextBox();
            tabPageWaterModel = new TabPage();
            projectOpenSaveControlWaterModelTab = new Controls.ProjectOpenSaveControl();
            waterModelControl2 = new Controls.Modules.WaterSightModules.WaterModelControl();
            tabControlMain.SuspendLayout();
            tabPageWaterSight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerActions).BeginInit();
            splitContainerActions.Panel1.SuspendLayout();
            splitContainerActions.Panel2.SuspendLayout();
            splitContainerActions.SuspendLayout();
            flowLayoutPanelActionsRepo.SuspendLayout();
            tabPageLog.SuspendLayout();
            tabPageWaterModel.SuspendLayout();
            SuspendLayout();
            // 
            // projectOpenSaveControl
            // 
            projectOpenSaveControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            projectOpenSaveControl.Location = new Point(332, 6);
            projectOpenSaveControl.Name = "projectOpenSaveControl";
            projectOpenSaveControl.Size = new Size(565, 86);
            projectOpenSaveControl.TabIndex = 1;
            // 
            // signInControl
            // 
            signInControl.Location = new Point(6, 6);
            signInControl.MinimumSize = new Size(246, 25);
            signInControl.Name = "signInControl";
            signInControl.SignInControlModel = null;
            signInControl.Size = new Size(320, 85);
            signInControl.SynchronizationContext = synchronizationContext1;
            signInControl.TabIndex = 0;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageWaterSight);
            tabControlMain.Controls.Add(tabPageLog);
            tabControlMain.Controls.Add(tabPageWaterModel);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(913, 692);
            tabControlMain.TabIndex = 3;
            // 
            // tabPageWaterSight
            // 
            tabPageWaterSight.Controls.Add(splitContainerActions);
            tabPageWaterSight.Controls.Add(projectOpenSaveControl);
            tabPageWaterSight.Controls.Add(signInControl);
            tabPageWaterSight.Location = new Point(4, 24);
            tabPageWaterSight.Name = "tabPageWaterSight";
            tabPageWaterSight.Padding = new Padding(3);
            tabPageWaterSight.Size = new Size(905, 664);
            tabPageWaterSight.TabIndex = 0;
            tabPageWaterSight.Text = "🕸️ WaterSight";
            tabPageWaterSight.UseVisualStyleBackColor = true;
            // 
            // splitContainerActions
            // 
            splitContainerActions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerActions.Location = new Point(8, 98);
            splitContainerActions.Name = "splitContainerActions";
            // 
            // splitContainerActions.Panel1
            // 
            splitContainerActions.Panel1.Controls.Add(flowLayoutPanelActions);
            splitContainerActions.Panel1.DragDrop += splitContainerActions_Panel1_DragDrop;
            splitContainerActions.Panel1.DragOver += splitContainerActions_Panel1_DragOver;
            // 
            // splitContainerActions.Panel2
            // 
            splitContainerActions.Panel2.Controls.Add(flowLayoutPanelActionsRepo);
            splitContainerActions.Size = new Size(897, 566);
            splitContainerActions.SplitterDistance = 399;
            splitContainerActions.TabIndex = 4;
            // 
            // flowLayoutPanelActions
            // 
            flowLayoutPanelActions.Dock = DockStyle.Fill;
            flowLayoutPanelActions.Location = new Point(0, 0);
            flowLayoutPanelActions.Name = "flowLayoutPanelActions";
            flowLayoutPanelActions.Size = new Size(399, 566);
            flowLayoutPanelActions.TabIndex = 0;
            // 
            // flowLayoutPanelActionsRepo
            // 
            flowLayoutPanelActionsRepo.Controls.Add(dragableControlBase2);
            flowLayoutPanelActionsRepo.Controls.Add(dragableControlBase1);
            flowLayoutPanelActionsRepo.Controls.Add(excelUploadControl1);
            flowLayoutPanelActionsRepo.Controls.Add(waterModelControl1);
            flowLayoutPanelActionsRepo.Dock = DockStyle.Fill;
            flowLayoutPanelActionsRepo.Location = new Point(0, 0);
            flowLayoutPanelActionsRepo.Name = "flowLayoutPanelActionsRepo";
            flowLayoutPanelActionsRepo.Size = new Size(494, 566);
            flowLayoutPanelActionsRepo.TabIndex = 5;
            // 
            // dragableControlBase2
            // 
            dragableControlBase2.BackColor = Color.LightSalmon;
            dragableControlBase2.Location = new Point(3, 3);
            dragableControlBase2.MinimumSize = new Size(221, 75);
            dragableControlBase2.Name = "dragableControlBase2";
            dragableControlBase2.Section = "Section?";
            dragableControlBase2.Size = new Size(221, 75);
            dragableControlBase2.TabIndex = 3;
            dragableControlBase2.Title = "Title?";
            dragableControlBase2.TitleColor = Color.Salmon;
            // 
            // dragableControlBase1
            // 
            dragableControlBase1.BackColor = Color.LightGreen;
            dragableControlBase1.Location = new Point(230, 3);
            dragableControlBase1.MinimumSize = new Size(219, 75);
            dragableControlBase1.Name = "dragableControlBase1";
            dragableControlBase1.Section = "Section? 22";
            dragableControlBase1.Size = new Size(219, 75);
            dragableControlBase1.TabIndex = 4;
            dragableControlBase1.Title = "Title? 2";
            dragableControlBase1.TitleColor = Color.MediumSeaGreen;
            // 
            // excelUploadControl1
            // 
            excelUploadControl1.BackColor = Color.LightGreen;
            excelUploadControl1.DeleteExistingServerItems = false;
            excelUploadControl1.ExcelFilePath = "No project is active";
            excelUploadControl1.Location = new Point(3, 84);
            excelUploadControl1.MinimumSize = new Size(358, 167);
            excelUploadControl1.Name = "excelUploadControl1";
            excelUploadControl1.Section = "Excel";
            excelUploadControl1.Size = new Size(358, 167);
            excelUploadControl1.TabIndex = 5;
            excelUploadControl1.Title = "Upload Excel File";
            excelUploadControl1.TitleColor = Color.SeaGreen;
            // 
            // waterModelControl1
            // 
            waterModelControl1.BackColor = Color.LightSteelBlue;
            waterModelControl1.Location = new Point(3, 257);
            waterModelControl1.MinimumSize = new Size(416, 280);
            waterModelControl1.Name = "waterModelControl1";
            waterModelControl1.Section = "Hydraulic Model";
            waterModelControl1.Size = new Size(416, 280);
            waterModelControl1.TabIndex = 6;
            waterModelControl1.Title = "Upload Model";
            waterModelControl1.TitleColor = Color.SteelBlue;
            // 
            // tabPageLog
            // 
            tabPageLog.Controls.Add(richTextBoxLog);
            tabPageLog.Location = new Point(4, 24);
            tabPageLog.Name = "tabPageLog";
            tabPageLog.Padding = new Padding(3);
            tabPageLog.Size = new Size(192, 72);
            tabPageLog.TabIndex = 1;
            tabPageLog.Text = "📒 Log";
            tabPageLog.UseVisualStyleBackColor = true;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.BackColor = SystemColors.Info;
            richTextBoxLog.BorderStyle = BorderStyle.None;
            richTextBoxLog.Dock = DockStyle.Fill;
            richTextBoxLog.Location = new Point(3, 3);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.Size = new Size(186, 66);
            richTextBoxLog.TabIndex = 0;
            richTextBoxLog.Text = "";
            // 
            // tabPageWaterModel
            // 
            tabPageWaterModel.Controls.Add(waterModelControl2);
            tabPageWaterModel.Controls.Add(projectOpenSaveControlWaterModelTab);
            tabPageWaterModel.Location = new Point(4, 24);
            tabPageWaterModel.Name = "tabPageWaterModel";
            tabPageWaterModel.Size = new Size(905, 664);
            tabPageWaterModel.TabIndex = 2;
            tabPageWaterModel.Text = "Water Model";
            tabPageWaterModel.UseVisualStyleBackColor = true;
            // 
            // projectOpenSaveControlWaterModelTab
            // 
            projectOpenSaveControlWaterModelTab.Location = new Point(3, 3);
            projectOpenSaveControlWaterModelTab.Name = "projectOpenSaveControlWaterModelTab";
            projectOpenSaveControlWaterModelTab.Size = new Size(878, 86);
            projectOpenSaveControlWaterModelTab.TabIndex = 0;
            // 
            // waterModelControl2
            // 
            waterModelControl2.BackColor = Color.LightSteelBlue;
            waterModelControl2.Location = new Point(8, 105);
            waterModelControl2.MinimumSize = new Size(416, 280);
            waterModelControl2.Name = "waterModelControl2";
            waterModelControl2.Section = "Water Model";
            waterModelControl2.Size = new Size(779, 280);
            waterModelControl2.TabIndex = 1;
            waterModelControl2.Title = "Upload Model";
            waterModelControl2.TitleColor = Color.SteelBlue;
            // 
            // WaterSightParentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(913, 692);
            Controls.Add(tabControlMain);
            Name = "WaterSightParentForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WaterSightParentForm";
            tabControlMain.ResumeLayout(false);
            tabPageWaterSight.ResumeLayout(false);
            splitContainerActions.Panel1.ResumeLayout(false);
            splitContainerActions.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerActions).EndInit();
            splitContainerActions.ResumeLayout(false);
            flowLayoutPanelActionsRepo.ResumeLayout(false);
            tabPageLog.ResumeLayout(false);
            tabPageWaterModel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Controls.ProjectOpenSaveControl projectOpenSaveControl;
        private Controls.SignInControl signInControl;
        private TabControl tabControlMain;
        private TabPage tabPageWaterSight;
        private TabPage tabPageLog;
        private RichTextBox richTextBoxLog;
        private Controls.DragableControlBase dragableControlBase2;
        private SplitContainer splitContainerActions;
        private Controls.DragableControlBase dragableControlBase1;
        private FlowLayoutPanel flowLayoutPanelActions;
        private FlowLayoutPanel flowLayoutPanelActionsRepo;
        private Controls.Modules.WaterSightModules.ExcelUploadControl excelUploadControl1;
        private Controls.Modules.WaterSightModules.WaterModelControl waterModelControl1;
        private TabPage tabPageWaterModel;
        private Controls.ProjectOpenSaveControl projectOpenSaveControlWaterModelTab;
        private Controls.Modules.WaterSightModules.WaterModelControl waterModelControl2;
    }
}