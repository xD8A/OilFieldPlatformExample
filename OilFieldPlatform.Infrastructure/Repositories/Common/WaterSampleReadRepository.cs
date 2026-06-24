using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Infrastructure.Repositories.ABC;
using NHibernate;
using NHibernate.Linq;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий чтения для WaterSampleEntity (проба воды).
/// </summary>
public class WaterSampleReadRepository : ABCRepository<WaterSampleEntity>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public WaterSampleReadRepository(ISession session, ILogger<WaterSampleReadRepository>? logger = null) : base(session, logger)
    {
    }

    private IQueryable<WaterSampleEntity> BuildByQuery(long? oilFieldId, long? devTargetId, long? wellId)
    {
        var query = _session.Query<WaterSampleEntity>();

        if (oilFieldId.HasValue)
            query = query.Where(w => w.OilField!.Id == oilFieldId.Value);
        if (devTargetId.HasValue)
            query = query.Where(w => w.DevTarget!.Id == devTargetId.Value);
        if (wellId.HasValue)
            query = query.Where(w => w.Well!.Id == wellId.Value);

        return query;
    }

    /// <summary>
    /// Получить пробы по фильтрам: месторождение, объект разработки, скважина.
    /// </summary>
    /// <param name="oilFieldId">Id месторождения (опционально).</param>
    /// <param name="devTargetId">Id объекта разработки (опционально).</param>
    /// <param name="wellId">Id скважины (опционально).</param>
    /// <returns>Отфильтрованный список проб.</returns>
    public IEnumerable<WaterSampleEntity> GetBy(long? oilFieldId, long? devTargetId, long? wellId)
    {
        _logger.LogDebug("Getting water samples by filters: oilField={OilFieldId}, devTarget={DevTargetId}, well={WellId}", oilFieldId, devTargetId, wellId);
        var result = BuildByQuery(oilFieldId, devTargetId, wellId).ToList();
        _logger.LogDebug("Found {Count} water samples", result.Count);
        return result;
    }

    /// <summary>
    /// Получить пробы по фильтрам (асинхронно).
    /// </summary>
    /// <param name="oilFieldId">Id месторождения (опционально).</param>
    /// <param name="devTargetId">Id объекта разработки (опционально).</param>
    /// <param name="wellId">Id скважины (опционально).</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Отфильтрованный список проб.</returns>
    public async Task<IEnumerable<WaterSampleEntity>> GetByAsync(long? oilFieldId, long? devTargetId, long? wellId, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting water samples by filters async: oilField={OilFieldId}, devTarget={DevTargetId}, well={WellId}", oilFieldId, devTargetId, wellId);
        var result = await BuildByQuery(oilFieldId, devTargetId, wellId).ToListAsync(ct);
        _logger.LogDebug("Found {Count} water samples async", result.Count);
        return result;
    }
}
