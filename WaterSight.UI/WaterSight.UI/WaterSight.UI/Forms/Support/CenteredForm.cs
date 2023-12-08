namespace WaterSight.UI.Forms.Support;



public class CenteredToolForm
{
    #region Constructor
    public CenteredToolForm(string title, Form parentForm, Control control, Size size, Icon icon = null)
    {
        this.Title = title;
        this.ParentForm = parentForm;
        this.Control = control;
        this.Size = size;
        this.Icon = icon;
    }
    #endregion

    #region Public Methods
    public static Point CenteredLocation(Form parentForm, Form childForm)
    {
        childForm.StartPosition = FormStartPosition.Manual;

        if (parentForm != null)
        {
            childForm.Location = new Point(
                parentForm.Left + parentForm.Width / 2 - childForm.Width / 2,
                parentForm.Top + parentForm.Height / 2 - childForm.Height / 2);
        }

        return childForm.Location;

    }
    public Form NewCenteredForm()
    {
        var form = new Form();

        Control.Dock = DockStyle.Fill;
        form.Controls.Add(Control);
        form.FormBorderStyle = FormBorderStyle.SizableToolWindow;
        form.Text = Title;
        form.Size = Size;

        if (Icon != null)
            form.Icon = Icon;

        form.StartPosition = FormStartPosition.Manual;
        form.Location = CenteredLocation(ParentForm, form);

        return form;
    }
    public DialogResult ShowDialog()
    {
        return NewCenteredForm().ShowDialog(ParentForm);
    }
    public void Show()
    {
        NewCenteredForm().Show();
    }
    public DialogResult Show(Keys keys)
    {
        if (keys == Keys.Control)
        {
            NewCenteredForm().Show();
            return DialogResult.None;
        }
        else
            return ShowDialog();
    }
    #endregion

    #region Fields
    string Title { get; set; }
    Form ParentForm { get; set; }
    Control Control { get; set; }
    Size Size { get; set; }
    Icon Icon { get; set; }
    #endregion
}
