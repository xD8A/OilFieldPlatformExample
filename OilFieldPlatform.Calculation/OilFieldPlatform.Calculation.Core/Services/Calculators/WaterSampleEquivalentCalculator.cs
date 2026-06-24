using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OilFieldPlatform.Calculation.Core.Models;

namespace OilFieldPlatform.Calculation.Core.Services.Calculators;

/// <summary>Калькулятор эквивалентных концентраций ионов для пробы воды.</summary>
public sealed class WaterSampleEquivalentCalculator : ISubjectCalculator<WaterSampleModel>
{
    private readonly ILogger _logger;
    private readonly List<IDisposable> _subscriptions = [];
    private bool _disposed;

    /// <summary>Событие завершения расчёта.</summary>
    public EventHandler<WaterSampleModel> Calculated { get; set; } = delegate { };

    /// <summary>Событие запроса расчёта.</summary>
    public EventHandler<WaterSampleModel> AwaitedCalculated { get; set; } = delegate { };

    /// <summary>Автоматический пересчёт при изменении входных данных.</summary>
    public bool IsAuto { get; set; }

    /// <summary>Проба воды.</summary>
    public WaterSampleModel Sample { get; }

    /// <summary>Конструктор.</summary>
    /// <param name="sample">Проба воды для расчёта.</param>
    /// <param name="isAuto">Автоматический пересчёт.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public WaterSampleEquivalentCalculator(WaterSampleModel sample, bool isAuto = false, ILogger? logger = null)
    {
        Sample = sample;
        _logger = logger ?? NullLogger.Instance;
        IsAuto = isAuto;

        _logger.LogDebug("Initializing calculator for WaterSample Id: {Id}, IsAuto: {IsAuto}", sample.Id, isAuto);

        Action onChanged = () =>
        {
            AwaitedCalculated.Invoke(this, Sample);
            if (IsAuto) Calculate();
        };

        _subscriptions.Add(sample.ChlorideAsObservable.Subscribe(_ => onChanged()));
        _subscriptions.Add(sample.CarbonateAsObservable.Subscribe(_ => onChanged()));
        _subscriptions.Add(sample.BicarbonateAsObservable.Subscribe(_ => onChanged()));
        _subscriptions.Add(sample.SulfateAsObservable.Subscribe(_ => onChanged()));
        _subscriptions.Add(sample.CalciumAsObservable.Subscribe(_ => onChanged()));
        _subscriptions.Add(sample.MagnesiumAsObservable.Subscribe(_ => onChanged()));
        _subscriptions.Add(sample.SodiumAsObservable.Subscribe(_ => onChanged()));

        _logger.LogDebug("Calculator subscriptions initialized: {Count} ion observables", _subscriptions.Count);
    }

    /// <summary>Запустить расчёт эквивалентов.</summary>
    /// <returns>true.</returns>
    public bool Calculate()
    {
        _logger.LogDebug("Calculating equivalents for WaterSample Id: {Id}", Sample.Id);
        Calculate(Sample, _logger);
        Calculated.Invoke(this, Sample);
        _logger.LogDebug("Equivalents calculated for WaterSample Id: {Id}", Sample.Id);
        return true;
    }

    private static void Calculate(WaterSampleModel sample, ILogger logger)
    {
        var equivalent = sample.Equivalent;
        if (equivalent is null)
        {
            logger.LogWarning("Equivalent is null for WaterSample Id: {Id}, skipping calculation", sample.Id);
            return;
        }

        equivalent.Chloride = sample.Chloride is not null
            ? sample.Chloride / EquivalentWeights.Chloride
            : LogNull(logger, sample.Id, "Chloride");
        equivalent.Carbonate = sample.Carbonate is not null
            ? sample.Carbonate / EquivalentWeights.Carbonate
            : LogNull(logger, sample.Id, "Carbonate");
        equivalent.Bicarbonate = sample.Bicarbonate is not null
            ? sample.Bicarbonate / EquivalentWeights.Bicarbonate
            : LogNull(logger, sample.Id, "Bicarbonate");
        equivalent.Sulfate = sample.Sulfate is not null
            ? sample.Sulfate / EquivalentWeights.Sulfate
            : LogNull(logger, sample.Id, "Sulfate");
        equivalent.Calcium = sample.Calcium is not null
            ? sample.Calcium / EquivalentWeights.Calcium
            : LogNull(logger, sample.Id, "Calcium");
        equivalent.Magnesium = sample.Magnesium is not null
            ? sample.Magnesium / EquivalentWeights.Magnesium
            : LogNull(logger, sample.Id, "Magnesium");
        equivalent.Sodium = sample.Sodium is not null
            ? sample.Sodium / EquivalentWeights.Sodium
            : LogNull(logger, sample.Id, "Sodium");
    }

    private static double? LogNull(ILogger logger, long? sampleId, string ionName)
    {
        logger.LogWarning("Ion {IonName} is null for WaterSample Id: {Id}, equivalent set to null", ionName, sampleId);
        return null;
    }

    /// <summary>Освобождение ресурсов (отписка от observable).</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _logger.LogDebug("Disposing calculator for WaterSample Id: {Id}", Sample.Id);
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }
        _disposed = true;
    }
}
