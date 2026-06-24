namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос закрытия текущего проекта.</summary>
public sealed class ApplicationCloseProjectRequest : IApplicationRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "application.closeProject";
}
