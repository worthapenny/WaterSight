namespace WaterSight.UI.Controls.Modules;


public partial class ModuleControlBase : UserControl
{
    public event EventHandler<ModuleControlBaseEventArgs>? ModuleControlBaseEvent;

    public ModuleControlBase()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitializeBinding();
        InitializeVisually();

    }

    protected virtual void InitializeBinding() { }
    protected virtual void InitializeVisually()
    {
        if (!DroppedToTarget)
        {
            var controlWidth = 200;
            if (Label.Length > 0)
                controlWidth = Label.Length * 12;

            this.Size = new Size(controlWidth, 20);
        }
    }

    private void buttonMoveUp_Click(object sender, EventArgs e)
    {
        ModuleControlBaseEvent?.Invoke(this, new ModuleControlBaseEventArgs(ModuleControlBaseEventType.MoveUp));
    }

    private void buttonDown_Click(object sender, EventArgs e)
    {
        ModuleControlBaseEvent?.Invoke(this, new ModuleControlBaseEventArgs(ModuleControlBaseEventType.MoveDown));
    }

    private void buttonToggleSize_Click(object sender, EventArgs e)
    {
        ModuleControlBaseEvent?.Invoke(this, new ModuleControlBaseEventArgs(ModuleControlBaseEventType.ToggleSize));
    }

    private void buttonClose_Click(object sender, EventArgs e)
    {
        ModuleControlBaseEvent?.Invoke(this, new ModuleControlBaseEventArgs(ModuleControlBaseEventType.Close));
    }


    public string Label
    {
        get { return this.labelTitle.Text; }
        set { this.labelTitle.Text = value; }
    }

    public bool DroppedToTarget { get; set; } = false;

    //public ModuleControlBase(ModuleBase moduleBase)
    //    : this()
    //{
    //    ModuleBase = moduleBase;

    //    this.labelTitle.Text = moduleBase.Label;
    //}


    //public ModuleBase ModuleBase { get; }

    //public bool IsMinimized { get; set; } = true;
    //public bool CanSave { get; set; }
    //public Size MaxSize { get { return MaximumSize; } set { MaximumSize = value; } }
}
