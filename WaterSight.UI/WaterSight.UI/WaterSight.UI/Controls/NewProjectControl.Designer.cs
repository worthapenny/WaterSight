namespace WaterSight.UI.Controls;

partial class NewProjectControl
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
        labelDtInfo = new Label();
        textBoxDtInfo = new TextBox();
        groupBoxNameDescription = new GroupBox();
        textBoxDescription = new TextBox();
        labelDescription = new Label();
        textBoxShortName = new TextBox();
        labelShortName = new Label();
        textBoxName = new TextBox();
        labelDtName = new Label();
        groupBoxGeo = new GroupBox();
        linkLabelGoogleMap = new LinkLabel();
        linkLabelTimeZone = new LinkLabel();
        textBoxTimeZone = new TextBox();
        label1 = new Label();
        textBoxLng = new TextBox();
        labelLonitude = new Label();
        textBoxLat = new TextBox();
        labelLatitude = new Label();
        textBoxEpsgCode = new TextBox();
        labelEpsgCode = new Label();
        buttonWsLoad = new Button();
        buttonWsSave = new Button();
        buttonFileSave = new Button();
        groupBoxUnit = new GroupBox();
        radioButtonUnitSI = new RadioButton();
        radioButtonUnitUS = new RadioButton();
        groupBoxNameDescription.SuspendLayout();
        groupBoxGeo.SuspendLayout();
        groupBoxUnit.SuspendLayout();
        SuspendLayout();
        // 
        // labelDtInfo
        // 
        labelDtInfo.AutoSize = true;
        labelDtInfo.Location = new Point(3, 6);
        labelDtInfo.Name = "labelDtInfo";
        labelDtInfo.Size = new Size(92, 15);
        labelDtInfo.TabIndex = 0;
        labelDtInfo.Text = "Digital Twin Info";
        // 
        // textBoxDtInfo
        // 
        textBoxDtInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxDtInfo.Location = new Point(160, 3);
        textBoxDtInfo.Name = "textBoxDtInfo";
        textBoxDtInfo.ReadOnly = true;
        textBoxDtInfo.Size = new Size(246, 23);
        textBoxDtInfo.TabIndex = 0;
        // 
        // groupBoxNameDescription
        // 
        groupBoxNameDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxNameDescription.Controls.Add(textBoxDescription);
        groupBoxNameDescription.Controls.Add(labelDescription);
        groupBoxNameDescription.Controls.Add(textBoxShortName);
        groupBoxNameDescription.Controls.Add(labelShortName);
        groupBoxNameDescription.Controls.Add(textBoxName);
        groupBoxNameDescription.Controls.Add(labelDtName);
        groupBoxNameDescription.Location = new Point(3, 32);
        groupBoxNameDescription.Name = "groupBoxNameDescription";
        groupBoxNameDescription.Size = new Size(403, 122);
        groupBoxNameDescription.TabIndex = 2;
        groupBoxNameDescription.TabStop = false;
        groupBoxNameDescription.Text = "DT Name, Description";
        // 
        // textBoxDescription
        // 
        textBoxDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxDescription.Location = new Point(157, 51);
        textBoxDescription.Name = "textBoxDescription";
        textBoxDescription.Size = new Size(240, 23);
        textBoxDescription.TabIndex = 1;
        // 
        // labelDescription
        // 
        labelDescription.AutoSize = true;
        labelDescription.Location = new Point(6, 54);
        labelDescription.Name = "labelDescription";
        labelDescription.Size = new Size(67, 15);
        labelDescription.TabIndex = 0;
        labelDescription.Text = "Description";
        // 
        // textBoxShortName
        // 
        textBoxShortName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxShortName.Location = new Point(157, 80);
        textBoxShortName.Name = "textBoxShortName";
        textBoxShortName.Size = new Size(240, 23);
        textBoxShortName.TabIndex = 2;
        // 
        // labelShortName
        // 
        labelShortName.AutoSize = true;
        labelShortName.Location = new Point(6, 83);
        labelShortName.Name = "labelShortName";
        labelShortName.Size = new Size(125, 15);
        labelShortName.TabIndex = 0;
        labelShortName.Text = "Short Name (App Use)";
        // 
        // textBoxName
        // 
        textBoxName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxName.Location = new Point(157, 22);
        textBoxName.Name = "textBoxName";
        textBoxName.Size = new Size(240, 23);
        textBoxName.TabIndex = 0;
        // 
        // labelDtName
        // 
        labelDtName.AutoSize = true;
        labelDtName.Location = new Point(6, 25);
        labelDtName.Name = "labelDtName";
        labelDtName.Size = new Size(39, 15);
        labelDtName.TabIndex = 0;
        labelDtName.Text = "Name";
        // 
        // groupBoxGeo
        // 
        groupBoxGeo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxGeo.Controls.Add(linkLabelGoogleMap);
        groupBoxGeo.Controls.Add(linkLabelTimeZone);
        groupBoxGeo.Controls.Add(textBoxTimeZone);
        groupBoxGeo.Controls.Add(label1);
        groupBoxGeo.Controls.Add(textBoxLng);
        groupBoxGeo.Controls.Add(labelLonitude);
        groupBoxGeo.Controls.Add(textBoxLat);
        groupBoxGeo.Controls.Add(labelLatitude);
        groupBoxGeo.Controls.Add(textBoxEpsgCode);
        groupBoxGeo.Controls.Add(labelEpsgCode);
        groupBoxGeo.Location = new Point(3, 160);
        groupBoxGeo.Name = "groupBoxGeo";
        groupBoxGeo.Size = new Size(403, 145);
        groupBoxGeo.TabIndex = 2;
        groupBoxGeo.TabStop = false;
        groupBoxGeo.Text = "Geo";
        // 
        // linkLabelGoogleMap
        // 
        linkLabelGoogleMap.AutoSize = true;
        linkLabelGoogleMap.Location = new Point(79, 83);
        linkLabelGoogleMap.Name = "linkLabelGoogleMap";
        linkLabelGoogleMap.Size = new Size(72, 15);
        linkLabelGoogleMap.TabIndex = 3;
        linkLabelGoogleMap.TabStop = true;
        linkLabelGoogleMap.Text = "Google Map";
        // 
        // linkLabelTimeZone
        // 
        linkLabelTimeZone.AutoSize = true;
        linkLabelTimeZone.Location = new Point(122, 54);
        linkLabelTimeZone.Name = "linkLabelTimeZone";
        linkLabelTimeZone.Size = new Size(29, 15);
        linkLabelTimeZone.TabIndex = 1;
        linkLabelTimeZone.TabStop = true;
        linkLabelTimeZone.Text = "Link";
        // 
        // textBoxTimeZone
        // 
        textBoxTimeZone.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxTimeZone.Location = new Point(157, 51);
        textBoxTimeZone.Name = "textBoxTimeZone";
        textBoxTimeZone.Size = new Size(240, 23);
        textBoxTimeZone.TabIndex = 2;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(6, 54);
        label1.Name = "label1";
        label1.Size = new Size(95, 15);
        label1.TabIndex = 0;
        label1.Text = "TimeZone Name";
        // 
        // textBoxLng
        // 
        textBoxLng.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxLng.Location = new Point(157, 109);
        textBoxLng.Name = "textBoxLng";
        textBoxLng.Size = new Size(240, 23);
        textBoxLng.TabIndex = 5;
        // 
        // labelLonitude
        // 
        labelLonitude.AutoSize = true;
        labelLonitude.Location = new Point(6, 112);
        labelLonitude.Name = "labelLonitude";
        labelLonitude.Size = new Size(61, 15);
        labelLonitude.TabIndex = 0;
        labelLonitude.Text = "Longitude";
        // 
        // textBoxLat
        // 
        textBoxLat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxLat.Location = new Point(157, 80);
        textBoxLat.Name = "textBoxLat";
        textBoxLat.Size = new Size(240, 23);
        textBoxLat.TabIndex = 4;
        // 
        // labelLatitude
        // 
        labelLatitude.AutoSize = true;
        labelLatitude.Location = new Point(6, 83);
        labelLatitude.Name = "labelLatitude";
        labelLatitude.Size = new Size(50, 15);
        labelLatitude.TabIndex = 0;
        labelLatitude.Text = "Latitude";
        // 
        // textBoxEpsgCode
        // 
        textBoxEpsgCode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxEpsgCode.Location = new Point(157, 22);
        textBoxEpsgCode.Name = "textBoxEpsgCode";
        textBoxEpsgCode.Size = new Size(240, 23);
        textBoxEpsgCode.TabIndex = 0;
        // 
        // labelEpsgCode
        // 
        labelEpsgCode.AutoSize = true;
        labelEpsgCode.Location = new Point(6, 25);
        labelEpsgCode.Name = "labelEpsgCode";
        labelEpsgCode.Size = new Size(88, 15);
        labelEpsgCode.TabIndex = 0;
        labelEpsgCode.Text = "EPSG Code (all)";
        // 
        // buttonWsLoad
        // 
        buttonWsLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonWsLoad.Location = new Point(3, 373);
        buttonWsLoad.Name = "buttonWsLoad";
        buttonWsLoad.Size = new Size(75, 23);
        buttonWsLoad.TabIndex = 1;
        buttonWsLoad.Text = "WS Load";
        buttonWsLoad.UseVisualStyleBackColor = true;
        // 
        // buttonWsSave
        // 
        buttonWsSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonWsSave.Location = new Point(84, 373);
        buttonWsSave.Name = "buttonWsSave";
        buttonWsSave.Size = new Size(75, 23);
        buttonWsSave.TabIndex = 2;
        buttonWsSave.Text = "WS Save";
        buttonWsSave.UseVisualStyleBackColor = true;
        // 
        // buttonFileSave
        // 
        buttonFileSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonFileSave.Location = new Point(331, 373);
        buttonFileSave.Name = "buttonFileSave";
        buttonFileSave.Size = new Size(75, 23);
        buttonFileSave.TabIndex = 3;
        buttonFileSave.Text = "File Save";
        buttonFileSave.UseVisualStyleBackColor = true;
        // 
        // groupBoxUnit
        // 
        groupBoxUnit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxUnit.Controls.Add(radioButtonUnitSI);
        groupBoxUnit.Controls.Add(radioButtonUnitUS);
        groupBoxUnit.Location = new Point(6, 311);
        groupBoxUnit.Name = "groupBoxUnit";
        groupBoxUnit.Size = new Size(400, 56);
        groupBoxUnit.TabIndex = 7;
        groupBoxUnit.TabStop = false;
        groupBoxUnit.Text = "Unit";
        // 
        // radioButtonUnitSI
        // 
        radioButtonUnitSI.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        radioButtonUnitSI.AutoSize = true;
        radioButtonUnitSI.Location = new Point(335, 22);
        radioButtonUnitSI.Name = "radioButtonUnitSI";
        radioButtonUnitSI.Size = new Size(59, 19);
        radioButtonUnitSI.TabIndex = 1;
        radioButtonUnitSI.Text = "SI Unit";
        radioButtonUnitSI.UseVisualStyleBackColor = true;
        // 
        // radioButtonUnitUS
        // 
        radioButtonUnitUS.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        radioButtonUnitUS.AutoSize = true;
        radioButtonUnitUS.Checked = true;
        radioButtonUnitUS.Location = new Point(265, 22);
        radioButtonUnitUS.Name = "radioButtonUnitUS";
        radioButtonUnitUS.Size = new Size(64, 19);
        radioButtonUnitUS.TabIndex = 0;
        radioButtonUnitUS.TabStop = true;
        radioButtonUnitUS.Text = "US Unit";
        radioButtonUnitUS.UseVisualStyleBackColor = true;
        // 
        // NewProjectControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(groupBoxUnit);
        Controls.Add(buttonFileSave);
        Controls.Add(buttonWsSave);
        Controls.Add(buttonWsLoad);
        Controls.Add(groupBoxGeo);
        Controls.Add(groupBoxNameDescription);
        Controls.Add(textBoxDtInfo);
        Controls.Add(labelDtInfo);
        Name = "NewProjectControl";
        Size = new Size(409, 399);
        groupBoxNameDescription.ResumeLayout(false);
        groupBoxNameDescription.PerformLayout();
        groupBoxGeo.ResumeLayout(false);
        groupBoxGeo.PerformLayout();
        groupBoxUnit.ResumeLayout(false);
        groupBoxUnit.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelDtInfo;
    private TextBox textBoxDtInfo;
    private GroupBox groupBoxNameDescription;
    private TextBox textBoxDescription;
    private Label labelDescription;
    private TextBox textBoxName;
    private Label labelDtName;
    private TextBox textBoxShortName;
    private Label labelShortName;
    private GroupBox groupBoxGeo;
    private TextBox textBoxTimeZone;
    private Label label1;
    private TextBox textBoxLat;
    private Label labelLatitude;
    private TextBox textBoxEpsgCode;
    private Label labelEpsgCode;
    private LinkLabel linkLabelTimeZone;
    private TextBox textBoxLng;
    private Label labelLonitude;
    private LinkLabel linkLabelGoogleMap;
    private Button buttonWsLoad;
    private Button buttonWsSave;
    private Button buttonFileSave;
    private GroupBox groupBoxUnit;
    private RadioButton radioButtonUnitSI;
    private RadioButton radioButtonUnitUS;
}
