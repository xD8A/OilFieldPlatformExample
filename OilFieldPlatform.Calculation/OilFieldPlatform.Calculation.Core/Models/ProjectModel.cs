using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using DynamicData;

namespace OilFieldPlatform.Calculation.Core.Models;

/// <summary>Расчётный проект.</summary>
public sealed class ProjectModel : IDisposable
{
    /// <summary>Список проб воды в проекте (реактивный).</summary>
    [JsonIgnore]
    public SourceList<WaterSampleModel> WaterSamplesAsObservable { get; }

    private readonly BehaviorSubject<bool> _isChanged = new(false);
    private readonly IDisposable _changeSubscription;
    private bool _disposed;

    /// <summary>Идентификатор проекта.</summary>
    public long? Id { get; internal set; }

    /// <summary>Месторождение.</summary>
    public OilFieldModel OilField { get; }

    /// <summary>Объект разработки.</summary>
    public DevTargetModel DevTarget { get; }

    /// <summary>Название проекта.</summary>
    public string Name => $"{OilField.Name}: {DevTarget.Name}";

    /// <summary>Флаг наличия несохранённых изменений (реактивный).</summary>
    [JsonIgnore]
    public IObservable<bool> IsChangedAsObservable => _isChanged.AsObservable();

    /// <summary>Флаг наличия несохранённых изменений.</summary>
    [JsonIgnore]
    public bool IsChanged => _isChanged.Value;

    /// <summary>Конструктор.</summary>
    /// <param name="oilField">Месторождение.</param>
    /// <param name="devTarget">Объект разработки.</param>
    /// <param name="waterSamples">Начальный список проб (опционально).</param>
    /// <param name="id">Идентификатор проекта (опционально).</param>
    public ProjectModel(OilFieldModel oilField, DevTargetModel devTarget, IList<WaterSampleModel>? waterSamples = null, long? id = null)
    {
        OilField = oilField;
        DevTarget = devTarget;
        WaterSamplesAsObservable = new SourceList<WaterSampleModel>();

        _changeSubscription = WaterSamplesAsObservable.Connect()
            .SubscribeMany(sample =>
            {
                var merged = Observable.Merge(
                    sample.ChlorideAsObservable.Select(_ => Unit.Default),
                    sample.CarbonateAsObservable.Select(_ => Unit.Default),
                    sample.BicarbonateAsObservable.Select(_ => Unit.Default),
                    sample.SulfateAsObservable.Select(_ => Unit.Default),
                    sample.CalciumAsObservable.Select(_ => Unit.Default),
                    sample.MagnesiumAsObservable.Select(_ => Unit.Default),
                    sample.SodiumAsObservable.Select(_ => Unit.Default)
                );

                if (sample.Equivalent is { } eq)
                {
                    merged = Observable.Merge(
                        merged,
                        eq.ChlorideAsObservable.Select(_ => Unit.Default),
                        eq.CarbonateAsObservable.Select(_ => Unit.Default),
                        eq.BicarbonateAsObservable.Select(_ => Unit.Default),
                        eq.SulfateAsObservable.Select(_ => Unit.Default),
                        eq.CalciumAsObservable.Select(_ => Unit.Default),
                        eq.MagnesiumAsObservable.Select(_ => Unit.Default),
                        eq.SodiumAsObservable.Select(_ => Unit.Default)
                    );
                }

                return merged.Subscribe(_ => MarkChanged());
            })
            .Subscribe(_ => MarkChanged());

        if (waterSamples is { Count: > 0 })
            foreach (var sample in waterSamples)
                AddWaterSample(sample);

        MarkUnchanged();

        Id = id;
    }

    /// <summary>Добавить пробу с сохранением сортировки по дате, типу воды, насосной станции, скважине.</summary>
    /// <param name="sample">Проба для добавления.</param>
    /// <exception cref="InvalidOperationException">Проба с такими же датой, скважиной и типом воды уже существует.</exception>
    public void AddWaterSample(WaterSampleModel sample)
    {
        WaterSamplesAsObservable.Edit(inner =>
        {
            var index = inner.BinarySearch(sample);
            if (index >= 0)
                throw new InvalidOperationException(
                    $"Проба с датой {sample.SampledAt:s}, типом {sample.WaterType}, насосной станцией '{sample.ClusterStationName}', скважиной '{sample.WellName}' уже существует.");

            inner.Insert(~index, sample);
        });
    }

    /// <summary>Список проб воды в проекте.</summary>
    public IEnumerable<WaterSampleModel> WaterSamples => WaterSamplesAsObservable.Items;

    /// <summary>Отметить проект как изменённый.</summary>
    public void MarkChanged() => _isChanged.OnNext(true);

    /// <summary>Отметить проект как неизменённый (например, после сохранения).</summary>
    public void MarkUnchanged() => _isChanged.OnNext(false);

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
            _changeSubscription.Dispose();
            WaterSamplesAsObservable.Dispose();
            _isChanged.Dispose();
        }
        _disposed = true;
    }
}
