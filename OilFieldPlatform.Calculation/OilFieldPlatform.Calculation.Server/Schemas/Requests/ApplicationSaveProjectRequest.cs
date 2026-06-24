namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос сохранения текущего проекта.</summary>
public sealed class ApplicationSaveProjectRequest : IApplicationRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "application.saveProject";
}
