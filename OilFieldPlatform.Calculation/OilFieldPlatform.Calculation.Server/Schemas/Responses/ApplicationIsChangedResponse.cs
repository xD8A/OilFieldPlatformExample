namespace OilFieldPlatform.Calculation.Server.Schemas.Responses;

/// <summary>Уведомление об изменении флага несохранённых изменений.</summary>
public sealed class ApplicationIsChangedResponse : IApplicationResponse
{
    /// <summary>Новое значение флага IsChanged.</summary>
    public required bool IsChanged { get; init; }
}
