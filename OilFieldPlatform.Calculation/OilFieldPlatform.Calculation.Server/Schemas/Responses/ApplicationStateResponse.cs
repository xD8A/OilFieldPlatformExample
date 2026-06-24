using OilFieldPlatform.Calculation.Core.States.UI;

namespace OilFieldPlatform.Calculation.Server.Schemas.Responses;

/// <summary>Ответ с полным состоянием приложения.</summary>
public sealed class ApplicationStateResponse : IApplicationResponse
{
    /// <summary>Состояние приложения.</summary>
    public required ApplicationHeaderState State { get; init; }
}
