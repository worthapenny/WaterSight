using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.MonitoredFileDataPusher.Support;

namespace WaterSight.MonitoredFileDataPusher.Test;


[TestFixture]
public class TestExcelRead
{

    [Test]
    public void Test()
    {
        var xlFilePath = @"D:\Office\WaterSight\Users\AquaAmerica\Kankakee\02_Analysis\04_SCADA\Kankakee Data 5 min 2022.xlsx";
        var xlFile = new ExcelFile(xlFilePath);

        //foreach (var dt in xlFile.Read("January"))
        //{

        //}

        new ExcelFile(xlFilePath).ReadExcel("January");
    }
}
