using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Domain.Enums;
using OilFieldPlatform.Domain.Projections.Common;
using OilFieldPlatform.Infrastructure.Repositories.ABC;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий чтения для ClusterStationEntity (насосная станция).
/// </summary>
public class ClusterStationReadRepository : ABCRepository<ClusterStationEntity>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public ClusterStationReadRepository(ISession session, ILogger<ClusterStationReadRepository>? logger = null) : base(session, logger)
    {
    }

    private IQueryable<ClusterStationEntity> BuildByTypeQuery(ClusterStationType stationType)
    {
        return _session.Query<ClusterStationEntity>()
            .Where(s => s.StationType == stationType);
    }

    private IQueryOver<ClusterStationEntity, ClusterStationEntity> BuildProjectionsQuery()
    {
        ClusterStationProjection alias = null!;

        return _session.QueryOver<ClusterStationEntity>()
            .SelectList(list => list
                .Select(e => e.Id).WithAlias(() => alias.Id)
                .Select(e => e.Name).WithAlias(() => alias.Name))
            .TransformUsing(Transformers.AliasToBean<ClusterStationProjection>());
    }

    /// <summary>
    /// Получить станции по типу (КНС / ДНС).
    /// </summary>
    /// <param name="stationType">Тип станции.</param>
    /// <returns>Список станций указанного типа.</returns>
    public IEnumerable<ClusterStationEntity> GetByType(ClusterStationType stationType)
    {
        _logger.LogDebug("Getting cluster stations by type: {StationType}", stationType);
        var result = BuildByTypeQuery(stationType).ToList();
        _logger.LogDebug("Found {Count} cluster stations of type: {StationType}", result.Count, stationType);
        return result;
    }

    /// <summary>
    /// Получить все станции (проекции Id + Name).
    /// </summary>
    /// <returns>Список проекций станций.</returns>
    public IEnumerable<ClusterStationProjection> GetProjections()
    {
        _logger.LogDebug("Getting cluster station projections");
        var result = BuildProjectionsQuery().List<ClusterStationProjection>();
        _logger.LogDebug("Retrieved {Count} cluster station projections", result.Count);
        return result;
    }

    /// <summary>
    /// Получить станции по типу (асинхронно).
    /// </summary>
    /// <param name="stationType">Тип станции.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список станций указанного типа.</returns>
    public async Task<IEnumerable<ClusterStationEntity>> GetByTypeAsync(ClusterStationType stationType, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting cluster stations by type async: {StationType}", stationType);
        var result = await BuildByTypeQuery(stationType).ToListAsync(ct);
        _logger.LogDebug("Found {Count} cluster stations of type: {StationType} async", result.Count, stationType);
        return result;
    }

    /// <summary>
    /// Получить все станции (проекции Id + Name) асинхронно.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список проекций станций.</returns>
    public async Task<IEnumerable<ClusterStationProjection>> GetProjectionsAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Getting cluster station projections async");
        var result = await BuildProjectionsQuery().ListAsync<ClusterStationProjection>(ct);
        _logger.LogDebug("Retrieved {Count} cluster station projections async", result.Count);
        return result;
    }
}
