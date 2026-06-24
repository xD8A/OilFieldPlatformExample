using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using DynamicData;
using DynamicData.Binding;
using OilFieldPlatform.Calculation.Core.Proxies;

namespace OilFieldPlatform.Calculation.Core.States.UI;

/// <summary>Состояние страницы проб воды.</summary>
public sealed class WaterSamplePageState : IDisposable
{
#pragma warning disable IDE0044, S2933 // _proxyList is reassigned via BindToObservableList out parameter
    private IObservableList<WaterSampleProxyModel> _proxyList = new SourceList<WaterSampleProxyModel>();
#pragma warning restore IDE0044, S2933
    private readonly BehaviorSubject<bool> _isAutoCalc = new(false);
    private readonly IDisposable? _projectSubscription;
    private bool _disposed;

    /// <summary>Конструктор.</summary>
    /// <param name="appState">Состояние приложения.</param>
    public WaterSamplePageState(ApplicationState appState)
    {
        _projectSubscription = appState.ProjectAsObservable
            .Where(p => p is not null)
            .Select(p => p!.WaterSamplesAsObservable.Connect()
                .Transform(s => new WaterSampleProxyModel(s))
                .BindToObservableList(out _proxyList))
            .Switch()
            .Subscribe();
    }

    /// <summary>Список проб воды на странице (реактивный).</summary>
    [JsonIgnore]
    public IObservableList<WaterSampleProxyModel> ListAsObservable => _proxyList;

    /// <summary>Список проб воды на странице.</summary>
    public IEnumerable<WaterSampleProxyModel> List => _proxyList.Items;

    /// <summary>Найти прокси по ключу.</summary>
    /// <param name="key">Ключ пробы.</param>
    /// <returns>Прокси или null.</returns>
    public WaterSampleProxyModel? FindByKey(string key) =>
        (_proxyList as SourceList<WaterSampleProxyModel>)?.Items.FirstOrDefault(p => p.Key == key);


    /// <summary>Флаг автоматического расчёта (реактивный).</summary>
    [JsonIgnore]
    public IObservable<bool> IsAutoCalcAsObservable => _isAutoCalc.AsObservable();

    /// <summary>Флаг автоматического расчёта.</summary>
    public bool IsAutoCalc
    {
        get => _isAutoCalc.Value;
        set => _isAutoCalc.OnNext(value);
    }

    /// <summary>Освобождение ресурсов.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _projectSubscription?.Dispose();
        (_proxyList as IDisposable)?.Dispose();
        _isAutoCalc.Dispose();
        _disposed = true;
    }
}
