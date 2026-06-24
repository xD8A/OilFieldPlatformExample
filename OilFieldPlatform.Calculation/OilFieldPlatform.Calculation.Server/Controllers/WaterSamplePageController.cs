using System.Reactive.Linq;
using System.Text.Json;
using DynamicData;
using Microsoft.Extensions.Logging.Abstractions;
using OilFieldPlatform.Calculation.Core.Models;
using OilFieldPlatform.Calculation.Core.Proxies;
using OilFieldPlatform.Calculation.Core.Services.Calculators;
using OilFieldPlatform.Calculation.Core.States;
using OilFieldPlatform.Calculation.Core.States.UI;
using OilFieldPlatform.Calculation.Server.Schemas;
using OilFieldPlatform.Calculation.Server.Schemas.Requests;
using OilFieldPlatform.Calculation.Server.Schemas.Responses;
using OilFieldPlatform.Calculation.Server.Services;

namespace OilFieldPlatform.Calculation.Server.Controllers;

/// <summary>Контроллер страницы проб воды.</summary>
public sealed class WaterSamplePageController : IWebSocketController, IDisposable
{
    private readonly WaterSamplePageState _state;
    private readonly ILogger _logger;
    private readonly SourceCache<WaterSampleEquivalentCalculator, string> _calcs;
    private readonly IDisposable _connectionSubscription;
    private int _isConnected;
    private bool _disposed;

    /// <summary>Конструктор.</summary>
    /// <param name="appState">Состояние приложения.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public WaterSamplePageController(ApplicationState appState, ILogger<WaterSamplePageController>? logger = null)
    {
        _state = new WaterSamplePageState(appState);
        _logger = new LoggerForwarder((ILogger?)logger ?? NullLogger.Instance, this);
        _calcs = new SourceCache<WaterSampleEquivalentCalculator, string>(c => WaterSampleProxyModel.BuildKey(c.Sample));

        _connectionSubscription = appState.ProjectAsObservable
            .Select(p => p?.WaterSamplesAsObservable.Connect()
                .Transform(s => new WaterSampleEquivalentCalculator(s, _state.IsAutoCalc, _logger))
                .OnItemAdded(calc =>
                {
                    _logger.LogDebug("WaterSample calculator added: {Key}", WaterSampleProxyModel.BuildKey(calc.Sample));
                    calc.Calculated += OnCalculate;
                    calc.AwaitedCalculated += OnAwaitedCalc;
                    _calcs.AddOrUpdate(calc);
                })
                .OnItemRemoved(calc =>
                {
                    var key = WaterSampleProxyModel.BuildKey(calc.Sample);
                    _logger.LogDebug("WaterSample calculator removed: {Key}", key);
                    calc.Calculated = (calc.Calculated - OnCalculate)!;
                    calc.AwaitedCalculated = (calc.AwaitedCalculated - OnAwaitedCalc)!;
                    _calcs.Remove(calc);
                    calc.Dispose();
                })
                ?? Observable.Empty<IChangeSet<WaterSampleEquivalentCalculator>>())
            .Switch()
            .Subscribe();
    }

    /// <summary>Получить состояние страницы проб воды.</summary>
    public WaterSamplePageState GetState()
    {
        _logger.LogDebug("Getting water sample page state");
        return _state;
    }

    /// <summary>Подключиться к реактивному списку проб воды и подписаться на события калькуляторов.</summary>
    public void Connect()
    {
        Interlocked.Exchange(ref _isConnected, 1);
        _logger.LogInformation("Connected, {Count} calculators loaded", _calcs.Count);
    }

    /// <summary>Отключиться и очистить кеш калькуляторов.</summary>
    public void Disconnect()
    {
        Interlocked.Exchange(ref _isConnected, 0);
        _logger.LogInformation("Disconnected");
    }

    /// <summary>Пересчитать все подключенные пробы воды.</summary>
    public void Calculate()
    {
        _logger.LogDebug("Starting calculation for {Count} calculators", _calcs.Count);

        // Операция неблокирующая (только double-арифметика, без I/O), 
        // поэтому обёртка в Thread/Task.Run не требуется.
        foreach (var calc in _calcs.Items)
        {
            calc.Calculate();
        }

        _logger.LogDebug("Calculation completed for all calculators");
    }

    /// <summary>Установить режим авто-расчёта для всех подключенных калькуляторов.</summary>
    /// <param name="isAutoCalc">Если true — включить авто-расчёт.</param>
    public void SetAutoCalc(bool isAutoCalc)
    {
        _logger.LogDebug("Setting auto calc mode to {IsAuto}", isAutoCalc);

        _state.IsAutoCalc = isAutoCalc;

        foreach (var calc in _calcs.Items.ToArray())
        {
            calc.IsAuto = isAutoCalc;
        }

        _logger.LogDebug("Auto calc mode set to {IsAuto} for {Count} calculators", isAutoCalc, _calcs.Count);
    }

    /// <summary>Редактировать свойство пробы воды по ключу.</summary>
    /// <param name="key">Ключ пробы.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <param name="value">Новое значение в JSON.</param>
    public void EditSample(string key, string propertyName, JsonElement value)
    {
        var proxy = _state.FindByKey(key);
        if (proxy is null)
        {
            _logger.LogWarning("WaterSample not found for key: {Key}", key);
            return;
        }

        _logger.LogDebug("Editing WaterSample {Key}: {Property} = {Value}", key, propertyName, value);

        double? floatValue = value.ValueKind is JsonValueKind.Null ? null : value.GetDouble();
        switch (propertyName)
        {
            case "chloride": proxy.Chloride = floatValue; break;
            case "carbonate": proxy.Carbonate = floatValue; break;
            case "bicarbonate": proxy.Bicarbonate = floatValue; break;
            case "sulfate": proxy.Sulfate = floatValue; break;
            case "calcium": proxy.Calcium = floatValue; break;
            case "magnesium": proxy.Magnesium = floatValue; break;
            case "sodium": proxy.Sodium = floatValue; break;
            default:
                _logger.LogWarning("Unknown property: {Property}", propertyName);
                return;
        }

        OnChangedIfConnected(this, new WaterSampleChangedResponse
        {
            Key = proxy.Key,
            IsOutdated = proxy.IsOutdated,
            Properties = new Dictionary<string, double?>()
            {
                [propertyName] = floatValue,
            }
        });
    }

