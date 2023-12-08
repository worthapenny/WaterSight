using Haestad.Support.User;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;

namespace WaterSight.Model.Extensions;

public static class UserNotificationExtensions
{
    public static string ToString(this IUserNotification un, IWaterModel waterModel)
    {
        var targetElement = (waterModel.Element(un.ElementId) as IWaterElement);
        return $"[{un.Level}] Element: '{un.ElementId}: {un.Label}', Type: {targetElement?.WaterElementType}, Scenario: {waterModel.ActiveScenario.IdLabel()}, Msg: {un.MessageKey}, Params: [{string.Join("|", un.Parameters)}]";
        //return $"{targetElement.IdLabel()} | {targetElement.ModelElementType.ToString()} | Level: {un.Level} | Scenario: {waterModel.ActiveScenario.IdLabel()}";
    }
}
