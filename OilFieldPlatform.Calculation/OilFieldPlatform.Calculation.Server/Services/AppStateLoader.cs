using System.Text.Json;
using OilFieldPlatform.Calculation.Core.Models;
using OilFieldPlatform.Calculation.Core.States;
using OilFieldPlatform.Domain.Enums;
using StackExchange.Redis;

namespace OilFieldPlatform.Calculation.Server.Services;

/// <summary>Сервис сохранения и восстановления несохранённого проекта по sessionId через Redis.</summary>
public sealed class AppStateLoader
{
    private readonly ConnectionMultiplexer _redis;
    private readonly ILogger<AppStateLoader> _logger;

    private const string KeyPrefix = "session:";
    private static readonly TimeSpan KeyTtl = TimeSpan.FromDays(7);

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    // ── Snapshots for serialization ──

    private sealed record OilFieldSnapshot(long Id, string Name);
    private sealed record DevTargetSnapshot(long Id, string Name);

    private sealed record WaterSampleSnapshot(
        long? Id, long? SourceSampleId, DateTime SampledAt, WaterType WaterType,
        string? ClusterStationName, string? WellName,
        double? Chloride, double? Carbonate, double? Bicarbonate,
        double? Sulfate, double? Calcium, double? Magnesium, double? Sodium,
        bool IsOutdated, double? ChlorideEquivalent, double? CarbonateEquivalent,
        double? BicarbonateEquivalent, double? SulfateEquivalent,
        double? CalciumEquivalent, double? MagnesiumEquivalent, double? SodiumEquivalent);

    private sealed record ProjectSnapshot(
        long? Id, OilFieldSnapshot OilField, DevTargetSnapshot DevTarget,
        IReadOnlyList<WaterSampleSnapshot> WaterSamples, bool IsChanged);

    private sealed record StateSnapshot(ProjectSnapshot? Project);

    /// <summary>Конструктор.</summary>
    public AppStateLoader(ConnectionMultiplexer redis, ILogger<AppStateLoader> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    /// <summary>Сохранить состояние приложения по sessionId.</summary>
    public async Task SaveAsync(string sessionId, ApplicationState state)
    {
        var db = _redis.GetDatabase();
        var key = $"{KeyPrefix}{sessionId}:state";
        var snapshot = ToSnapshot(state);
        var json = JsonSerializer.Serialize(snapshot, _jsonOptions);
        await db.StringSetAsync(key, json, KeyTtl);
        _logger.LogDebug("Session {SessionId}: state saved ({Length} chars)", sessionId, json.Length);
    }

    /// <summary>Восстановить ApplicationState из Redis по sessionId.</summary>
    public async Task<ApplicationState?> LoadAsync(string sessionId)
    {
        var db = _redis.GetDatabase();
        var key = $"{KeyPrefix}{sessionId}:state";
        var json = await db.StringGetAsync(key);

        if (json.IsNullOrEmpty)
        {
            _logger.LogDebug("Session {SessionId}: no saved state", sessionId);
            return null;
        }

        StateSnapshot? snapshot;
        try
        {
            snapshot = JsonSerializer.Deserialize<StateSnapshot>(json!, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Session {SessionId}: failed to deserialize state", sessionId);
            return null;
        }

        if (snapshot is null)
        {
            _logger.LogWarning("Session {SessionId}: deserialized snapshot is null", sessionId);
            return null;
        }

        var state = FromSnapshot(snapshot);
        _logger.LogInformation("Session {SessionId}: state restored", sessionId);
        return state;
    }

    // ── Mapping ──

    private static StateSnapshot ToSnapshot(ApplicationState state)
    {
        var project = state.Project;
        if (project is null)
            return new StateSnapshot(null);

        return new StateSnapshot(new ProjectSnapshot(
            project.Id,
            new OilFieldSnapshot(project.OilField.Id, project.OilField.Name),
            new DevTargetSnapshot(project.DevTarget.Id, project.DevTarget.Name),
            project.WaterSamples.Select(ToSnapshot).ToList(),
            project.IsChanged));
    }

    private static WaterSampleSnapshot ToSnapshot(WaterSampleModel s)
    {
        var eq = s.Equivalent;
        return new WaterSampleSnapshot(
            s.Id, s.SourceSampleId, s.SampledAt, s.WaterType,
            s.ClusterStationName, s.WellName,
            s.Chloride, s.Carbonate, s.Bicarbonate,
            s.Sulfate, s.Calcium, s.Magnesium, s.Sodium,
            eq.IsOutdated, eq.Chloride, eq.Carbonate, eq.Bicarbonate,
            eq.Sulfate, eq.Calcium, eq.Magnesium, eq.Sodium);
    }

    private static ApplicationState FromSnapshot(StateSnapshot snapshot)
    {
        if (snapshot.Project is not { } ps)
            return new ApplicationState();

        var oilField = new OilFieldModel(ps.OilField.Name, ps.OilField.Id);
        var devTarget = new DevTargetModel(ps.DevTarget.Name, ps.DevTarget.Id);

        var project = new ProjectModel(oilField, devTarget, null, ps.Id);

        foreach (var ws in ps.WaterSamples)
            project.AddWaterSample(FromSnapshot(ws, project));

        if (ps.IsChanged)
            project.MarkChanged();
        else
            project.MarkUnchanged();

        return new ApplicationState { Project = project };
    }

    private static WaterSampleModel FromSnapshot(WaterSampleSnapshot ws, ProjectModel project)
    {
        var sample = new WaterSampleModel(project, ws.SampledAt, ws.WaterType, ws.ClusterStationName, ws.WellName, ws.SourceSampleId, ws.Id)
        {
            Chloride = ws.Chloride,
            Carbonate = ws.Carbonate,
            Bicarbonate = ws.Bicarbonate,
            Sulfate = ws.Sulfate,
            Calcium = ws.Calcium,
            Magnesium = ws.Magnesium,
            Sodium = ws.Sodium,
        };

        sample.Equivalent.IsOutdated = ws.IsOutdated;
        sample.Equivalent.Chloride = ws.ChlorideEquivalent;
        sample.Equivalent.Carbonate = ws.CarbonateEquivalent;
        sample.Equivalent.Bicarbonate = ws.BicarbonateEquivalent;
        sample.Equivalent.Sulfate = ws.SulfateEquivalent;
        sample.Equivalent.Calcium = ws.CalciumEquivalent;
        sample.Equivalent.Magnesium = ws.MagnesiumEquivalent;
        sample.Equivalent.Sodium = ws.SodiumEquivalent;

        return sample;
    }
}
