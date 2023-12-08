namespace WaterSight.UI.Controls.Modules
{
    partial class ModuleControlBase
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
            buttonClose = new Button();
            buttonToggleSize = new Button();
            buttonDown = new Button();
            buttonMoveUp = new Button();
            labelTitle = new Label();
            groupBox = new GroupBox();
            SuspendLayout();
            // 
            // buttonClose
            // 
            buttonClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonClose.Location = new Point(280, 3);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(23, 23);
            buttonClose.TabIndex = 0;
            buttonClose.Text = "x";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // buttonToggleSize
            // 
            buttonToggleSize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonToggleSize.Location = new Point(258, 3);
            buttonToggleSize.Name = "buttonToggleSize";
            buttonToggleSize.Size = new Size(23, 23);
            buttonToggleSize.TabIndex = 1;
            buttonToggleSize.Text = "□";
            buttonToggleSize.UseVisualStyleBackColor = true;
            buttonToggleSize.Click += buttonToggleSize_Click;
            // 
            // buttonDown
            // 
            buttonDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDown.Location = new Point(229, 3);
            buttonDown.Name = "buttonDown";
            buttonDown.Size = new Size(23, 23);
            buttonDown.TabIndex = 2;
            buttonDown.Text = "▼";
            buttonDown.UseVisualStyleBackColor = true;
            buttonDown.Click += buttonDown_Click;
            // 
            // buttonMoveUp
            // 
            buttonMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonMoveUp.Location = new Point(206, 3);
            buttonMoveUp.Name = "buttonMoveUp";
            buttonMoveUp.Size = new Size(23, 23);
            buttonMoveUp.TabIndex = 3;
            buttonMoveUp.Text = "▲";
            buttonMoveUp.UseVisualStyleBackColor = true;
            buttonMoveUp.Click += buttonMoveUp_Click;
            // 
            // labelTitle
            // 
            labelTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelTitle.Location = new Point(3, 5);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(197, 28);
            labelTitle.TabIndex = 4;
            labelTitle.Text = "label?";
            // 
            // groupBox
            // 
            groupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox.Location = new Point(3, 34);
            groupBox.Name = "groupBox";
            groupBox.Size = new Size(297, 318);
            groupBox.TabIndex = 5;
            groupBox.TabStop = false;
            // 
            // ModuleControlBase
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox);
            Controls.Add(labelTitle);
            Controls.Add(buttonMoveUp);
            Controls.Add(buttonDown);
            Controls.Add(buttonToggleSize);
            Controls.Add(buttonClose);
            Name = "ModuleControlBase";
            Size = new Size(303, 349);
            ResumeLayout(false);
        }

        #endregion

        private Button buttonClose;
        private Button buttonToggleSize;
        private Button buttonDown;
        private Button buttonMoveUp;
        private Label labelTitle;
        private GroupBox groupBox;
    }
}
