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
            SynchronizationContext synchronizationContext1 = new SynchronizationContext();
            splitContainerMain = new SplitContainer();
            projectOpenSaveControl = new Controls.ProjectOpenSaveControl();
            signInControl = new Controls.SignInControl();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerMain
            // 
            splitContainerMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerMain.Location = new Point(0, 110);
            splitContainerMain.Name = "splitContainerMain";
            splitContainerMain.Size = new Size(721, 384);
            splitContainerMain.SplitterDistance = 398;
            splitContainerMain.TabIndex = 2;
            // 
            // projectOpenSaveControl
            // 
            projectOpenSaveControl.Location = new Point(329, 1);
            projectOpenSaveControl.Name = "projectOpenSaveControl";
            projectOpenSaveControl.Size = new Size(382, 86);
            projectOpenSaveControl.TabIndex = 1;
            // 
            // signInControl
            // 
            signInControl.Location = new Point(3, 2);
            signInControl.MinimumSize = new Size(246, 25);
            signInControl.Name = "signInControl";
            signInControl.SignInControlModel = null;
            signInControl.Size = new Size(320, 85);
            signInControl.SynchronizationContext = synchronizationContext1;
            signInControl.TabIndex = 0;
            // 
            // WaterSightParentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(722, 490);
            Controls.Add(signInControl);
            Controls.Add(projectOpenSaveControl);
            Controls.Add(splitContainerMain);
            Name = "WaterSightParentForm";
            Text = "WaterSightParentForm";
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer splitContainerMain;
        private Controls.ProjectOpenSaveControl projectOpenSaveControl;
        private Controls.SignInControl signInControl;
    }
}