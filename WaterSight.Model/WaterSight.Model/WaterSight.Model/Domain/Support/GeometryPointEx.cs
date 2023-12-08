using Haestad.Support.Support;
using System.Diagnostics;

namespace WaterSight.Model.Domain.Scada.Support;



/// <summary>
/// This class serializes better compared to GeometryPoint class
/// </summary>
[DebuggerDisplay ("{ToString()}")]
public class GeometryPointEx
{
    #region Constructor
    public GeometryPointEx()
    {
    }
    public GeometryPointEx(double x, double y)
        : this()
    {
        X = x;
        Y = y;
    }
    public GeometryPointEx(GeometryPoint? point)
        : this(point?.X ?? 0, point?.Y ?? 0) { }
    #endregion

    #region Public Methods
    public GeometryPoint ToGeometryPoint()
    {
        return new GeometryPoint(X, Y);
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
    #endregion

    #region Public Properties
    public double X { get; set; }
    public double Y { get; set; }
    #endregion

}