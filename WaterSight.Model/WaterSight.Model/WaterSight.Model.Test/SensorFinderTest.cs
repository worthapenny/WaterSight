using Haestad.Domain;
using NUnit.Framework;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System.IO;
using System.Threading.Tasks;
using WaterSight.Model.Sensors;
using WaterSight.Model.Support;
using WaterSight.Model.Support.Network;

namespace WaterSight.Model.Test
{
    public class SensorFinderTest : OpenFlowsWaterTestFixtureBase
    {
        #region Setup/Teardown
        protected override void OneTimeSetupImpl()
        {
            ModelFilePath = @"C:\Program Files (x86)\Bentley\WaterGEMS\Samples\Example5.wtg";
            Assert.That(File.Exists(ModelFilePath));

            //OpenModel(modelPath);

            Assert.That(WaterModel, Is.Not.Null);
        }
        #endregion

        #region Tests
        [Test]
        public async Task SensorFinder()
        {
            var connectivity = await ConnectionTopology.GetConnectivityAsync(DomainProject);
            var sensorFinder = new SensorFinder(WaterModel);
            var tanks = sensorFinder.SearchTanks(connectivity, new SensorFinderOptions());
        }

        [Test]
        public void AddSpotElevation()
        {
            var dds = WaterModel.DomainDataSet;
            string databaseFileName = (string)((IDataSourceConnection)dds.DomainDataSetManager.DataSource).GetConnectionProperty(ConnectionProperty.FileName);
            var f = new FileInfo(databaseFileName);

            ////var a = ((Haestad.Domain.ModelingObjects.DataSourceBase)dds.DomainDataSetManager.DataSource).DataConnection;

            //var seMngr = dds.DomainElementManager((int)DomainElementType.ScadaElementManager);
            //var see = (IDomainElement)seMngr.Elements()[0];


            foreach (var element in WaterModel.Network.Elements())
            {
                if (element is IPointNodeInput)
                {
                    var se = WaterModel.Network.SpotElevations.Create();
                    se.Label = $"SE_{element.Label}";
                    se.Input.Elevation = (element as IPhysicalNodeElementInput).Elevation;
                    se.Input.SetPoint((element as IPointNodeInput).GetPoint());
                    se.Input.IsActive = (element as IActiveElementInput).IsActive;
                }
            }
        }
        #endregion
    }
}
