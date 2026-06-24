#pragma warning disable S1210, CA1036 // CompareTo only for sorting, no operators
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using OilFieldPlatform.Calculation.Core.Models.Data;
using OilFieldPlatform.Domain.Enums;
using OilFieldPlatform.Domain.Interfaces;

namespace OilFieldPlatform.Calculation.Core.Models;

/// <summary>Проба воды (расчётная модель).</summary>
public sealed class WaterSampleModel : IAnionSample, ICationSample, IComparable<WaterSampleModel>, IDisposable
{
    private bool _disposed;

    /// <summary>Идентификатор пробы.</summary>
    public long? Id { get; internal set; }

    /// <summary>Расчётный проект.</summary>
    [JsonIgnore]
    public ProjectModel Project { get; }

    /// <summary>Id исходной пробы из БД.</summary>
    public long? SourceSampleId { get; }

    /// <summary>Дата отбора пробы.</summary>
    public DateTime SampledAt { get; }

    /// <summary>Тип воды (пластовая / закачиваемая).</summary>
    public WaterType WaterType { get; }

    /// <summary>Наименование насосной станции (КНС / ДНС).</summary>
    public string? ClusterStationName { get; }

    /// <summary>Имя скважины.</summary>
    public string? WellName { get; }

    /// <summary>Отображаемое имя пробы.</summary>
    public string DisplayName => $"{SampledAt:yyyy-MM-dd} {WaterType} {ClusterStationName ?? WellName ?? "(?)"}";

    /// <summary>Расчётные эквиваленты ионов.</summary>
    public WaterSampleEquivalentData Equivalent { get; }

    /// <summary>Конструктор для NHibernate (new() constraint).</summary>
    public WaterSampleModel()
    {
        Project = null!;
        Equivalent = new WaterSampleEquivalentData(this);
    }

    /// <summary>Конструктор для прикладного кода.</summary>
    /// <param name="project">Расчётный проект.</param>
    /// <param name="sampledAt">Дата отбора.</param>
    /// <param name="waterType">Тип воды.</param>
    /// <param name="clusterStationName">Наименование насосной станции.</param>
    /// <param name="wellName">Имя скважины.</param>
    /// <param name="sourceSampleId">Id исходной пробы.</param>
    /// <param name="id">Идентификатор пробы.</param>
    public WaterSampleModel(
        ProjectModel project,
        DateTime sampledAt,
        WaterType waterType,
        string? clusterStationName = null,
        string? wellName = null,
        long? sourceSampleId = null,
        long? id = null)
    {
        Project = project;
        SampledAt = sampledAt;
        WaterType = waterType;
        ClusterStationName = clusterStationName;
        WellName = wellName;
        SourceSampleId = sourceSampleId;
        Id = id;
        Equivalent = new WaterSampleEquivalentData(this);
    }

    private readonly BehaviorSubject<double?> _chloride = new(default);
    /// <summary>Cl⁻ (мг/л).</summary>
    [JsonIgnore]
    public IObservable<double?> ChlorideAsObservable => _chloride.AsObservable();
    /// <summary>Cl⁻ (мг/л).</summary>
    public double? Chloride { get => _chloride.Value; set => _chloride.OnNext(value); }

    private readonly BehaviorSubject<double?> _carbonate = new(default);
    /// <summary>CO₃²⁻ (мг/л).</summary>
    [JsonIgnore]
    public IObservable<double?> CarbonateAsObservable => _carbonate.AsObservable();
    /// <summary>CO₃²⁻ (мг/л).</summary>
    public double? Carbonate { get => _carbonate.Value; set => _carbonate.OnNext(value); }

    private readonly BehaviorSubject<double?> _bicarbonate = new(default);
    /// <summary>HCO₃⁻ (мг/л).</summary>
    [JsonIgnore]
    public IObservable<double?> BicarbonateAsObservable => _bicarbonate.AsObservable();
    /// <summary>HCO₃⁻ (мг/л).</summary>
    public double? Bicarbonate { get => _bicarbonate.Value; set => _bicarbonate.OnNext(value); }

    private readonly BehaviorSubject<double?> _calcium = new(default);
    /// <summary>Ca²⁺ (мг/л).</summary>
    [JsonIgnore]
    public IObservable<double?> CalciumAsObservable => _calcium.AsObservable();
    /// <summary>Ca²⁺ (мг/л).</summary>
    public double? Calcium { get => _calcium.Value; set => _calcium.OnNext(value); }

    private readonly BehaviorSubject<double?> _magnesium = new(default);
    /// <summary>Mg²⁺ (мг/л).</summary>
    [JsonIgnore]
    public IObservable<double?> MagnesiumAsObservable => _magnesium.AsObservable();
    /// <summary>Mg²⁺ (мг/л).</summary>
    public double? Magnesium { get => _magnesium.Value; set => _magnesium.OnNext(value); }

    private readonly BehaviorSubject<double?> _sodium = new(default);
    /// <summary>Na⁺ (мг/л).</summary>
    [JsonIgnore]
    public IObservable<double?> SodiumAsObservable => _sodium.AsObservable();
    /// <summary>Na⁺ (мг/л).</summary>
    public double? Sodium { get => _sodium.Value; set => _sodium.OnNext(value); }

    private readonly BehaviorSubject<double?> _sulfate = new(default);
    /// <summary>SO₄²⁻ (мг/л).</summary>
    [JsonIgnore]
    public IObservable<double?> SulfateAsObservable => _sulfate.AsObservable();
    /// <summary>SO₄²⁻ (мг/л).</summary>
    public double? Sulfate { get => _sulfate.Value; set => _sulfate.OnNext(value); }

    /// <summary>Сравнение проб по дате, типу воды, насосной станции, скважине.</summary>
    /// <param name="other">Другая проба.</param>
    /// <returns>Результат сравнения (-1, 0, 1).</returns>
    public int CompareTo(WaterSampleModel? other)
    {
        if (other is null) return 1;
        var dateComparison = SampledAt.CompareTo(other.SampledAt);
        if (dateComparison != 0) return dateComparison;
        var waterTypeComparison = WaterType.CompareTo(other.WaterType);
        if (waterTypeComparison != 0) return waterTypeComparison;
        var stationComparison = string.Compare(ClusterStationName, other.ClusterStationName, StringComparison.Ordinal);
        if (stationComparison != 0) return stationComparison;
        var wellComparison = string.Compare(WellName, other.WellName, StringComparison.Ordinal);
        return wellComparison;
    }

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
            _chloride.Dispose();
            _carbonate.Dispose();
            _bicarbonate.Dispose();
            _calcium.Dispose();
            _magnesium.Dispose();
            _sodium.Dispose();
            _sulfate.Dispose();
            Equivalent?.Dispose();
        }
        _disposed = true;
    }
}
