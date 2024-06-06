using WaterSight.Domain;
using WaterSight.UI.App;
using WaterSight.UI.Extensions;

namespace WaterSight.UI.Controls;

public partial class DragableControlBase : UserControl
{

    #region Public Evengs
    public event EventHandler<EventArgs>? AddOrRemoveButtonClicked;
    public event EventHandler<MouseEventArgs>? DragEventStarted;
    public event EventHandler<MouseEventArgs>? DragEventEnded;
    #endregion

    #region Constants
    private const string ADD = "┼";
    private const string REMOVE = "X";
    #endregion

    public DragableControlBase()
    {
        InitializeComponent();
    }

    #region Overridden Methods
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        //
        // One time
        CheckBoxEnableAction.Checked = true;
        ProgressBarControl.Visible = false;

        MinimumSize = Size;
        OriginalWidth = Width;

        // Context Menu
        var validateImage = IconExtensions.Extract("shell32.dll", 294, false)?.ToBitmap();
        var runImage = IconExtensions.Extract("shell32.dll", 297, false)?.ToBitmap(); ;
        var saveImage = IconExtensions.Extract("shell32.dll", 6, false)?.ToBitmap(); ;

        contextMenuStrip.Items.Clear();
        contextMenuStrip.Items.Add(
            text: "Validate",
            image: validateImage,
            onClick: (s, e) => { ValidateInput(); });
        contextMenuStrip.Items.Add(
            text: "Run",
            image: runImage,
            onClick: (s, e) => { Run(); });
        contextMenuStrip.Items.Add(
            text: "Save",
            image: saveImage,
            onClick: (s, e) => { SaveAsync(); });

        // Buttons
        buttonAddOrRemove.ForeColor = Color.FromKnownColor(KnownColor.DarkGreen);
        ButtonValidate.Image = validateImage;
        ButtonRun.Image = runImage;
        buttonSave.Image = saveImage;
        buttonAddOrRemove.Text = ADD;


        InitializeEvents();
        InitializeVisually();
    }

    protected virtual void InitializeEvents()
    {
        //App.ProjectSelectionChanged += (s, e) => InitializeVisually();
        UIApp.Instance.DigitalTwinChanged += (s, e) => DigitalTwinIdChanged();
        ButtonValidate.Click += (s, e) => ValidateInput();
        ButtonRun.Click += (s, e) => Run();
        buttonSave.Click += (s, e) => SaveAsync();
        CheckBoxEnableAction.CheckedChanged += (s, e) => EnableControls(CheckBoxEnableAction.Checked);
    }
    protected virtual void DigitalTwinIdChanged()
    {
        InitializeVisually();
    }
    protected virtual void InitializeVisually()
    {


        // Add Curtain
        var transparentControlName = "Curtain";
        if (!this.Controls.ContainsKey(transparentControlName))
        {
            this.curtainControl ??= new TransparentControl() { Name = transparentControlName };
            this.curtainControl.MouseDown += (s, e) => OnMouseDownOnCurtain(e);
            this.curtainControl.MouseUp += (s, e) => OnMouseUpOnCurtain(e);
            this.curtainControl.MouseMove += (s, e) => OnMouseMoveOnCurtain(e);

            this.Controls.Add(this.curtainControl);
            this.curtainControl.BringToFront();
        }
        DropCurtain(false);
    }
    protected virtual async Task<bool> SaveAsync() { return false; }
    protected virtual void EnableControls(bool enable)
    {
        ProgressBarControl.Enabled = enable;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && IsMouseOverRightEdge(e)) // Adjust the value for your resizing sensitivity
        {
            IsResizing = true;
            OriginalWidth = Width;
        }
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        Cursor.Current = IsMouseOverRightEdge(e) ? Cursors.SizeWE : Cursors.Default;

        if (IsResizing)
        {
            int newWidth = OriginalWidth + e.X - (Left + OriginalWidth);
            Width = Math.Max(newWidth, MinimumWidth);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        IsResizing = false;
    }

    protected virtual void ValidateInput() { }
    protected virtual void Run() { }
    #endregion



    #region Private Methods
    private bool IsMouseOverRightEdge(MouseEventArgs e)
    {
        return e.X >= Width - 5;
    }
    protected void OnMouseDownOnCurtain(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        _lastPosition = e.Location;
        Cursor = Cursors.SizeAll;

        DoDragDrop(this.labelSectionTitle.Text, DragDropEffects.Move);

        DragEventStarted?.Invoke(this, e);
    }
    protected void OnMouseUpOnCurtain(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        this.labelSectionTitle.Cursor = Cursors.Hand;

        DragEventEnded?.Invoke(this, e);
    }
    protected void OnMouseMoveOnCurtain(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.Button == MouseButtons.Left)
        {
            Left += e.Location.X - _lastPosition.X;
            Top += e.Location.Y - _lastPosition.Y;
        }
    }

    private void DropCurtain(bool drop)
    {
        this.curtainControl.Size = drop ? this.Size : new Size(0, 0);
        this.curtainControl.AutoSize = false;
        this.curtainControl.Cursor = Cursors.Hand;
    }
    private void UpdateSectionAndTitle()
    {
        labelSectionTitle.Text = $"{Section}: {Title}";
    }
    #endregion

    #region Event Handlers

    private void buttonClose_Click(object sender, EventArgs e)
    {

        if (CanAddControl)
        {
            buttonAddOrRemove.ForeColor = Color.FromKnownColor(KnownColor.DarkGreen);
            buttonAddOrRemove.Text = ADD;
        }
        else
        {
            buttonAddOrRemove.ForeColor = Color.FromKnownColor(KnownColor.DarkRed);
            buttonAddOrRemove.Text = REMOVE;
        }

        AddOrRemoveButtonClicked?.Invoke(this, e);
    }

    #endregion

    #region Public Property
    public string Section
    {
        get => _section;
        set { _section = value; UpdateSectionAndTitle(); }
    }
    public string Title
    {
        get => _title;
        set { _title = value; UpdateSectionAndTitle(); }
    }
    public Color TitleColor
    {
        get => this.labelSectionTitle.BackColor;
        set => this.labelSectionTitle.BackColor = value;
    }
    //public bool EnableAction
    //{
    //    get => this.CheckBoxEnableAction.Enabled;
    //    set => this.CheckBoxEnableAction.Enabled = value;
    //}

    public bool CanAddControl => buttonAddOrRemove.Text == REMOVE;
    public int MinimumWidth => MinimumSize.Width;



    private bool IsResizing { get; set; } = false;
    private int OriginalWidth { get; set; }


    #endregion

    #region Fields
    private string _title;
    private string _section;
    private string _waterSightDir;

    private Point _lastPosition;
    private Control curtainControl;
    #endregion

}
