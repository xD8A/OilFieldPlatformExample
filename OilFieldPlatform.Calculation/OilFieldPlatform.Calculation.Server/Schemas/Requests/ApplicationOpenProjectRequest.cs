namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос открытия проекта.</summary>
public sealed class ApplicationOpenProjectRequest : IApplicationRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "application.openProject";

    /// <summary>Идентификатор проекта.</summary>
    public required long Id { get; init; }
}
