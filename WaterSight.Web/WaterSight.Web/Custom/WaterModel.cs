using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.NumericModels;
using WaterSight.Web.Sensors;
using WaterSight.Web.Support;

namespace WaterSight.Web.Custom;

public class WaterModel : WSItem
{
	#region Constructor
	public WaterModel(WS ws)
		:base(ws)
	{
	}
	#endregion

	#region Public Methods
	public async Task<string> GetDomainName() {
		return waterModelDomainName ??= (await WS.NumericModel.GetModelDomainsWaterType()).First().Name;
	}
	public async Task<List<ModelMeasureData>> GetAllScadaElementsOutputData()
	{
		var waterModelDomainName = await GetDomainName();

		var getSensorsConfig = WS.Sensor.GetSensorsConfigAsync();
		var getMappedSignals = WS.NumericModel.GetModelTargetElements(waterModelDomainName);
		var getRunTimeSteps = WS.NumericModel.GetSimulationTimeStepsWaterModel(waterModelDomainName);

		// run all the tasks
		await Task.WhenAll(getSensorsConfig, getMappedSignals, getRunTimeSteps);

		var sensorsConfig = getSensorsConfig.Result;
		var mappedSignals = getMappedSignals.Result; // model element to corresponding SCADA Elements
		var runTimeSteps = getRunTimeSteps.Result;
		var firstTimeStep = runTimeSteps.First();
		var lastTimeStep = runTimeSteps.Last();

        Logger.Debug(Util.LogSeparatorDashes);

        var modelMeasureDataLlist = new List<ModelMeasureData>();
		Logger.Debug($"About to collect model data for '{mappedSignals.Count}' elements.");

		// get model results for each SCADA Element		
		var modelElementCount = 0;
		foreach (var mappedSignal in mappedSignals)
		{
			foreach (var scadaElementConfig in mappedSignal.Value)
			{
				var results = await WS.NumericModel.GetModelResults(
					elementId: scadaElementConfig.ModelElementId,
					parameterName: scadaElementConfig.ResultAttribute,
					modelDomainName: waterModelDomainName,
					startDate: firstTimeStep,
					endDate: lastTimeStep
					);

				var modelMeasuredData = new ModelMeasureData();
                modelMeasuredData.ScadaElementId = scadaElementConfig.ScadaElementId;
                modelMeasuredData.TargetModelElementId = scadaElementConfig.ModelElementId;
                modelMeasuredData.ScadaTag = scadaElementConfig.SignalLabel;
				modelMeasuredData.ModelData = results;

                var sensorConfig = sensorsConfig.Where(s => s.TagId == scadaElementConfig.SignalLabel).First();
				modelMeasuredData.SensorConfigId = sensorConfig.ID;
                modelMeasuredData.ParameterType = scadaElementConfig.ResultAttribute;
				modelMeasuredData.DisplayName = sensorConfig.Name;
				modelMeasuredData.Unit = sensorConfig.Units;

                modelMeasureDataLlist.Add(modelMeasuredData);
				modelElementCount++;
				Logger.Debug($"[{modelElementCount}] Collected model data for {modelMeasuredData.TargetModelElementId}.");
            }
		}
		Logger.Debug(Util.LogSeparatorDashes);

		// SCADA data colleciton
		// for each modelMeasured object
		Logger.Debug($"About to collect SCADA for '{modelMeasureDataLlist.Count}' sensors");
		var getScadaData = modelMeasureDataLlist.Select(m => WS.Sensor.GetSensorTSDAsync(m.SensorConfigId, firstTimeStep, lastTimeStep));
		var scadaDataList = await Task.WhenAll(getScadaData);
        Logger.Debug(Util.LogSeparatorDashes);

        // update SCADA data
        for (int i = 0; i < scadaDataList.Length; i++)
			modelMeasureDataLlist[i].ScadaData = scadaDataList[i];

		return modelMeasureDataLlist;
    }
    #endregion

    #region Public Property
    #endregion

    #region Private Fields
    private string waterModelDomainName;
    #endregion
}

#region Support Class
public class ModelMeasureData
{
	public int SensorConfigId { get; set; }
	public string DisplayName { get; set; }
	public string ScadaTag { get; set; }
	public string Unit { get; set; }
	public string ParameterType { get; set; }
	public SensorTsdWeb ScadaData { get; set; }
	public int WaterSightDomainElementType { get; set; }

	public int ScadaElementId { get; set; }
	public int TargetModelElementId { get; set; }
	public string AttributeType { get; set; }
	public ElementTsdResult ModelData { get; set; }

	#region Overridden Methods
	public override string ToString()
	{
		return $"{DisplayName}, Scada D #: {ScadaData.Points.Count}, Model D #: {ModelData.Values.Count}";
	}
	#endregion
}
#endregion