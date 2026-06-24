#pragma warning disable S3881 // Full Dispose(bool) pattern not needed — no unmanaged resources, sealed class
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using OilFieldPlatform.Domain.Interfaces;

namespace OilFieldPlatform.Calculation.Core.Models.Data;

/// <summary>Эквивалентные концентрации ионов (мг-экв/л) для пробы воды.</summary>
public sealed class WaterSampleEquivalentData : IAnionSample, ICationSample, IDisposable
{
    private readonly BehaviorSubject<bool> _isOutdated = new(false);
    private bool _disposed;

    /// <summary>Проба воды, к которой относятся эквиваленты.</summary>
    [JsonIgnore]
    public WaterSampleModel Sample { get; }

    /// <summary>Конструктор.</summary>
    /// <param name="sample">Проба воды.</param>
    public WaterSampleEquivalentData(WaterSampleModel sample)
    {
        Sample = sample;
    }

    /// <summary>Реактивный флаг устаревания результата.</summary>
    [JsonIgnore]
    public IObservable<bool> IsOutdatedAsObservable => _isOutdated.AsObservable();
    /// <summary>Флаг устаревания результата.</summary>
    public bool IsOutdated { get => _isOutdated.Value; set => _isOutdated.OnNext(value); }

    private readonly BehaviorSubject<double?> _chloride = new(default);
    /// <summary>Cl⁻ (мг-экв/л).</summary>
    [JsonIgnore]
    public IObservable<double?> ChlorideAsObservable => _chloride.AsObservable();
    /// <summary>Cl⁻ (мг-экв/л).</summary>
    public double? Chloride { get => _chloride.Value; set => _chloride.OnNext(value); }

    private readonly BehaviorSubject<double?> _carbonate = new(default);
    /// <summary>CO₃²⁻ (мг-экв/л).</summary>
    [JsonIgnore]
    public IObservable<double?> CarbonateAsObservable => _carbonate.AsObservable();
    /// <summary>CO₃²⁻ (мг-экв/л).</summary>
    public double? Carbonate { get => _carbonate.Value; set => _carbonate.OnNext(value); }

    private readonly BehaviorSubject<double?> _bicarbonate = new(default);
    /// <summary>HCO₃⁻ (мг-экв/л).</summary>
    [JsonIgnore]
    public IObservable<double?> BicarbonateAsObservable => _bicarbonate.AsObservable();
    /// <summary>HCO₃⁻ (мг-экв/л).</summary>
    public double? Bicarbonate { get => _bicarbonate.Value; set => _bicarbonate.OnNext(value); }

    private readonly BehaviorSubject<double?> _calcium = new(default);
    /// <summary>Ca²⁺ (мг-экв/л).</summary>
    [JsonIgnore]
    public IObservable<double?> CalciumAsObservable => _calcium.AsObservable();
    /// <summary>Ca²⁺ (мг-экв/л).</summary>
    public double? Calcium { get => _calcium.Value; set => _calcium.OnNext(value); }

    private readonly BehaviorSubject<double?> _magnesium = new(default);
    /// <summary>Mg²⁺ (мг-экв/л).</summary>
    [JsonIgnore]
    public IObservable<double?> MagnesiumAsObservable => _magnesium.AsObservable();
    /// <summary>Mg²⁺ (мг-экв/л).</summary>
    public double? Magnesium { get => _magnesium.Value; set => _magnesium.OnNext(value); }

    private readonly BehaviorSubject<double?> _sodium = new(default);
    /// <summary>Na⁺ (мг-экв/л).</summary>
    [JsonIgnore]
    public IObservable<double?> SodiumAsObservable => _sodium.AsObservable();
    /// <summary>Na⁺ (мг-экв/л).</summary>
    public double? Sodium { get => _sodium.Value; set => _sodium.OnNext(value); }

    private readonly BehaviorSubject<double?> _sulfate = new(default);
    /// <summary>SO₄²⁻ (мг-экв/л).</summary>
    [JsonIgnore]
    public IObservable<double?> SulfateAsObservable => _sulfate.AsObservable();
    /// <summary>SO₄²⁻ (мг-экв/л).</summary>
    public double? Sulfate { get => _sulfate.Value; set => _sulfate.OnNext(value); }

    /// <summary>Освобождение ресурсов.</summary>
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
            _isOutdated.Dispose();
            _chloride.Dispose();
            _carbonate.Dispose();
            _bicarbonate.Dispose();
            _calcium.Dispose();
            _magnesium.Dispose();
            _sodium.Dispose();
            _sulfate.Dispose();
        }
        _disposed = true;
    }
}
