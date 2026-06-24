using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Common;
using NHibernate;

namespace OilFieldPlatform.Infrastructure.Repositories.Common;

/// <summary>
/// Репозиторий для DevTargetEntity (объект разработки) с операциями записи.
/// </summary>
public class DevTargetRepository : DevTargetReadRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public DevTargetRepository(ISession session, ILogger<DevTargetRepository>? logger = null) : base(session, logger)
    {
    }

    /// <summary>Добавить объект разработки.</summary>
    /// <param name="entity">Новый объект разработки.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new long Add(DevTargetEntity entity) => base.Add(entity);
    /// <summary>Обновить объект разработки.</summary>
    public new void Update(DevTargetEntity entity) => base.Update(entity);
    /// <summary>Добавить или обновить объект разработки.</summary>
    /// <param name="entity">Объект разработки для сохранения.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new DevTargetEntity AddOrUpdate(DevTargetEntity entity) => base.AddOrUpdate(entity);
    /// <summary>Удалить объект разработки по Id.</summary>
    public new void Delete(long id) => base.Delete(id);
    /// <summary>Добавить объект разработки (асинхронно).</summary>
    /// <param name="entity">Новый объект разработки.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сгенерированный Id.</returns>
    public new async Task<long> AddAsync(DevTargetEntity entity, CancellationToken ct = default) => await base.AddAsync(entity, ct);
    /// <summary>Обновить объект разработки (асинхронно).</summary>
    /// <param name="entity">Объект разработки с изменениями.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task UpdateAsync(DevTargetEntity entity, CancellationToken ct = default) => await base.UpdateAsync(entity, ct);
    /// <summary>Добавить или обновить объект разработки (асинхронно).</summary>
    /// <param name="entity">Объект разработки для сохранения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Persistent-экземпляр после Merge.</returns>
    public new async Task<DevTargetEntity> AddOrUpdateAsync(DevTargetEntity entity, CancellationToken ct = default) => await base.AddOrUpdateAsync(entity, ct);
    /// <summary>Удалить объект разработки по Id (асинхронно).</summary>
    /// <param name="id">Идентификатор объекта разработки.</param>
    /// <param name="ct">Токен отмены.</param>
    public new async Task DeleteAsync(long id, CancellationToken ct = default) => await base.DeleteAsync(id, ct);
}
