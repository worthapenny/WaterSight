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
        buttonMakeCurrent = new Button();
        labelProjectPath = new Label();
        textBoxProjectPath = new TextBox();
        buttonBrowseProject = new Button();
        groupBoxCosts = new GroupBox();
        textBoxEnergyCost = new TextBox();
        labelEnergyCost = new Label();
        textBoxTariff = new TextBox();
        labelTariff = new Label();
        textBoxProductionCost = new TextBox();
        labelProductionCost = new Label();
        groupBoxServiceExpectations = new GroupBox();
        textBoxTargetEfficiency = new TextBox();
        label2 = new Label();
        textBoxMinPressure = new TextBox();
        labelMinPressure = new Label();
        textBoxMaxPressure = new TextBox();
        labelMaxPressure = new Label();
        buttonLoadDefaultService = new Button();
        buttonLoadDefaultCosts = new Button();
        groupBoxNameDescription.SuspendLayout();
        groupBoxGeo.SuspendLayout();
        groupBoxUnit.SuspendLayout();
        groupBoxCosts.SuspendLayout();
        groupBoxServiceExpectations.SuspendLayout();
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
        textBoxDtInfo.Size = new Size(251, 23);
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
        groupBoxNameDescription.Location = new Point(3, 61);
        groupBoxNameDescription.Name = "groupBoxNameDescription";
        groupBoxNameDescription.Size = new Size(408, 122);
        groupBoxNameDescription.TabIndex = 2;
        groupBoxNameDescription.TabStop = false;
        groupBoxNameDescription.Text = "DT Name, Description";
        // 
        // textBoxDescription
        // 
        textBoxDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxDescription.Location = new Point(157, 51);
        textBoxDescription.Name = "textBoxDescription";
        textBoxDescription.Size = new Size(245, 23);
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
        textBoxShortName.Size = new Size(245, 23);
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
        textBoxName.Size = new Size(245, 23);
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
        groupBoxGeo.Location = new Point(3, 189);
        groupBoxGeo.Name = "groupBoxGeo";
        groupBoxGeo.Size = new Size(408, 145);
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
        textBoxTimeZone.Size = new Size(245, 23);
        textBoxTimeZone.TabIndex = 1;
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
        textBoxLng.Size = new Size(245, 23);
        textBoxLng.TabIndex = 3;
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
        textBoxLat.Size = new Size(245, 23);
        textBoxLat.TabIndex = 2;
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
        textBoxEpsgCode.Size = new Size(245, 23);
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
        buttonWsLoad.Location = new Point(3, 570);
        buttonWsLoad.Name = "buttonWsLoad";
        buttonWsLoad.Size = new Size(75, 23);
        buttonWsLoad.TabIndex = 3;
        buttonWsLoad.Text = "WS Load";
        buttonWsLoad.UseVisualStyleBackColor = true;
        // 
        // buttonWsSave
        // 
        buttonWsSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonWsSave.Location = new Point(84, 570);
        buttonWsSave.Name = "buttonWsSave";
        buttonWsSave.Size = new Size(75, 23);
        buttonWsSave.TabIndex = 4;
        buttonWsSave.Text = "WS Save";
        buttonWsSave.UseVisualStyleBackColor = true;
        // 
        // buttonFileSave
        // 
        buttonFileSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonFileSave.Location = new Point(165, 570);
        buttonFileSave.Name = "buttonFileSave";
        buttonFileSave.Size = new Size(75, 23);
        buttonFileSave.TabIndex = 5;
        buttonFileSave.Text = "File Save";
        buttonFileSave.UseVisualStyleBackColor = true;
        // 
        // groupBoxUnit
        // 
        groupBoxUnit.Controls.Add(radioButtonUnitSI);
        groupBoxUnit.Controls.Add(radioButtonUnitUS);
        groupBoxUnit.Location = new Point(3, 340);
        groupBoxUnit.Name = "groupBoxUnit";
        groupBoxUnit.Size = new Size(206, 53);
        groupBoxUnit.TabIndex = 7;
        groupBoxUnit.TabStop = false;
        groupBoxUnit.Text = "Unit";
        // 
        // radioButtonUnitSI
        // 
        radioButtonUnitSI.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        radioButtonUnitSI.AutoSize = true;
        radioButtonUnitSI.Location = new Point(141, 22);
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
        radioButtonUnitUS.Location = new Point(71, 22);
        radioButtonUnitUS.Name = "radioButtonUnitUS";
        radioButtonUnitUS.Size = new Size(64, 19);
        radioButtonUnitUS.TabIndex = 0;
        radioButtonUnitUS.TabStop = true;
        radioButtonUnitUS.Text = "US Unit";
        radioButtonUnitUS.UseVisualStyleBackColor = true;
        // 
        // buttonMakeCurrent
        // 
        buttonMakeCurrent.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonMakeCurrent.Location = new Point(330, 570);
        buttonMakeCurrent.Name = "buttonMakeCurrent";
        buttonMakeCurrent.Size = new Size(75, 23);
        buttonMakeCurrent.TabIndex = 6;
        buttonMakeCurrent.Text = "Set Current";
        buttonMakeCurrent.UseVisualStyleBackColor = true;
        // 
        // labelProjectPath
        // 
        labelProjectPath.AutoSize = true;
        labelProjectPath.Location = new Point(9, 35);
        labelProjectPath.Name = "labelProjectPath";
        labelProjectPath.Size = new Size(71, 15);
        labelProjectPath.TabIndex = 0;
        labelProjectPath.Text = "Project Path";
        // 
        // textBoxProjectPath
        // 
        textBoxProjectPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxProjectPath.Location = new Point(160, 32);
        textBoxProjectPath.Name = "textBoxProjectPath";
        textBoxProjectPath.Size = new Size(214, 23);
        textBoxProjectPath.TabIndex = 1;
        // 
        // buttonBrowseProject
        // 
        buttonBrowseProject.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonBrowseProject.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
        buttonBrowseProject.Location = new Point(380, 32);
        buttonBrowseProject.Name = "buttonBrowseProject";
        buttonBrowseProject.Size = new Size(31, 23);
        buttonBrowseProject.TabIndex = 2;
        buttonBrowseProject.Text = "📂";
        buttonBrowseProject.TextAlign = ContentAlignment.TopCenter;
        buttonBrowseProject.UseVisualStyleBackColor = true;
        buttonBrowseProject.Click += buttonBrowseProject_Click;
        // 
        // groupBoxCosts
        // 
        groupBoxCosts.Controls.Add(textBoxEnergyCost);
        groupBoxCosts.Controls.Add(labelEnergyCost);
        groupBoxCosts.Controls.Add(textBoxTariff);
        groupBoxCosts.Controls.Add(labelTariff);
        groupBoxCosts.Controls.Add(textBoxProductionCost);
        groupBoxCosts.Controls.Add(buttonLoadDefaultCosts);
        groupBoxCosts.Controls.Add(labelProductionCost);
        groupBoxCosts.Location = new Point(215, 399);
        groupBoxCosts.Name = "groupBoxCosts";
        groupBoxCosts.Size = new Size(196, 133);
        groupBoxCosts.TabIndex = 7;
        groupBoxCosts.TabStop = false;
        groupBoxCosts.Text = "Costs ($/?)";
        // 
        // textBoxEnergyCost
        // 
        textBoxEnergyCost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxEnergyCost.Location = new Point(97, 72);
        textBoxEnergyCost.Name = "textBoxEnergyCost";
        textBoxEnergyCost.Size = new Size(93, 23);
        textBoxEnergyCost.TabIndex = 6;
        // 
        // labelEnergyCost
        // 
        labelEnergyCost.AutoSize = true;
        labelEnergyCost.Location = new Point(3, 75);
        labelEnergyCost.Name = "labelEnergyCost";
        labelEnergyCost.Size = new Size(70, 15);
        labelEnergyCost.TabIndex = 0;
        labelEnergyCost.Text = "Energy Cost";
        // 
        // textBoxTariff
        // 
        textBoxTariff.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxTariff.Location = new Point(97, 43);
        textBoxTariff.Name = "textBoxTariff";
        textBoxTariff.Size = new Size(93, 23);
        textBoxTariff.TabIndex = 5;
        // 
        // labelTariff
        // 
        labelTariff.AutoSize = true;
        labelTariff.Location = new Point(3, 46);
        labelTariff.Name = "labelTariff";
        labelTariff.Size = new Size(33, 15);
        labelTariff.TabIndex = 0;
        labelTariff.Text = "Tariff";
        // 
        // textBoxProductionCost
        // 
        textBoxProductionCost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxProductionCost.Location = new Point(97, 16);
        textBoxProductionCost.Name = "textBoxProductionCost";
        textBoxProductionCost.Size = new Size(93, 23);
        textBoxProductionCost.TabIndex = 4;
        // 
        // labelProductionCost
        // 
        labelProductionCost.AutoSize = true;
        labelProductionCost.Location = new Point(3, 19);
        labelProductionCost.Name = "labelProductionCost";
        labelProductionCost.Size = new Size(59, 15);
        labelProductionCost.TabIndex = 0;
        labelProductionCost.Text = "Prod Cost";
        // 
        // groupBoxServiceExpectations
        // 
        groupBoxServiceExpectations.Controls.Add(textBoxTargetEfficiency);
        groupBoxServiceExpectations.Controls.Add(label2);
        groupBoxServiceExpectations.Controls.Add(textBoxMinPressure);
        groupBoxServiceExpectations.Controls.Add(labelMinPressure);
        groupBoxServiceExpectations.Controls.Add(textBoxMaxPressure);
        groupBoxServiceExpectations.Controls.Add(buttonLoadDefaultService);
        groupBoxServiceExpectations.Controls.Add(labelMaxPressure);
        groupBoxServiceExpectations.Location = new Point(3, 399);
        groupBoxServiceExpectations.Name = "groupBoxServiceExpectations";
        groupBoxServiceExpectations.Size = new Size(206, 133);
        groupBoxServiceExpectations.TabIndex = 7;
        groupBoxServiceExpectations.TabStop = false;
        groupBoxServiceExpectations.Text = "Service (psi)";
        // 
        // textBoxTargetEfficiency
        // 
        textBoxTargetEfficiency.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxTargetEfficiency.Location = new Point(109, 72);
        textBoxTargetEfficiency.Name = "textBoxTargetEfficiency";
        textBoxTargetEfficiency.Size = new Size(91, 23);
        textBoxTargetEfficiency.TabIndex = 6;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(3, 75);
        label2.Name = "label2";
        label2.Size = new Size(79, 15);
        label2.TabIndex = 0;
        label2.Text = "Tar. Efficiency";
        // 
        // textBoxMinPressure
        // 
        textBoxMinPressure.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxMinPressure.Location = new Point(109, 43);
        textBoxMinPressure.Name = "textBoxMinPressure";
        textBoxMinPressure.Size = new Size(91, 23);
        textBoxMinPressure.TabIndex = 5;
        // 
        // labelMinPressure
        // 
        labelMinPressure.AutoSize = true;
        labelMinPressure.Location = new Point(3, 46);
        labelMinPressure.Name = "labelMinPressure";
        labelMinPressure.Size = new Size(75, 15);
        labelMinPressure.TabIndex = 0;
        labelMinPressure.Text = "Min Pressure";
        // 
        // textBoxMaxPressure
        // 
        textBoxMaxPressure.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxMaxPressure.Location = new Point(109, 16);
        textBoxMaxPressure.Name = "textBoxMaxPressure";
        textBoxMaxPressure.Size = new Size(91, 23);
        textBoxMaxPressure.TabIndex = 4;
        // 
        // labelMaxPressure
        // 
        labelMaxPressure.AutoSize = true;
        labelMaxPressure.Location = new Point(3, 19);
        labelMaxPressure.Name = "labelMaxPressure";
        labelMaxPressure.Size = new Size(77, 15);
        labelMaxPressure.TabIndex = 0;
        labelMaxPressure.Text = "Max Pressure";
        // 
        // buttonLoadDefaultService
        // 
        buttonLoadDefaultService.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonLoadDefaultService.Location = new Point(109, 101);
        buttonLoadDefaultService.Name = "buttonLoadDefaultService";
        buttonLoadDefaultService.Size = new Size(91, 23);
        buttonLoadDefaultService.TabIndex = 0;
        buttonLoadDefaultService.Text = "Load Defaults";
        buttonLoadDefaultService.UseVisualStyleBackColor = true;
        buttonLoadDefaultService.Click += buttonLoadDefaultService_Click;
        // 
        // buttonLoadDefaultCosts
        // 
        buttonLoadDefaultCosts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonLoadDefaultCosts.Location = new Point(97, 101);
        buttonLoadDefaultCosts.Name = "buttonLoadDefaultCosts";
        buttonLoadDefaultCosts.Size = new Size(93, 23);
        buttonLoadDefaultCosts.TabIndex = 0;
        buttonLoadDefaultCosts.Text = "Load Defaults";
        buttonLoadDefaultCosts.UseVisualStyleBackColor = true;
        buttonLoadDefaultCosts.Click += buttonLoadDefaultCosts_Click;
        // 
        // NewProjectControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(buttonBrowseProject);
        Controls.Add(groupBoxServiceExpectations);
        Controls.Add(groupBoxCosts);
        Controls.Add(groupBoxUnit);
        Controls.Add(buttonMakeCurrent);
        Controls.Add(buttonFileSave);
        Controls.Add(buttonWsSave);
        Controls.Add(textBoxProjectPath);
        Controls.Add(labelProjectPath);
        Controls.Add(buttonWsLoad);
        Controls.Add(groupBoxGeo);
        Controls.Add(groupBoxNameDescription);
        Controls.Add(textBoxDtInfo);
        Controls.Add(labelDtInfo);
        Name = "NewProjectControl";
        Size = new Size(414, 596);
        groupBoxNameDescription.ResumeLayout(false);
        groupBoxNameDescription.PerformLayout();
        groupBoxGeo.ResumeLayout(false);
        groupBoxGeo.PerformLayout();
        groupBoxUnit.ResumeLayout(false);
        groupBoxUnit.PerformLayout();
        groupBoxCosts.ResumeLayout(false);
        groupBoxCosts.PerformLayout();
        groupBoxServiceExpectations.ResumeLayout(false);
        groupBoxServiceExpectations.PerformLayout();
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
    private Button buttonMakeCurrent;
    private Label labelProjectPath;
    private TextBox textBoxProjectPath;
    private Button buttonBrowseProject;
    private GroupBox groupBoxCosts;
    private TextBox textBoxEnergyCost;
    private Label labelEnergyCost;
    private TextBox textBoxTariff;
    private Label labelTariff;
    private TextBox textBoxProductionCost;
    private Label labelProductionCost;
    private GroupBox groupBoxServiceExpectations;
    private TextBox textBoxTargetEfficiency;
    private Label label2;
    private TextBox textBoxMinPressure;
    private Label labelMinPressure;
    private TextBox textBoxMaxPressure;
    private Label labelMaxPressure;
    private Button buttonLoadDefaultCosts;
    private Button buttonLoadDefaultService;
}
