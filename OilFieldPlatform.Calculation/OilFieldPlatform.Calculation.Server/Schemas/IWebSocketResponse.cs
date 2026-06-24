using System.Text.Json.Serialization;
using OilFieldPlatform.Calculation.Server.Schemas.Responses;

namespace OilFieldPlatform.Calculation.Server.Schemas;

/// <summary>Базовый интерфейс для всех исходящих WebSocket-сообщений (ответов).</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ApplicationListProjectsResponse), typeDiscriminator: "application.projects")]
[JsonDerivedType(typeof(ApplicationStateResponse), typeDiscriminator: "application.state")]
[JsonDerivedType(typeof(ApplicationProjectOpenedResponse), typeDiscriminator: "application.projectOpened")]
[JsonDerivedType(typeof(ApplicationProjectSavedResponse), typeDiscriminator: "application.projectSaved")]
[JsonDerivedType(typeof(ApplicationProjectClosedResponse), typeDiscriminator: "application.projectClosed")]
[JsonDerivedType(typeof(ApplicationIsChangedResponse), typeDiscriminator: "application.isChanged")]
[JsonDerivedType(typeof(WaterSampleStateResponse), typeDiscriminator: "pages.waterSample.state")]
[JsonDerivedType(typeof(WaterSampleConnectedResponse), typeDiscriminator: "pages.waterSample.connected")]
[JsonDerivedType(typeof(WaterSampleDisconnectedResponse), typeDiscriminator: "pages.waterSample.disconnected")]
[JsonDerivedType(typeof(WaterSampleEditedResponse), typeDiscriminator: "pages.waterSample.edited")]
[JsonDerivedType(typeof(WaterSampleAutoCalcSetResponse), typeDiscriminator: "pages.waterSample.autoCalcSet")]
[JsonDerivedType(typeof(WaterSampleCalculatedResponse), typeDiscriminator: "pages.waterSample.calculated")]
[JsonDerivedType(typeof(WaterSampleChangedResponse), typeDiscriminator: "pages.waterSample.changed")]
[JsonDerivedType(typeof(LogResponse), typeDiscriminator: "application.log")]
public interface IWebSocketResponse
{
}
