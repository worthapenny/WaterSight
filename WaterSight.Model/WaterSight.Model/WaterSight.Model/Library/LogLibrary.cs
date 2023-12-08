using Serilog;

namespace WaterSight.Model.Library;

public static class LogLibrary
{
    #region Constancts
    public const char Equal = '=';
    public const char Dot = '.';
    public const char Star = '*';
    public const char Plus = '+';
    public const char XSmall = 'x';
    public const char XBig = 'X';
    public const char Dash = '-';
    public const char AboveScore = '‾';
    public const char Underscore = '_';
    public const char Bullet = '•';
    public const char BulletPlus = '●';
    public const char BulletInverse = '◘';
    public const char BulletWhite = '◦';
    public const char Square = '▪';
    public const char SquarePlus = '■';
    public const char Infinity = '∞';
    public const char OSmall = 'o';
    public const char OBig = 'O';
    public const char ArrowDown = '↓';
    public const char ArrowUp = '↑';


    public const int SeparatorLength = 100;
    #endregion

    #region Public Static Methods
    public static void Separate(char separator, int length) => Log.Debug(new string(separator, length));

    public static void Separate_Hard() => Separate(BulletPlus, SeparatorLength);
    public static void Separate_Soft() => Separate(Bullet, SeparatorLength);
    public static void Separate_SuperSoft() => Separate(Dash, SeparatorLength);
    public static void Separate_StartMajor() => Separate(SquarePlus, SeparatorLength);
    public static void Separate_StartMinor() => Separate(Square, SeparatorLength);
    public static void Separate_EndMajor() => Separate(BulletInverse, SeparatorLength);
    public static void Separate_EndMinor() => Separate(BulletWhite, SeparatorLength);
    public static void Separate_StartGroup() => Separate(ArrowDown, SeparatorLength);
    public static void Separate_EndGroup() => Separate(ArrowUp, SeparatorLength);


    public static void Separate_Equal() => Separate(Equal, SeparatorLength);
    public static void Separate_Dot() => Separate(Dot, SeparatorLength);
    public static void Separate_Star() => Separate(Star, SeparatorLength);
    public static void Separate_Plus() => Separate(Plus, SeparatorLength);
    public static void Separate_XSmall() => Separate(XSmall, SeparatorLength);
    public static void Separate_XBig() => Separate(XBig, SeparatorLength);
    public static void Separate_Dash() => Separate(Dash, SeparatorLength);
    public static void Separate_AboveScore() => Separate(AboveScore, SeparatorLength);
    public static void Separate_Underscore() => Separate(Underscore, SeparatorLength);
    public static void Separate_Bullet() => Separate(Bullet, SeparatorLength);
    public static void Separate_BulletPlus() => Separate(BulletPlus, SeparatorLength);
    public static void Separate_BulletInverse() => Separate(BulletInverse, SeparatorLength);
    public static void Separate_BulletWhite() => Separate(BulletWhite, SeparatorLength);
    public static void Separate_Square() => Separate(Square, SeparatorLength);
    public static void Separate_SquarePlus() => Separate(SquarePlus, SeparatorLength);
    public static void Separate_Infinity() => Separate(Infinity, SeparatorLength);
    public static void Separate_OSmall() => Separate(OSmall, SeparatorLength);
    public static void Separate_OBig() => Separate(OBig, SeparatorLength);
    #endregion
}
