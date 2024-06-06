using System.Runtime.InteropServices;

namespace WaterSight.UI.Extensions;

public enum ProgrssBarState
{
    Normal = 1,     // green
    Error = 2,      // red
    Warning = 3,    // yellow
}

public static class ProgressBarExtensions
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
    static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
    public static void SetState(this ProgressBar pBar, ProgrssBarState state)
    {
        SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
    }
}
