using System.Runtime.InteropServices;

namespace WaterSight.UI.Extensions;

public static class IconExtensions
{
    public static Icon? Extract(string file, int number, bool largeIcon)
    {
        ExtractIconEx(file, number, out IntPtr large, out IntPtr small, 1);
        try
        {
            return Icon.FromHandle(largeIcon ? large : small);
        }
        catch
        {
            return null;
        }

    }
    [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
}
