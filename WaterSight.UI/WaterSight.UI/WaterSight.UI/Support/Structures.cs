using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.UI.ControlModels;

namespace WaterSight.UI.Support;

public enum ServerEnvironment
{
    Prod, QA, Dev
}


public static class Colors
{
    public static Color Red = Color.Red;
    public static Color RedDark = Color.DarkRed;
    public static Color Lime = Color.Lime;
    public static Color LimeDark = Color.LimeGreen;
    public static Color Blue = Color.Blue;
    public static Color BlueDark = Color.DarkBlue;
    public static Color Yellow = Color.Khaki;
    public static Color YellowDark = Color.DarkKhaki;
    public static Color Cyan = Color.Cyan;
    public static Color CyanDark = Color.DarkCyan;
    public static Color Magenta = Color.Magenta;
    public static Color MagentaDark = Color.DarkMagenta;
    public static Color Brown = Color.Brown;
    public static Color BrownDark = Color.Maroon;
    public static Color Olive = Color.Olive;
    public static Color OliveDark = Color.DarkOliveGreen;
    public static Color Green = Color.Green;
    public static Color GreenDark = Color.DarkGreen;
    public static Color Navy = Color.LightBlue;
    public static Color NavyDark = Color.Navy;

}


//public class ProjectSelectionChangedEventArgs : EventArgs
//{
//    public ProjectSelectionChangedEventArgs(NewProjectControlModel project)
//    {
//            SelectedProject = project;
//    }

//    public NewProjectControlModel SelectedProject { get; set; }
//}