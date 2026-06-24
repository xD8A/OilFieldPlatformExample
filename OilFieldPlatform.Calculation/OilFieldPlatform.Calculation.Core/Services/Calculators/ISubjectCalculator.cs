namespace OilFieldPlatform.Calculation.Core.Services.Calculators;

/// <summary>Интерфейс калькулятора для расчёта показателей субъекта.</summary>
/// <typeparam name="TSubject">Тип субъекта расчёта.</typeparam>
public interface ISubjectCalculator<TSubject> : IDisposable where TSubject : new()
{
    /// <summary>Событие завершения расчёта.</summary>
    public EventHandler<TSubject> Calculated { get; }

    /// <summary>Событие запроса расчёта (ожидает подтверждения).</summary>
    public EventHandler<TSubject> AwaitedCalculated { get; }

    /// <summary>Автоматический пересчёт при изменении входных данных.</summary>
    public bool IsAuto { get; set; }

    /// <summary>Запустить расчёт.</summary>
    /// <returns>true, если расчёт выполнен успешно.</returns>
    public bool Calculate();
}
