namespace WaterSight.UI.Controls
{
    partial class DragableControlBase
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
            components = new System.ComponentModel.Container();
            labelSectionTitle = new Label();
            buttonAddOrRemove = new Button();
            CheckBoxEnableAction = new CheckBox();
            ProgressBarControl = new ProgressBar();
            contextMenuStrip = new ContextMenuStrip(components);
            ButtonRun = new Button();
            ButtonValidate = new Button();
            buttonSave = new Button();
            SuspendLayout();
            // 
            // labelSectionTitle
            // 
            labelSectionTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelSectionTitle.Location = new Point(0, 0);
            labelSectionTitle.Name = "labelSectionTitle";
            labelSectionTitle.Size = new Size(342, 23);
            labelSectionTitle.TabIndex = 3;
            labelSectionTitle.Text = "Section | Title";
            labelSectionTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // buttonAddOrRemove
            // 
            buttonAddOrRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonAddOrRemove.FlatStyle = FlatStyle.Flat;
            buttonAddOrRemove.ForeColor = Color.FromArgb(192, 0, 0);
            buttonAddOrRemove.Location = new Point(345, 0);
            buttonAddOrRemove.Margin = new Padding(0);
            buttonAddOrRemove.Name = "buttonAddOrRemove";
            buttonAddOrRemove.Size = new Size(23, 23);
            buttonAddOrRemove.TabIndex = 2;
            buttonAddOrRemove.Text = "X";
            buttonAddOrRemove.UseVisualStyleBackColor = false;
            buttonAddOrRemove.Click += buttonClose_Click;
            // 
            // CheckBoxEnableAction
            // 
            CheckBoxEnableAction.AutoSize = true;
            CheckBoxEnableAction.Location = new Point(3, 28);
            CheckBoxEnableAction.Name = "CheckBoxEnableAction";
            CheckBoxEnableAction.Size = new Size(66, 19);
            CheckBoxEnableAction.TabIndex = 4;
            CheckBoxEnableAction.Text = "Enable?";
            CheckBoxEnableAction.UseVisualStyleBackColor = true;
            // 
            // ProgressBarControl
            // 
            ProgressBarControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ProgressBarControl.Location = new Point(72, 28);
            ProgressBarControl.Name = "ProgressBarControl";
            ProgressBarControl.Size = new Size(212, 19);
            ProgressBarControl.TabIndex = 5;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(61, 4);
            // 
            // ButtonRun
            // 
            ButtonRun.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ButtonRun.FlatStyle = FlatStyle.Flat;
            ButtonRun.Location = new Point(345, 25);
            ButtonRun.Name = "ButtonRun";
            ButtonRun.Size = new Size(23, 23);
            ButtonRun.TabIndex = 6;
            ButtonRun.UseVisualStyleBackColor = true;
            // 
            // ButtonValidate
            // 
            ButtonValidate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ButtonValidate.FlatStyle = FlatStyle.Flat;
            ButtonValidate.Location = new Point(319, 25);
            ButtonValidate.Name = "ButtonValidate";
            ButtonValidate.Size = new Size(23, 23);
            ButtonValidate.TabIndex = 6;
            ButtonValidate.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            buttonSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.Location = new Point(290, 25);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(23, 23);
            buttonSave.TabIndex = 6;
            buttonSave.UseVisualStyleBackColor = true;
            // 
            // DragableControlBase
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ContextMenuStrip = contextMenuStrip;
            Controls.Add(buttonSave);
            Controls.Add(ButtonValidate);
            Controls.Add(ButtonRun);
            Controls.Add(ProgressBarControl);
            Controls.Add(CheckBoxEnableAction);
            Controls.Add(buttonAddOrRemove);
            Controls.Add(labelSectionTitle);
            Name = "DragableControlBase";
            Size = new Size(371, 75);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public ProgressBar ProgressBarControl;
        public CheckBox CheckBoxEnableAction;
        private ProgressBar progressBarControl;
        public Label labelSectionTitle;
        public Button buttonAddOrRemove;
        private ContextMenuStrip contextMenuStrip;
        public Button ButtonRun;
        public Button ButtonValidate;
        public Button buttonSave;
    }
}
