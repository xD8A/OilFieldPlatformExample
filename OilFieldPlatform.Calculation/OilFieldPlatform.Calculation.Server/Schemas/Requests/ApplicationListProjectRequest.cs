namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос списка проектов.</summary>
public sealed class ApplicationListProjectRequest : IApplicationRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "application.listProjects";
}
