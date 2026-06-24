using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Domain.Projections.Common;
using OilFieldPlatform.Infrastructure.Repositories.ABC;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий чтения для WellEntity (скважина).
/// </summary>
public class WellReadRepository : ABCRepository<WellEntity>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public WellReadRepository(ISession session, ILogger<WellReadRepository>? logger = null) : base(session, logger)
    {
    }

    private IQueryable<WellEntity> BuildByOilFieldIdQuery(long oilFieldId)
    {
        return _session.Query<WellEntity>()
            .Where(w => w.OilField.Id == oilFieldId);
    }

    private IQueryOver<WellEntity, WellEntity> BuildOilFieldProjectionsQuery(long oilFieldId)
    {
        WellProjection alias = null!;

        return _session.QueryOver<WellEntity>()
            .Where(w => w.OilField.Id == oilFieldId)
            .SelectList(list => list
                .Select(e => e.Id).WithAlias(() => alias.Id)
                .Select(e => e.Name).WithAlias(() => alias.Name))
            .TransformUsing(Transformers.AliasToBean<WellProjection>());
    }

    /// <summary>
    /// Получить скважины по месторождению.
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <returns>Список скважин.</returns>
    public IEnumerable<WellEntity> GetByOilFieldId(long oilFieldId)
    {
        _logger.LogDebug("Getting wells by oil field Id: {OilFieldId}", oilFieldId);
        var result = BuildByOilFieldIdQuery(oilFieldId).ToList();
        _logger.LogDebug("Found {Count} wells for oil field Id: {OilFieldId}", result.Count, oilFieldId);
        return result;
    }

    /// <summary>
    /// Получить проекции скважин по месторождению (Id + Name).
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <returns>Список проекций скважин.</returns>
    public IEnumerable<WellProjection> GetOilFieldProjections(long oilFieldId)
    {
        _logger.LogDebug("Getting well projections by oil field Id: {OilFieldId}", oilFieldId);
        var result = BuildOilFieldProjectionsQuery(oilFieldId).List<WellProjection>();
        _logger.LogDebug("Retrieved {Count} well projections for oil field Id: {OilFieldId}", result.Count, oilFieldId);
        return result;
    }

    /// <summary>
    /// Получить скважины по месторождению (асинхронно).
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список скважин.</returns>
    public async Task<IEnumerable<WellEntity>> GetByOilFieldIdAsync(long oilFieldId, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting wells by oil field Id async: {OilFieldId}", oilFieldId);
        var result = await BuildByOilFieldIdQuery(oilFieldId).ToListAsync(ct);
        _logger.LogDebug("Found {Count} wells for oil field Id: {OilFieldId} async", result.Count, oilFieldId);
        return result;
    }

    /// <summary>
    /// Получить проекции скважин по месторождению (асинхронно).
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список проекций скважин.</returns>
    public async Task<IEnumerable<WellProjection>> GetOilFieldProjectionsAsync(long oilFieldId, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting well projections by oil field Id async: {OilFieldId}", oilFieldId);
        var result = await BuildOilFieldProjectionsQuery(oilFieldId).ListAsync<WellProjection>(ct);
        _logger.LogDebug("Retrieved {Count} well projections for oil field Id: {OilFieldId} async", result.Count, oilFieldId);
        return result;
    }
}
