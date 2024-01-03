namespace WaterSight.WaterSightDataCollector.Test;

enum DtID
{
    // QA
    QaAwMoStl = 3253,

    // Prod
    ProdAquaKankakee = 139,
    ProdAwMoStl = 378,

}

public static class DtInfo
{
    // QA
    public const string DtName_QaAwMoStl = "QaAwMoStl";

    // Prod
    public const string DtName_ProdAwMoStl = "ProdAwMoStl";
    public const string DtName_ProdAquaKankakee = "ProdKankakee";
}
