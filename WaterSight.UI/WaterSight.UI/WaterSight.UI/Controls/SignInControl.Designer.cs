namespace WaterSight.UI.Controls
{
    partial class SignInControl
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
            buttonSignIn = new Button();
            radioButtonDev = new RadioButton();
            radioButtonQA = new RadioButton();
            radioButtonProd = new RadioButton();
            groupBoxSignIn = new GroupBox();
            checkBoxEuRegion = new CheckBox();
            groupBoxSignIn.SuspendLayout();
            SuspendLayout();
            // 
            // buttonSignIn
            // 
            buttonSignIn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSignIn.Location = new Point(212, 43);
            buttonSignIn.Name = "buttonSignIn";
            buttonSignIn.Size = new Size(95, 25);
            buttonSignIn.TabIndex = 0;
            buttonSignIn.Text = "Sign In";
            buttonSignIn.UseVisualStyleBackColor = true;
            // 
            // radioButtonDev
            // 
            radioButtonDev.AutoSize = true;
            radioButtonDev.Location = new Point(12, 22);
            radioButtonDev.Name = "radioButtonDev";
            radioButtonDev.Size = new Size(45, 19);
            radioButtonDev.TabIndex = 1;
            radioButtonDev.Text = "&Dev";
            radioButtonDev.UseVisualStyleBackColor = true;
            radioButtonDev.CheckedChanged += radioButtonDev_CheckedChanged;
            // 
            // radioButtonQA
            // 
            radioButtonQA.AutoSize = true;
            radioButtonQA.Location = new Point(63, 22);
            radioButtonQA.Name = "radioButtonQA";
            radioButtonQA.Size = new Size(42, 19);
            radioButtonQA.TabIndex = 2;
            radioButtonQA.Text = "&QA";
            radioButtonQA.UseVisualStyleBackColor = true;
            radioButtonQA.CheckedChanged += radioButtonQA_CheckedChanged;
            // 
            // radioButtonProd
            // 
            radioButtonProd.AutoSize = true;
            radioButtonProd.Checked = true;
            radioButtonProd.Location = new Point(120, 22);
            radioButtonProd.Name = "radioButtonProd";
            radioButtonProd.Size = new Size(50, 19);
            radioButtonProd.TabIndex = 3;
            radioButtonProd.TabStop = true;
            radioButtonProd.Text = "&Prod";
            radioButtonProd.UseVisualStyleBackColor = true;
            radioButtonProd.CheckedChanged += radioButtonProd_CheckedChanged;
            // 
            // groupBoxSignIn
            // 
            groupBoxSignIn.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxSignIn.Controls.Add(checkBoxEuRegion);
            groupBoxSignIn.Controls.Add(radioButtonQA);
            groupBoxSignIn.Controls.Add(buttonSignIn);
            groupBoxSignIn.Controls.Add(radioButtonProd);
            groupBoxSignIn.Controls.Add(radioButtonDev);
            groupBoxSignIn.Location = new Point(3, 3);
            groupBoxSignIn.Name = "groupBoxSignIn";
            groupBoxSignIn.Size = new Size(313, 80);
            groupBoxSignIn.TabIndex = 4;
            groupBoxSignIn.TabStop = false;
            groupBoxSignIn.Text = "Sign In";
            // 
            // checkBoxEuRegion
            // 
            checkBoxEuRegion.AutoSize = true;
            checkBoxEuRegion.Location = new Point(12, 47);
            checkBoxEuRegion.Name = "checkBoxEuRegion";
            checkBoxEuRegion.Size = new Size(80, 19);
            checkBoxEuRegion.TabIndex = 4;
            checkBoxEuRegion.Text = "&EU Region";
            checkBoxEuRegion.UseVisualStyleBackColor = true;
            checkBoxEuRegion.CheckedChanged += checkBoxEuRegion_CheckedChanged;
            // 
            // SignInControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBoxSignIn);
            MinimumSize = new Size(246, 25);
            Name = "SignInControl";
            Size = new Size(320, 85);
            groupBoxSignIn.ResumeLayout(false);
            groupBoxSignIn.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button buttonSignIn;
        private RadioButton radioButtonDev;
        private RadioButton radioButtonQA;
        private RadioButton radioButtonProd;
        private GroupBox groupBoxSignIn;
        private CheckBox checkBoxEuRegion;
    }
}
