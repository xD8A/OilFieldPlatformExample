using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Domain.Projections.Common;
using OilFieldPlatform.Infrastructure.Repositories.ABC;
using NHibernate;
using NHibernate.Transform;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий чтения для OilFieldEntity (месторождение).
/// </summary>
public class OilFieldReadRepository : ABCRepository<OilFieldEntity>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public OilFieldReadRepository(ISession session, ILogger<OilFieldReadRepository>? logger = null) : base(session, logger)
    {
    }

    private IQueryOver<OilFieldEntity, OilFieldEntity> BuildProjectionsQuery()
    {
        OilFieldProjection alias = null!;

        return _session.QueryOver<OilFieldEntity>()
            .SelectList(list => list
                .Select(e => e.Id).WithAlias(() => alias.Id)
                .Select(e => e.Name).WithAlias(() => alias.Name))
            .TransformUsing(Transformers.AliasToBean<OilFieldProjection>());
    }

    /// <summary>
    /// Получить проекции месторождений (Id + Name).
    /// </summary>
    /// <returns>Список проекций месторождений.</returns>
    public IEnumerable<OilFieldProjection> GetProjections()
    {
        _logger.LogDebug("Getting oil field projections");
        var result = BuildProjectionsQuery().List<OilFieldProjection>();
        _logger.LogDebug("Retrieved {Count} oil field projections", result.Count);
        return result;
    }

    /// <summary>
    /// Получить проекции месторождений (асинхронно).
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список проекций месторождений.</returns>
    public async Task<IEnumerable<OilFieldProjection>> GetProjectionsAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Getting oil field projections async");
        var result = await BuildProjectionsQuery().ListAsync<OilFieldProjection>(ct);
        _logger.LogDebug("Retrieved {Count} oil field projections async", result.Count);
        return result;
    }
}
