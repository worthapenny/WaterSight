using WaterSight.Model.Generator.Data;

namespace WaterSight.Domain;

public class SCADA
{
    #region Constructor
    public SCADA(string scadaDir, TimeSeriesDbStructure? dbStructure = null)
    {
        ScadaDir = scadaDir;
        TimeSeriesDbStructure = dbStructure;
    }
    #endregion


    #region Public Properties
    public string ScadaDir { get; }
    public TimeSeriesDbStructure? TimeSeriesDbStructure { get; set; }
    #endregion
}