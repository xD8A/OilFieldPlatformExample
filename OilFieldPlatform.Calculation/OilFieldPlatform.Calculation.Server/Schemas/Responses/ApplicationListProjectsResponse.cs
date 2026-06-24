using OilFieldPlatform.Domain.Projections.Calculation;

namespace OilFieldPlatform.Calculation.Server.Schemas.Responses;

/// <summary>Ответ со списком проектов.</summary>
public sealed class ApplicationListProjectsResponse : IApplicationResponse
{
    /// <summary>Список проектов.</summary>
    public required IList<CalcProjectProjection> Projects { get; init; }
}