    /// <summary>Событие изменения пробы воды после расчёта.</summary>
    public event EventHandler<IWebSocketResponse>? OnChanged;

    /// <summary>Вызвать OnChanged только если есть активное подключение.</summary>
    private void OnChangedIfConnected(object? sender, IWebSocketResponse response)
    {
        if (_isConnected == 1)
            OnChanged?.Invoke(sender, response);
    }

    /// <summary>Опубликовать лог-сообщение, если подключение активно.</summary>
    /// <param name="response">Лог-сообщение.</param>
    public void PublishLog(LogResponse response)
    {
        OnChangedIfConnected(this, response);
    }

    /// <summary>Обработчик завершения расчёта — снимает флаг IsOutdated и уведомляет подписчиков.</summary>
    private void OnCalculate(object? sender, WaterSampleModel sample)
    {
        _logger.LogDebug("Calculation completed for WaterSample Id: {Id}", sample.Id);
        var key = WaterSampleProxyModel.BuildKey(sample.SampledAt, sample.WaterType, sample.ClusterStationName, sample.WellName);
        var proxy = _state.FindByKey(key);
        if (proxy is not null)
        {
            proxy.IsOutdated = false;
            OnChangedIfConnected(this, new WaterSampleChangedResponse
            {
                Key = proxy.Key,
                IsOutdated = proxy.IsOutdated,
                Properties = new Dictionary<string, double?>
                {
                    ["chlorideEquivalent"] = proxy.ChlorideEquivalent,
                    ["carbonateEquivalent"] = proxy.CarbonateEquivalent,
                    ["bicarbonateEquivalent"] = proxy.BicarbonateEquivalent,
                    ["sulfateEquivalent"] = proxy.SulfateEquivalent,
                    ["calciumEquivalent"] = proxy.CalciumEquivalent,
                    ["magnesiumEquivalent"] = proxy.MagnesiumEquivalent,
                    ["sodiumEquivalent"] = proxy.SodiumEquivalent,
                },
            });
        }
    }

    /// <summary>Обработчик запроса расчёта — устанавливает флаг IsOutdated.</summary>
    private void OnAwaitedCalc(object? sender, WaterSampleModel sample)
    {
        _logger.LogDebug("Calculation awaited for WaterSample Id: {Id}", sample.Id);
        var key = WaterSampleProxyModel.BuildKey(sample.SampledAt, sample.WaterType, sample.ClusterStationName, sample.WellName);
        var proxy = _state.FindByKey(key);
        if (proxy is not null)
            proxy.IsOutdated = true;
    }

    /// <summary>Обработать запрос состояния страницы проб воды.</summary>
    public WaterSampleStateResponse HandleGetState()
    {
        return new WaterSampleStateResponse { State = _state };
    }

    /// <summary>Обработать запрос подключения к реактивному списку проб.</summary>
    public WaterSampleConnectedResponse HandleConnect()
    {
        Connect();
        return new WaterSampleConnectedResponse();
    }

    /// <summary>Обработать запрос отключения от реактивного списка проб.</summary>
    public WaterSampleDisconnectedResponse HandleDisconnect()
    {
        Disconnect();
        return new WaterSampleDisconnectedResponse();
    }

    /// <summary>Обработать запрос редактирования ионного состава пробы.</summary>
    public WaterSampleEditedResponse HandleEdit(WaterSampleEditRequest request)
    {
        EditSample(request.Key, request.Property, request.Value);
        return new WaterSampleEditedResponse { Key = request.Key, Property = request.Property };
    }

    /// <summary>Обработать запрос установки режима авто-расчёта.</summary>
    public WaterSampleAutoCalcSetResponse HandleSetAutoCalc(WaterSampleSetAutoCalcRequest request)
    {
        SetAutoCalc(request.IsAuto);
        return new WaterSampleAutoCalcSetResponse { IsAuto = request.IsAuto };
    }

    /// <summary>Обработать запрос пересчёта эквивалентов всех проб.</summary>
    public WaterSampleCalculatedResponse HandleCalculate()
    {
        Calculate();
        return new WaterSampleCalculatedResponse();
    }

    /// <summary>Обработать запрос.</summary>
    public IWebSocketResponse? HandleRequest(IWebSocketRequest request)
    {
        IWebSocketResponse? response = request switch
        {
            WaterSampleGetStateRequest _ => HandleGetState(),
            WaterSampleConnectRequest _ => HandleConnect(),
            WaterSampleDisconnectRequest _ => HandleDisconnect(),
            WaterSampleEditRequest r => HandleEdit(r),
            WaterSampleSetAutoCalcRequest r => HandleSetAutoCalc(r),
            WaterSampleCalculateRequest _ => HandleCalculate(),
            _ => null
        };
        return response;
    }

    /// <summary>Освобождение ресурсов.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _logger.LogDebug("Disposing WaterSamplePageController");
        _connectionSubscription.Dispose();
        _calcs.Dispose();
        _state.Dispose();
        _disposed = true;
    }
}
