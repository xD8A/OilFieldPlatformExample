using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using NHibernate;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий для WellEntity (скважина) с операциями записи.
/// </summary>
public class WellRepository : WellReadRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public WellRepository(ISession session, ILogger<WellRepository>? logger = null) : base(session, logger)
    {
    }

    /// <summary>Добавить скважину.</summary>
    /// <param name="entity">Новая скважина.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new long Add(WellEntity entity) => base.Add(entity);
    /// <summary>Обновить скважину.</summary>
    public new void Update(WellEntity entity) => base.Update(entity);
    /// <summary>Добавить или обновить скважину.</summary>
    /// <param name="entity">Скважина для сохранения.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new WellEntity AddOrUpdate(WellEntity entity) => base.AddOrUpdate(entity);
    /// <summary>Удалить скважину по Id.</summary>
    public new void Delete(long id) => base.Delete(id);
    /// <summary>Добавить скважину (асинхронно).</summary>
    /// <param name="entity">Новая скважина.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new async Task<long> AddAsync(WellEntity entity, CancellationToken ct = default) => await base.AddAsync(entity, ct);
    /// <summary>Обновить скважину (асинхронно).</summary>
    /// <param name="entity">Скважина с изменениями.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task UpdateAsync(WellEntity entity, CancellationToken ct = default) => await base.UpdateAsync(entity, ct);
    /// <summary>Добавить или обновить скважину (асинхронно).</summary>
    /// <param name="entity">Скважина для сохранения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new async Task<WellEntity> AddOrUpdateAsync(WellEntity entity, CancellationToken ct = default) => await base.AddOrUpdateAsync(entity, ct);
    /// <summary>Удалить скважину по Id (асинхронно).</summary>
    /// <param name="id">Идентификатор скважины.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task DeleteAsync(long id, CancellationToken ct = default) => await base.DeleteAsync(id, ct);
}
