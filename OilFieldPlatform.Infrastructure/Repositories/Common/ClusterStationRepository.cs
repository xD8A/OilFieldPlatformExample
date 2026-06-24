using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using NHibernate;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий для ClusterStationEntity (насосная станция) с операциями записи.
/// </summary>
public class ClusterStationRepository : ClusterStationReadRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public ClusterStationRepository(ISession session, ILogger<ClusterStationRepository>? logger = null) : base(session, logger)
    {
    }

    /// <summary>Добавить насосную станцию.</summary>
    /// <param name="entity">Новая насосная станция.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new long Add(ClusterStationEntity entity) => base.Add(entity);
    /// <summary>Обновить насосную станцию.</summary>
    public new void Update(ClusterStationEntity entity) => base.Update(entity);
    /// <summary>Добавить или обновить насосную станцию.</summary>
    /// <param name="entity">Насосная станция для сохранения.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new ClusterStationEntity AddOrUpdate(ClusterStationEntity entity) => base.AddOrUpdate(entity);
    /// <summary>Удалить насосную станцию по Id.</summary>
    public new void Delete(long id) => base.Delete(id);
    /// <summary>Добавить насосную станцию (асинхронно).</summary>
    /// <param name="entity">Новая насосная станция.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new async Task<long> AddAsync(ClusterStationEntity entity, CancellationToken ct = default) => await base.AddAsync(entity, ct);
    /// <summary>Обновить насосную станцию (асинхронно).</summary>
    /// <param name="entity">Насосная станция с изменениями.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task UpdateAsync(ClusterStationEntity entity, CancellationToken ct = default) => await base.UpdateAsync(entity, ct);
    /// <summary>Добавить или обновить насосную станцию (асинхронно).</summary>
    /// <param name="entity">Насосная станция для сохранения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new async Task<ClusterStationEntity> AddOrUpdateAsync(ClusterStationEntity entity, CancellationToken ct = default) => await base.AddOrUpdateAsync(entity, ct);
    /// <summary>Удалить насосную станцию по Id (асинхронно).</summary>
    /// <param name="id">Идентификатор насосной станции.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task DeleteAsync(long id, CancellationToken ct = default) => await base.DeleteAsync(id, ct);
}
