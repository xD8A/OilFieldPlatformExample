using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Domain.Projections.Common;
using OilFieldPlatform.Infrastructure.Repositories.ABC;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий чтения для DevTargetEntity (объект разработки).
/// </summary>
public class DevTargetReadRepository : ABCRepository<DevTargetEntity>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public DevTargetReadRepository(ISession session, ILogger<DevTargetReadRepository>? logger = null) : base(session, logger)
    {
    }

    private IQueryable<DevTargetEntity> BuildByOilFieldIdQuery(long oilFieldId)
    {
        return _session.Query<DevTargetEntity>()
            .Where(d => d.OilField!.Id == oilFieldId);
    }

    private IQueryOver<DevTargetEntity, DevTargetEntity> BuildProjectionsByOilFieldIdQuery(long oilFieldId)
    {
        DevTargetProjection alias = null!;

        return _session.QueryOver<DevTargetEntity>()
            .Where(d => d.OilField!.Id == oilFieldId)
            .SelectList(list => list
                .Select(e => e.Id).WithAlias(() => alias.Id)
                .Select(e => e.Name).WithAlias(() => alias.Name))
            .TransformUsing(Transformers.AliasToBean<DevTargetProjection>());
    }

    /// <summary>
    /// Получить объекты разработки по месторождению.
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <returns>Список объектов разработки.</returns>
    public IEnumerable<DevTargetEntity> GetByOilFieldId(long oilFieldId)
    {
        _logger.LogDebug("Getting dev targets by oil field Id: {OilFieldId}", oilFieldId);
        var result = BuildByOilFieldIdQuery(oilFieldId).ToList();
        _logger.LogDebug("Found {Count} dev targets for oil field Id: {OilFieldId}", result.Count, oilFieldId);
        return result;
    }

    /// <summary>
    /// Получить проекции объектов разработки по месторождению (Id + Name).
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <returns>Список проекций.</returns>
    public IEnumerable<DevTargetProjection> GetProjectionsByOilFieldId(long oilFieldId)
    {
        _logger.LogDebug("Getting dev target projections by oil field Id: {OilFieldId}", oilFieldId);
        var result = BuildProjectionsByOilFieldIdQuery(oilFieldId).List<DevTargetProjection>();
        _logger.LogDebug("Retrieved {Count} dev target projections for oil field Id: {OilFieldId}", result.Count, oilFieldId);
        return result;
    }

    /// <summary>
    /// Получить объекты разработки по месторождению (асинхронно).
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список объектов разработки.</returns>
    public async Task<IEnumerable<DevTargetEntity>> GetByOilFieldIdAsync(long oilFieldId, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting dev targets by oil field Id async: {OilFieldId}", oilFieldId);
        var result = await BuildByOilFieldIdQuery(oilFieldId).ToListAsync(ct);
        _logger.LogDebug("Found {Count} dev targets for oil field Id: {OilFieldId} async", result.Count, oilFieldId);
        return result;
    }

    /// <summary>
    /// Получить проекции объектов разработки по месторождению (асинхронно).
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список проекций.</returns>
    public async Task<IEnumerable<DevTargetProjection>> GetProjectionsByOilFieldIdAsync(long oilFieldId, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting dev target projections by oil field Id async: {OilFieldId}", oilFieldId);
        var result = await BuildProjectionsByOilFieldIdQuery(oilFieldId).ListAsync<DevTargetProjection>(ct);
        _logger.LogDebug("Retrieved {Count} dev target projections for oil field Id: {OilFieldId} async", result.Count, oilFieldId);
        return result;
    }
}
