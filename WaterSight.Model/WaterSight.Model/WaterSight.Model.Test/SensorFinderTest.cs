using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using WaterSight.Model.Sensors;
using WaterSight.Model.Support;

namespace WaterSight.Model.Test
{
    public class SensorFinderTest : OpenFlowsWaterTestFixtureBase
    {
        #region Setup/Teardown
        protected override void SetupImpl()
        {
            string modelPath = @"C:\Program Files (x86)\Bentley\WaterGEMS\Samples\Example5.wtg";
            FileAssert.Exists(modelPath);

            OpenModel(modelPath);
            Assert.IsNotNull(WaterModel);
        }
        #endregion

        #region Tests
        [Test]
        public async Task SensorFinder()
        {
            var connectivity = await Library.GetConnectivityAsync(DomainProject);
            var sensorFinder = new SensorFinder(WaterModel);
            var tanks = sensorFinder.SearchTanks(connectivity, new SensorFinderOptions());
        }
        #endregion
    }
}
