using WaterSight.Web.Settings;

namespace WaterSight.UI.Forms.Support;

public class BusyWindow
{
    #region Constructor
    public BusyWindow()
    {
    }
    #endregion

    public void Show(Form parentForm)
    {
        BusyForm = new Form();
        BusyForm.FormBorderStyle = FormBorderStyle.None;
        BusyForm.Size = new Size(100, 5);
        BusyForm.ShowInTaskbar = false;

        var pb = new ProgressBar();
        BusyForm.Controls.Add(pb);

        pb.Style = ProgressBarStyle.Marquee;
        pb.Dock = DockStyle.Fill;
        pb.MarqueeAnimationSpeed = 10;

        BusyForm.Location = CenteredToolForm.CenteredLocation(parentForm, BusyForm);

        BusyForm.Show();
    }

    public void Done()
    {
        BusyForm?.Close();
        BusyForm?.Dispose();
    }

    public Form? BusyForm { get; private set; }
}
